using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PortalRoemmers.Areas.Marketing.Models.Estimacion;
using PortalRoemmers.Areas.Marketing.Models.Actividad;
using PortalRoemmers.Areas.Marketing.Models.SolicitudGastoMkt;
using System.Data.SqlClient;

namespace PortalRoemmers.Areas.Marketing.Services.Estimacion
{
    public class EstimacionRepositorio
    {
        Ennumerador enu = new Ennumerador();
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search, string primero, string actual)
        {
            //------------------------------------------------------------------------
            DateTime p = DateTime.Parse(primero); //desde
            DateTime a = DateTime.Parse(actual).AddHours(23).AddMinutes(59);//hasta
            //------------------------------------------------------------------------
            using (var db = new ApplicationDbContext())
            {
                //------------------------------------------------------------------------
                var model = db.tb_Estim
                    .Include(x => x.actividad.especialidad)
                    .Include(x => x.actividad.responsable.empleado)
                    .Include(x => x.presupuesto)
                    .Include(x => x.moneda)
                    .Include(x => x.estado)
                    .Include(x => x.detalleEstim_Gas.Select(z=>z.detSolGas.Select(y=>y.solicitud))).DefaultIfEmpty()
                    .OrderByDescending(x => new { x.fchCreActiv ,x.idActiv})
                    .Where(x => x.actividad.idAccRes == SessionPersister.UserId && (x.idActiv.Contains(search) ||
                    x.actividad.responsable.empleado.nomComEmp.Contains(search) ||
                    x.actividad.nomActiv.Contains(search)) && ((x.actividad.fchIniActiv >= p) && (x.actividad.fchFinActiv <= a)))
                    //.Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    //.Take(cantidadRegistrosPorPagina)
                    .ToList();
                //------------------------------------------------------------------------
                var modelo = new ViewModels.IndexViewModel();
                modelo.Estimaciones = model;
                //------------------------------------------------------------------------
                return modelo;
            }
            //------------------------------------------------------------------------
        }
        public EstimacionModels obtenerItem(string id)
        {
            EstimacionModels model = new EstimacionModels();
            //var db = new ApplicationDbContext();
            using (var db = new ApplicationDbContext())
            {
                model = db.tb_Estim
                        .Include(x => x.actividad.especialidad)
                        .Include(x => x.actividad.responsable.empleado)
                        .Include(x => x.presupuesto)
                        .Include(x => x.linea)
                        .Include(x => x.moneda)
                        .Include(x => x.estado)
                        .Include(y => y.detalleEstim_Gas.Select(x=>x.gastoActiv.tipoGastoActividad)).DefaultIfEmpty()
                        .Include(z => z.detalleEstim_Fam.Select(y => y.familia)).DefaultIfEmpty()
                        .Include(z => z.detalleEstim_Fam.Select(y => y.areaTerap)).DefaultIfEmpty()
                        .Where(x=>x.idActiv == id).FirstOrDefault();
            }
            return model;
        }
        public List<EstimacionModels> obtenerTodos()
        {
            var db = new ApplicationDbContext();
            List<EstimacionModels> model = db.tb_Estim
                                        .Include(x=>x.moneda)
                                        .Include(x=>x.actividad)
                                        .OrderBy(x => x.idActiv).Where(x => x.idEst != ConstantesGlobales.estadoInactivo).ToList();
            return model;
        }
        public Boolean crear(EstimacionModels model)
        {
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.tb_Estim.Add(model);
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
        public string modificar(EstimacionModels model)
        {
            string mensaje = "";
            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    mensaje = "exito";
                }
                catch (Exception e)
                {
                    mensaje = "error";
                }
            }
            return mensaje;
        }
        public Boolean modificaDetalleGastosEstim(List<DetEstim_GastActModels> listaActual, string idCod_Act)
        {
            string msn = "";
            string mensaje1 = "";
            string mensaje2 = "";
            bool diferencias = false;
            bool semejanzas = false;
            bool resultado = false;
            List<DetEstim_GastActModels> listaAnterior = new List<DetEstim_GastActModels>();
            List<DetEstim_GastActModels> listaDeEliminados = new List<DetEstim_GastActModels>();
            List<DetEstim_GastActModels> listaDeAgregados = new List<DetEstim_GastActModels>();
            List<DetEstim_GastActModels> listaDeComunes = new List<DetEstim_GastActModels>();
            //////////---------------------//////////
            using (var db = new ApplicationDbContext())
            {
                //////////---------------------//////////
                listaAnterior = db.tb_DetEstim_Gas.Where(x => x.idActiv == idCod_Act).ToList();
                //////////---------------------//////////
                listaDeEliminados = listaAnterior.Where(x => !listaActual.Any(y => y.idActGas == x.idActGas)).ToList();
                //////////---------------------//////////
                listaDeAgregados = listaActual.Where(y => !listaAnterior.Any(x => x.idActGas == y.idActGas)).ToList();
                //////////---------------------//////////
                listaDeComunes = listaActual.Where(y => listaAnterior.Any(x => x.idActGas == y.idActGas)).ToList();
                //////////---------------------//////////

                if (listaDeEliminados.Count != 0)
                {
                    db.tb_DetEstim_Gas.RemoveRange(listaDeEliminados);
                }
                if (listaDeAgregados.Count != 0)
                {
                    db.tb_DetEstim_Gas.AddRange(listaDeAgregados);
                }
                try
                {
                    db.SaveChanges();
                    mensaje1 = "Exito";
                    diferencias = true;
                }
                catch (Exception e)
                {
                    mensaje1 = "Error: " + e.Message;
                    diferencias = false;
                }
            }
            //////////---------------------//////////
            using (var db = new ApplicationDbContext())
            {
                if (listaDeComunes.Count != 0)
                {
                    foreach (var k in listaDeComunes)
                    {
                        var model = db.tb_DetEstim_Gas.FirstOrDefault(x => x.idActiv==k.idActiv && x.idActGas == k.idActGas);
                        if (model != null)
                        {
                            db.Entry(model).State = EntityState.Detached;
                        }
                        db.Entry(k).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                            semejanzas = true;
                            mensaje2 = "... Exito ...";
                        }
                        catch (Exception e)
                        {
                            semejanzas = false;
                            mensaje2 = "... Error: " + e.Message; ;
                        }
                    }
                }
                else
                {
                    semejanzas = true;
                }
            }
            //////////---------------------//////////
            if (diferencias & semejanzas)
            {//detalle fue generado bien
                resultado = true;
            }
            else
            {//detalle fue generado mal
                resultado = false;
            }
            msn = mensaje1 + mensaje2;
            //////////---------------------//////////
            return resultado;
        }
        public Boolean modificaDetalleFamEstim(List<DetEstim_FamProdModels> listaActual, string idCod_Act)
        {
            string mensaje = "";
            //////////---------------------//////////
            using (var db = new ApplicationDbContext())
            {
                //////////---------------------//////////
                var listaAnterior = db.tb_DetEstim_Fam.Where(x => x.idActiv == idCod_Act).ToList();
                //////////---------------------//////////
                var listaDeEliminados = listaAnterior.Where(x => !listaActual.Any(y => y.idFamRoe == x.idFamRoe)).ToList();
                //////////---------------------//////////
                var listaDeAgregados = listaActual.Where(y => !listaAnterior.Any(x => x.idFamRoe == y.idFamRoe)).ToList();
                //////////---------------------//////////
                //////////---------------------//////////
                if (listaDeEliminados.Count != 0)
                {
                    db.tb_DetEstim_Fam.RemoveRange(listaDeEliminados);
                }
                if (listaDeAgregados.Count != 0)
                {
                    db.tb_DetEstim_Fam.AddRange(listaDeAgregados);
                }
                try
                {
                    db.SaveChanges();
                    mensaje = "Exito";
                    return true;
                }
                catch (Exception e)
                {
                    mensaje = "Error: " + e.Message;
                    return false;
                }
                //////////---------------------//////////
            }
        }
        public Boolean crearDetalleG(List<DetEstim_GastActModels> model)
        {
            using (var db = new ApplicationDbContext())
            {
                db.tb_DetEstim_Gas.AddRange(model);
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                    return false;
                }
            }
        }
        public Boolean crearDetalleF(List<DetEstim_FamProdModels> model)
        {
            using (var db = new ApplicationDbContext())
            {
                db.tb_DetEstim_Fam.AddRange(model);
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                    return false;
                }
            }
        }
        public string eliminar(string id)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            EstimacionModels model = db.tb_Estim.Find(id);
            var listaEstimGas = db.tb_DetEstim_Gas.Where(x => x.idActiv == id).ToList();
            var listaEstimFam = db.tb_DetEstim_Fam.Where(x => x.idActiv == id).ToList();
            db.tb_DetEstim_Gas.RemoveRange(listaEstimGas);
            db.tb_DetEstim_Fam.RemoveRange(listaEstimFam);
            db.tb_Estim.Remove(model);
            try
            {
                db.SaveChanges();
                mensaje = "<div id='success' class='alert alert-success'>Se eliminó un registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public List<DetEstim_GastActModels> obtenerDetalleEstimacion(string idActiv)
        {
            var db = new ApplicationDbContext();
            List<DetEstim_GastActModels> model = db.tb_DetEstim_Gas.Include(x=>x.gastoActiv).Include(x => x.gastoActiv.tipoGastoActividad).Include(x=>x.cabecera).Where(x=>x.idActiv== idActiv).ToList();
            return model;
        }
        public List<DetSolGasto_GasModels> obtenerDetalleSolGas(string idActiv)
        {
            var db = new ApplicationDbContext();
            List<DetSolGasto_GasModels> model = db.tb_DetSolGas_Gas.Include(x=>x.solicitud).Where(x=>x.idActiv== idActiv).ToList();
            return model;
        }
        public Boolean modificarDetEstimSalReal(double actualizado, string idActiv , string idActGas, out string mensaje)
        {
            Boolean exito = false;
            var db = new ApplicationDbContext();
            string commandText = "UPDATE tb_DetEstim_Gas SET  salReal = @salReal ,userModEstGast = @usuMod , fchModEstGast= @usufchMod WHERE idActiv = @idActiv and idActGas = @idActGas";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {

                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@salReal", actualizado);
                command.Parameters.AddWithValue("@idActiv", idActiv);
                command.Parameters.AddWithValue("@idActGas", idActGas);
                command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                command.Parameters.AddWithValue("@usufchMod", DateTime.Now);
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    mensaje = "<div id='success' class='alert alert-success'>Se modifico el saldo estimado del presupuesto.</div>";
                    if (rowsAffected != 0)
                        exito = true;
                    else
                        exito = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    mensaje = "<div id='warning' class='alert alert-warning'>" + ex.Message + "</div>";
                    exito = false;
                }

            }
            return exito;
        }

        public List<DetEstim_FamProdModels> obtenerDetalleFamilia(string idActiv)
        {
            var db = new ApplicationDbContext();
            List<DetEstim_FamProdModels> model = db.tb_DetEstim_Fam.Include(x=>x.familia).Include(x=>x.areaTerap).Where(x => x.idActiv == idActiv).ToList();
            return model;
        }
        public List<DetActiv_MedModels> obtenerDetalleMedico(string idActiv)
        {
            var db = new ApplicationDbContext();
            List<DetActiv_MedModels> model = db.tb_DetAct_Med.Include(x => x.cliente).Where(x => x.idActiv == idActiv).ToList();
            return model;
        }
        public List<EstimacionModels> obtenerEstimaciones()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Estim
                    .Include(x => x.actividad.especialidad)
                    .Include(x => x.actividad.responsable.empleado)
                    .Include(x => x.presupuesto)
                    .Include(x => x.moneda)
                    .Include(x => x.estado)
                    //.Include(x => x.detalleEstim_Gas.Select(z => z.detSolGas.Select(y => y.solicitud))).DefaultIfEmpty()
                    .Include(x => x.detalleEstim_Gas.Select(z => z.detSolGas.Select(y => y.solicitud.movimiento))).DefaultIfEmpty()
                    .Include(x => x.detalleEstim_Gas.Select(z => z.detSolGas.Select(y => y.solicitud.liquidacion))).DefaultIfEmpty()
                    .OrderByDescending(x => new { x.fchCreActiv, x.idActiv }).ToList();

                /*var modelo = new ViewModels.IndexViewModel();
                modelo.Estimaciones = model;
                return modelo;*/

                return model;
            }
        }
    }
}