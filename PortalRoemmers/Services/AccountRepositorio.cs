using System;
using System.Linq;
using System.Web;
using System.Data.Entity; //permite usar landa
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System.Security.Cryptography;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Menu;
using PortalRoemmers.Helpers;
using PortalRoemmers.Areas.Sistemas.Models.Menu;
using PortalRoemmers.Areas.Sistemas.Services.Enlace;
using PortalRoemmers.Areas.RRHH.Services.Formulario;

namespace PortalRoemmers.Services
{
    public class AccountRepositorio
    {
        UsuarioRepositorio us;
        MenuRepositorio me;
        EmpleadoRepositorio em;
        TipoEnlaceRepositorio te;
        Form_Usu_Repositorio formUsu;
        public AccountRepositorio()
        {
            us = new UsuarioRepositorio();
            me = new MenuRepositorio();
            em = new EmpleadoRepositorio();
            te = new TipoEnlaceRepositorio();
            formUsu = new Form_Usu_Repositorio();
        }

        public UsuarioModels obtenerlogin(UsuarioModels acc) {

           using (var db = new ApplicationDbContext()){ 
 
                //transformo la contraseña
                var pass = "";
                using (MD5 md5Hash = MD5.Create())
                {
                    pass = GetMd5Hash(md5Hash, acc.userpass);
                }
                var cuenta = db.tb_Usuario.Include(y=>y.empleado).Include(x => x.accRoles).Where(x => x.username == acc.username && x.userpass == pass).FirstOrDefault();
                return cuenta;
           }
        }

        public UsuarioModels obtenerUsu(string id)
        {
            var db = new ApplicationDbContext();

            var cuenta = db.tb_Usuario.Include(x=>x.empleado).Include(x => x.accRoles).Where(x => x.idAcc == id).FirstOrDefault();

            return cuenta;
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public void sessionUser(UsuarioModels cuenta)
        {

            if (cuenta != null)
            {
                SessionPersister.Username = cuenta.username;
                //SessionPersister.Usernom1 = cuenta.empleado.nom1Emp;
               // SessionPersister.Usernom2 = cuenta.empleado.nom2Emp;
               // SessionPersister.Userapepat = cuenta.empleado.apePatEmp;
               // SessionPersister.Userapemat = cuenta.empleado.apeMatEmp;
                SessionPersister.UserId = cuenta.idAcc;
               // SessionPersister.UserCarg = cuenta.empleado.idCarg;
               // SessionPersister.EmpId = cuenta.empleado.idEmp;
                SessionPersister.NivApr = cuenta.idNapro;
               // SessionPersister.DniEmp = cuenta.empleado.nroDocEmp;
                //si la imagen es nula le pongo una imagen por defecto
                if (cuenta.rutaImgPer == null)
                {
                    string locacion = HttpContext.Current.Server.MapPath("~/Areas/Sistemas/FotoPerfil/default.png");
                    FileStream foto = new FileStream(locacion, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    Byte[] arreglo = new Byte[foto.Length];
                    BinaryReader reader = new BinaryReader(foto);
                    arreglo = reader.ReadBytes(Convert.ToInt32(foto.Length));
                    SessionPersister.UserIma = Encoding.Default.GetString(arreglo);
                    reader.Close();
                }
                else
                {
                    SessionPersister.UserIma = Encoding.Default.GetString(cuenta.rutaImgPer);
                }
                //envio los permisos convertidos
                using (var db = new ApplicationDbContext())
                {
                    var vroles = cuenta.accRoles;
                    var inner = (from vr in vroles join or in db.tb_Roles on vr.rolId equals or.rolId select new { or.rolId }).ToList();
                    List<string> t = new List<string>();
                    foreach (var e in inner)
                    {
                        t.Add(e.rolId.ToString());
                    }
                    string s = string.Join(",", t);
                    SessionPersister.UserRol = s;
                }
                //Menu configurable
                List<MenuModels> rolArbol = me.obtenerMenus().Where(x => x.ParentId == cuenta.idMen).Traverse(x => x.Childs).OrderBy(x => x.ordMen).ToList();
                HttpContext.Current.Session[Sessiones.formulario] = formUsu.obtenerFormulariosxUsuario(cuenta.idAcc);
                HttpContext.Current.Session[Sessiones.menu] = rolArbol;
                HttpContext.Current.Session[Sessiones.empleado] = cuenta.empleado;
                SessionPersister.UserMenu = cuenta.idMen;
                SessionPersister.ActiveMenuI = rolArbol.Where(x => x.tiMen.ToUpper() == "INICIO" && x.imgMen.Trim() == "mdi mdi-home-outline fa-fw").Select(x => x.idMen).FirstOrDefault();
                SessionPersister.ActiveVistaI = rolArbol.Where(x => x.tiMen.ToUpper() == "BIENVENIDA" && x.imgMen.Trim()=="mdi mdi-human-handsup fa-fw").Select(x => x.idMen).FirstOrDefault();
                HttpContext.Current.Session[Sessiones.cumple] = em.obtenerCumpleanio();
                HttpContext.Current.Session[Sessiones.enlace] = te.obtenerTipoEnlance();
            }
        }

        public bool updateConfirEmail(string idAcc)
        {
            string commandText = "UPDATE tb_Usuario SET confirmEmail = @confirmEmail, tokenUsu=@tokenUsu  WHERE idAcc = @idAcc ;";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idAcc", SqlDbType.Int);
                command.Parameters["@idAcc"].Value = idAcc;

                command.Parameters.AddWithValue("@confirmEmail", true);
                command.Parameters.AddWithValue("@tokenUsu", "");

                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }

            }
           
        }

        public UsuarioModels obUsuEmail(string email)
        {
            using (var db = new ApplicationDbContext())
            {
                var cuenta = db.tb_Usuario.Include(x=>x.empleado).Where(x => x.email == email).SingleOrDefault();
                return cuenta;
            }

        }

        public string generarToken(string idAcc) {
            // necesario
            string DESKEY = Encrypt.ED_KEY;
            byte[] _DESkey;
            _DESkey = ASCIIEncoding.ASCII.GetBytes(DESKEY);
            //CUANTO TIEMPO LE DARE
            TimeSpan ExpiresSpan = TimeSpan.FromMinutes(10);
            var requestAt = DateTime.Now; //UtcNow
            var expiresIn = requestAt + ExpiresSpan;
            //concatenar datos a guardar
            string cadena = expiresIn + "-" + idAcc.ToString();
            string token = EncryptDecrypt.Encrypt(cadena, _DESkey);

            //Token generado lo guardo en la tabla de usuario
            string commandText = "UPDATE tb_Usuario SET tokenUsu = @tokenUsu  WHERE idAcc = @idAcc;";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idAcc", SqlDbType.Int);
                command.Parameters["@idAcc"].Value = idAcc;

                command.Parameters.AddWithValue("@tokenUsu", token);
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
                return token;
        } 
        
        public string[] valorRealToken(string encriptado)
        {
            // necesario
            string DESKEY = Encrypt.ED_KEY;
            byte[] _DESkey;
            _DESkey = ASCIIEncoding.ASCII.GetBytes(DESKEY);

            string desincriptado = EncryptDecrypt.Decrypt(encriptado, _DESkey);

            var valores = desincriptado.Split(new char[] { '-' });

            return valores;
        }

        public bool resetearPassword(string password, string id) {
            //encripto la contraseña
            var pass = "";
            using (MD5 md5Hash = MD5.Create())
            {
                pass = GetMd5Hash(md5Hash, password);
            }

            //realizo la comnsulta 
            string commandText = "UPDATE tb_Usuario SET userpass = @userpass , tokenUsu=@tokenUsu  WHERE idAcc = @idAcc;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idAcc", SqlDbType.Int);
                command.Parameters["@idAcc"].Value = id;

                command.Parameters.AddWithValue("@userpass", pass);
                command.Parameters.AddWithValue("@tokenUsu", "");
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }

            

        }


    }
}