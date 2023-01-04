using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using PortalRoemmers.Models;
using PortalRoemmers.Areas.Contabilidad.Models.Letra;
using System.Data.Entity;
using PortalRoemmers.Helpers;
using System.Data;
using PortalRoemmers.Security;

namespace PortalRoemmers.Areas.Contabilidad.Services.Letra
{
    public class LetraService
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

                var model = db.tb_Letra
                    .OrderBy(x => x.idLetra).Where(x => x.aceptante.nomAceptante.Contains(search) || x.codLetra.Contains(search) || x.refLetra.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Letra.Where(x => x.aceptante.nomAceptante.Contains(search) || x.codLetra.Contains(search) || x.refLetra.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Letras = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public LetraModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            LetraModels model = db.tb_Letra.Find(id);
            return model;
        }
        public LetraModels obtenerItemEspecifico(string id)
        {
            var db = new ApplicationDbContext();
            LetraModels model = db.tb_Letra
                                .Include(x => x.aceptante)
                                .Include(x => x.estado)
                                .Include(x => x.moneda)
                                .OrderBy(x => x.idLetra).Where(x=>x.idLetra==id).FirstOrDefault();
            return model;
        }
        public string crear(LetraModels model)
        {
            string mensaje = "";
            //-----------------------------------
            //creo su ID
            string tabla = "tb_Letra";
            string id = enu.buscarTabla(tabla).ToString();
            enu.actualizarTabla(tabla, Int32.Parse(id));
            model.idLetra = id;
            //-----------------------------------
            var db = new ApplicationDbContext();
            db.tb_Letra.Add(model);
            try
            {
                db.SaveChanges();
                mensaje = "<div id='success' class='alert alert-success'>Se creó el registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(LetraModels model)
        {
            string mensaje = "";
            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    mensaje = "<div id='success' class='alert alert-success'>Se modificó el registro.</div>";
                }
                catch (Exception e)
                {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                }
            }
            return mensaje;
        }
        public string eliminar(string id)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            LetraModels model = db.tb_Letra.Find(id);

            if (model.idEst == ConstantesGlobales.estadoInactivo)
            {
                db.tb_Letra.Remove(model);
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
            else
            {
                mensaje = "<div id='success' class='alert alert-success'>El registro no se puede eliminar debido al estado ACTIVO.</div>";
            }
            return mensaje;
        }
        //listados
        public List<LetraModels> obtenerLetras()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Letra
                .Include(x => x.aceptante)
                .Include(x => x.estado)
                .Include(x => x.moneda)
                .OrderBy(x => x.idLetra)
                .ToList();
                return model;
            }
        }
        //Obtener Letras a Aprobar -- segun regla de negocio tiene que ser una letra registrada
        public List<LetraModels> obtenerLetrasRegistradas()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Letra
                .Include(x => x.aceptante)
                .Include(x => x.estado)
                .Include(x => x.moneda)
                .Where(x=>x.idEst == ConstantesGlobales.estadoRegistrado)
                .OrderBy(x => x.idLetra)
                .ToList();
                return model;
            }
        }
        //Actualizar estado de letras
        public Boolean updateEstadoLetra(string let, string est)
        {
            string commandText = "UPDATE tb_Letra SET idEst = @idEst,  usuMod=@usuMod , usufchMod=@usufchMod   WHERE idLetra = @idLetra;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                command.Parameters.Add("@idLetra", SqlDbType.VarChar);
                command.Parameters["@idLetra"].Value = let;

                command.Parameters.AddWithValue("@idEst", est);
                command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                command.Parameters.AddWithValue("@usufchMod", DateTime.Now);

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