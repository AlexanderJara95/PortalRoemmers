using PortalRoemmers.Areas.Sistemas.Models.Roles;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class UsuarioRepositorio
    {
        Ennumerador enu = new Ennumerador();
        public IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;
            if (pagina == 0)
            {
                pagina = 1;
            }
            using (var db = new ApplicationDbContext())
            {
                var usuarios = db.tb_Usuario.Include(x=>x.empleado).Include(x => x.estado).Include(x => x.aprobacion).OrderBy(x => new { x.empleado.apePatEmp, x.empleado.apeMatEmp }).Where(z => z.empleado.nom1Emp.Contains(search) || z.empleado.nom2Emp.Contains(search) || z.empleado.apePatEmp.Contains(search) || z.empleado.apeMatEmp.Contains(search) || z.estado.nomEst.Contains(search) || z.empleado.nomComEmp.Contains(search) || z.idAcc==search || z.username.Contains(search))
                                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Usuario.Include(x => x.empleado).Include(x => x.estado).Include(x => x.aprobacion).Where(z => z.empleado.nom1Emp.Contains(search) || z.empleado.nom2Emp.Contains(search) || z.empleado.apePatEmp.Contains(search) || z.empleado.apeMatEmp.Contains(search) || z.estado.nomEst.Contains(search) || z.empleado.nomComEmp.Contains(search) || z.idAcc == search).Count();

                var modelo = new IndexViewModel();
                modelo.Usuarios = usuarios;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public UsuarioModels obtenerItem(string id)
        {
            UsuarioModels cuenta = new UsuarioModels();

            if (id!=null)
            {
                using (var db = new ApplicationDbContext())
                {
                    cuenta = db.tb_Usuario.Include(x => x.empleado).Include(x => x.empleado.cargo).Include(x => x.empleado.area).Include(x => x.empleado.afp).Include(x => x.aprobacion).Where(y => y.idAcc == id).First();
                    return cuenta;
                }

            }
            return cuenta;
        }
        public UsuarioModels obtenerItemXUsername(string username)
        {
            UsuarioModels cuenta = new UsuarioModels();

            if (username != null)
            {
                using (var db = new ApplicationDbContext())
                {
                    cuenta = db.tb_Usuario.Include(x => x.empleado).Include(x => x.empleado.cargo).Include(x => x.empleado.area).Include(x => x.empleado.afp).Include(x => x.aprobacion).Where(y => y.username == username).First();
                    return cuenta;
                }

            }
            return cuenta;
        }
        
        public UsuarioModels obtenerItemXEmpleado(string id)
        {
            UsuarioModels cuenta = new UsuarioModels();

            if (id != null)
            {
                using (var db = new ApplicationDbContext())
                {
                    cuenta = db.tb_Usuario.Include(x => x.empleado).Include(x => x.empleado.cargo).Include(x => x.empleado.area).Include(x => x.empleado.afp).Include(x => x.aprobacion).Where(y => y.idEmp == id).First();
                    return cuenta;
                }

            }
            return cuenta;
        }
        public List<UsuarioModels> obtenerUsuariosXSolicitudMasiva(string varSolicitud)
        {
            using (var db = new ApplicationDbContext())
            {
                // Asumiendo que 'varSolicitud' es un ID y que cada solicitud masiva tiene una lista de grupos asociados.
                var usuariosDeSolicitudMasiva = db.tb_SolicitudRRHH
                    .Where(solicitud => solicitud.idSolicitudRrhh == varSolicitud && solicitud.idSubTipoSolicitudRrhh == ConstantesGlobales.subTipoVacacionesM)
                    .SelectMany(solicitud => db.tb_GrupoSolicitudRRHH
                        .Where(grupoSol => grupoSol.idSolicitudRrhh == solicitud.idSolicitudRrhh)
                        .SelectMany(grupoSol => db.tb_GrupoRRHH
                            .Where(grupo => grupo.idGrupoRrhh == grupoSol.idGrupoRrhh)
                            .SelectMany(grupo => db.tb_AreaGrupoRRHH
                                .Where(areaGrupo => areaGrupo.idGrupoRrhh == grupo.idGrupoRrhh)
                                .SelectMany(areaGrupo => db.tb_Empleado
                                    .Where(emp => emp.idAreRoe == areaGrupo.idAreRoe)
                                    .SelectMany(emp => emp.usuarios)
                                )
                            )
                        )
                    ).ToList();
                return usuariosDeSolicitudMasiva;
            }
        }

        public Boolean crear(UsuarioModels model)
        {
            Usu_RolModels ur = new Usu_RolModels();
            Boolean mensaje =false;
             using (var db = new ApplicationDbContext())
                {

                //creo su ID
                string idc = enu.buscarTabla("tb_Usuario").ToString();
                model.idAcc = idc;
                model.fchCreUsu = DateTime.Now;
                model.userCreUsu= SessionPersister.Username;
                //creo la contraseña encriptada
                using (MD5 md5Hash = MD5.Create())
                    {
                        model.userpass = GetMd5Hash(md5Hash, model.userpass);
                        model.confirmPassword = model.userpass;
                    }

                    //9 home_bienvenida siempre
                    ur.idAcc = idc;
                    ur.rolId = ConstantesGlobales.rolBienvenida;
                    ur.usuCrea = "Automatico";
                    ur.usufchCrea = DateTime.Now;
                    db.tb_Usuario.Add(model);
                    db.tb_Usu_Rol.Add(ur);
                    try {
                        db.SaveChanges();
                        enu.actualizarTabla("tb_Usuario", int.Parse(idc));
                        mensaje = true;
                     }
                    catch(Exception e) {
                        e.Message.ToString();
                    }
                }
            return mensaje;
        }
        public string modificar(UsuarioModels model)
        {
            string mensaje = "";
            using (var db = new ApplicationDbContext())
            {
                model.fchModUsu = DateTime.Now;
                model.userModUsu= SessionPersister.Username;

                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    mensaje = "<div id='success' class='alert alert-success'>Se modificó el registro.</div>";
                }
                catch ( Exception e)
                {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                }
              
            }
            return mensaje;
        }
        public string eliminar(int id)
        {
            string mensaje = "";
            using (var db = new ApplicationDbContext())
            {
                UsuarioModels acc = db.tb_Usuario.Find(id);
                db.tb_Usuario.Remove(acc);
                try
                {
                    db.SaveChanges();
                    mensaje = "<div id='success' class='alert alert-success'>Se eliminó un registro.</div>";
                }
                catch (Exception e)
                {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                }
            }
            return mensaje;
        }
        public string updateEstUsu(string idAcc)
        {
            string commandText = "UPDATE tb_Usuario SET idEst = @estUsu  WHERE idAcc = @idAcc;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idAcc", SqlDbType.Int);
                command.Parameters["@idAcc"].Value = idAcc;
                command.Parameters.AddWithValue("@estUsu", ConstantesGlobales.estadoCesado);

                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return " < div id = 'warning' class='alert alert-warning'>Se produjo un error</div>";
                }

            }
            return "<div id='success' class='alert alert-success'>Se cambio estado a Cesado.</div>";
        }

        //encriptar contraseña
        public string GetMd5Hash(MD5 md5Hash, string input)
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
        public List<UsuarioModels> obtenerUsuarios()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Usuario.Include(x=>x.empleado).Include(x=>x.aprobacion).Where(x=>x.idEst!=ConstantesGlobales.estadoCesado).OrderBy(x => x.idAcc).ToList();
            return model;
        }
        public List<UsuarioModels> obtenerUsuariosGeneral()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Usuario.Include(x => x.empleado).Include(x => x.aprobacion).OrderBy(x => x.idAcc).ToList();
            return model;
        }
        public void agregarMenuUsuario(string[] idAcc, string id)
        {
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {

                if (idAcc != null)
                {
                    foreach (var a in idAcc)
                    {
                        string commandText = "UPDATE tb_Usuario SET idMen = @idMen  WHERE idAcc = @idAcc;";
                        SqlCommand command = new SqlCommand(commandText, connection);
                        command.Parameters.Add("@idAcc", SqlDbType.NVarChar);
                        command.Parameters["@idAcc"].Value = a;
                        command.Parameters.AddWithValue("@idMen", id);
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
                }
            }
        }
        public void eliminarMenuUsuario(string[] idAcc)
        {
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {

                if (idAcc != null)
                {
                    foreach (var a in idAcc)
                    {

                        string commandText = "UPDATE tb_Usuario SET idMen = @idMen  WHERE idAcc = @idAcc;";
                        SqlCommand command = new SqlCommand(commandText, connection);
                        command.Parameters.Add("@idAcc", SqlDbType.NVarChar);
                        command.Parameters["@idAcc"].Value = a;
                        command.Parameters.AddWithValue("@idMen", DBNull.Value);
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
                }
            }
        }
        public string obtenerNivelAproUsu(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var resultado= db.tb_Usuario.Where(x => x.idAcc == id).Select(x => x.idNapro).FirstOrDefault();
                return resultado;
            }
        }
        public Boolean cesadoUsu(string idEmp)
        {
            string commandText = "UPDATE tb_Usuario SET idEst = @idEst, fchModUsu=@fchModUsu,userModUsu=@userModUsu  WHERE idEmp = @idEmp;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idEmp", SqlDbType.NVarChar);
                command.Parameters["@idEmp"].Value = idEmp;
                command.Parameters.AddWithValue("@idEst", ConstantesGlobales.estadoCesado);
                command.Parameters.AddWithValue("@fchModUsu", DateTime.Now);
                command.Parameters.AddWithValue("@userModUsu", SessionPersister.Username);
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }

            }
            return true;
        }
        public Boolean updatePassword(string idAcc, string userpass)
        {
            string commandText = "UPDATE tb_Usuario SET userpass = @userpass, fchModUsu = @fchModUsu,userModUsu = @userModUsu  WHERE idAcc = @idAcc;";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idAcc", SqlDbType.NVarChar);
                command.Parameters["@idAcc"].Value = idAcc;

                command.Parameters.AddWithValue("@userpass", userpass);
                command.Parameters.AddWithValue("@fchModUsu", DateTime.Now);
                command.Parameters.AddWithValue("@userModUsu", SessionPersister.Username);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
                return true;
        }

    }
}