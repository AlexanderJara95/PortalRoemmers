using PortalRoemmers.Areas.Marketing.Models.FarmacoVigilancia;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace PortalRoemmers.Areas.Marketing.Services.FarmacoVigilancia
{
    public class EventoAdversoRepositorio
    {
        Ennumerador enu = new Ennumerador();

        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }

            using (var db = new ApplicationDbContext())
            {

                var model = db.tb_Eve_Adv
                    .OrderByDescending(x => x.idEveAdv).Where( x =>x.usuCrea== SessionPersister.Username && (x.nomComEmp.Contains(search) || x.desProEveAdv.Contains(search)||x.usufchCrea.ToString().Contains(search)))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Eve_Adv.Where(x => x.usuCrea == SessionPersister.Username && (x.nomComEmp.Contains(search) || x.desProEveAdv.Contains(search) || x.usufchCrea.ToString().Contains(search))).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.EventoAdverso = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }

        public Boolean crear(EventoAdversoModels model)
        {
            Boolean mensaje = false;
            var db = new ApplicationDbContext();

            //creo su ID
            string tabla = "tb_Eve_Adv";
            int idc = enu.buscarTabla(tabla);
            model.idEveAdv = idc.ToString("D8");

            db.tb_Eve_Adv.Add(model);
            try
            {
                db.SaveChanges();
                enu.actualizarTabla(tabla, idc);
                mensaje = true;
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return mensaje;
        }

        public EventoAdversoModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                EventoAdversoModels model = db.tb_Eve_Adv.Include(x => x.gene).Where(x=>x.idEveAdv== id).FirstOrDefault();
                return model;
            }   
        }

        public Boolean modificar(EventoAdversoModels model)
        {
            Boolean mensaje = false;

            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            return mensaje;
        }

        public List<QueryAutoCom> obtenerProdDes()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Producto.Select(x => new QueryAutoCom { value = x.idProAX, text = x.nomPro }).ToList();
                return model;
            }
        }

        public class QueryAutoCom
        {
            public string value { get; set; }
            public string text { get; set; }

        }

        public Boolean actualizarVisualizar(string idEveAdv)
        {
            Boolean mensaje = false;
         
            string commandText = "update tb_Eve_Adv set usuVis=@usuVis,usufchVis=@usufchVis where idEveAdv=@idEveAdv;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@usuVis", SqlDbType.VarChar).Value = SessionPersister.Username;
                command.Parameters.Add("@usufchVis", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@idEveAdv", SqlDbType.VarChar).Value = idEveAdv;   
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    mensaje = true;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return mensaje;
        }

        public List<EventoAdversoModels> obtenerEventosAdversos()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Eve_Adv.Include(x=>x.gene).OrderBy(x => x.idEveAdv).ToList();
                return model;
            }
        }
   
        public Boolean CorreoNuevoRegistro()
        {
            
            EmailHelper em = new EmailHelper();
            bool ok = false;
            string titulo = "Alerta de nuevo registro de evento adverso - "+SessionPersister.Username;
            string mensaje = string.Format(
                "<section> Estimado(a) <span style=\"font - weight: bold;\">Usuario(a)</span><BR/> <p>El documento {0} ya se encuentra en el portal  <BR/> Para visualizar el registro ingresa a  <a href=\"{1}\" title=\"Megalabs-latam\">Megalas-latam.com.pe</a> </p>  </section>", "evento adverso", "https:\\megalabs-latam.com.pe");

            string correo = "farmacovigilancia@megalabs.com.pe"; 
            //"kkina@megalabs.com.pe", "jpelaez@megalabs.com.pe"
            try
            {
                ok = em.SendEmail(correo, mensaje, titulo, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }

            return ok;
        }


    }
}