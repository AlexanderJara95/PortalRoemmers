using PortalRoemmers.Models;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PortalRoemmers.Helpers;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;

namespace PortalRoemmers.Areas.Sistemas.Services.Presupuesto
{
    public class PresupuestoRepositorio
    {
        Ennumerador enu = new Ennumerador();
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search, string primero, string actual)
        {
            //------------------------------------------------------------------------
            DateTime p = DateTime.Parse(primero); //desde
            DateTime a = DateTime.Parse(actual).AddHours(23).AddMinutes(59);//hasta
            //------------------------------------------------------------------------
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }

            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Pres
                    .Include(x => x.tipospres)
                    .Include(x => x.estado)
                    .Include(x => x.tipogasto)
                    .Include(x => x.concepto)
                    .Include(x => x.especialidad)
                    .Include(x => x.zona)
                    .Include(x => x.linea)
                    .Include(x => x.moneda)
                    .Include(x => x.responsable.empleado)
                    .Include(x => x.aprobador.empleado)
                    .OrderByDescending(x => x.idPres)
                    .Where(x => x.usuCrea == SessionPersister.Username && ((x.fchIniVigencia >= p) && (x.fchFinVigencia <= a)) && (x.idEst == ConstantesGlobales.estadoActivo && (x.tipospres.nomTipPres.ToString().Contains(search) || x.linea.nomLin.ToString().Contains(search) || x.zona.nomZon.ToString().Contains(search) || x.especialidad.nomEsp.ToString().Contains(search) || x.concepto.nomConGas.ToString().Contains(search) ||   x.responsable.empleado.nomComEmp.ToString().Contains(search) || x.idPres.Contains(search))))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Pres.Where(x => x.usuCrea == SessionPersister.Username && ((x.fchIniVigencia >= p) && (x.fchFinVigencia <= a)) && (x.idEst == ConstantesGlobales.estadoActivo && (x.tipospres.nomTipPres.ToString().Contains(search) || x.linea.nomLin.ToString().Contains(search) || x.zona.nomZon.ToString().Contains(search) || x.especialidad.nomEsp.ToString().Contains(search) || x.concepto.nomConGas.ToString().Contains(search) ||  x.responsable.empleado.nomComEmp.ToString().Contains(search) || x.idPres.Contains(search)))).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Pres = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public string crear(PresupuestoModels model)
        {
            string mensaje = "";
            //creo su ID
            string tabla = "tb_Pres";
            int idc = enu.buscarTabla(tabla);
            model.idPres = idc.ToString("D7");
            using (var db = new ApplicationDbContext())
            {
                db.tb_Pres.Add(model);
                try
                {
                    db.SaveChanges();
                    enu.actualizarTabla(tabla, idc);
                    mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro : "+ idc.ToString("D7") + "</div>";
                    //mensaje = idc.ToString("D7");
                }
                catch (Exception e)
                {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                    //mensaje = "error "+ e.ToString();
                }
            }
            return mensaje;
        }
        public PresupuestoModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                PresupuestoModels model = db.tb_Pres
                                        .Include(x=>x.tipospres)
                                        .Include(x=>x.estado)
                                        .Include(x=>x.concepto)
                                        .Include(x=>x.especialidad)
                                        .Include(x=>x.linea)
                                        .Include(x=>x.moneda)
                                        .Include(x => x.responsable.empleado)
                                        .Include(x => x.aprobador.empleado)
                                        .Include(x=>x.zona)
                                        .OrderBy(x=>x.idPres)
                                        .Where(x => x.idPres == id).FirstOrDefault();
                return model;
            }

        }
        public string modificar(PresupuestoModels model)
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
        public Boolean anular(PresupuestoModels model, out string mensaje)
        {
            if (model.Monto == model.Saldo)
            {
                string commandText = "UPDATE tb_Pres SET idEst = @idEst, usuMod = @usuMod , usufchMod = @usufchMod WHERE idPres = @idPres";
                using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
                {
                    SqlCommand command = new SqlCommand(commandText, connection);
                    command.Parameters.AddWithValue("@idPres", model.idPres);
                    command.Parameters.AddWithValue("@idEst", ConstantesGlobales.estadoInactivo);
                    command.Parameters.AddWithValue("@usuMod", model.usuMod);
                    command.Parameters.AddWithValue("@usufchMod", model.usufchMod);
                    try
                    {
                        connection.Open();
                        Int32 rowsAffected = command.ExecuteNonQuery();
                        connection.Close();
                        mensaje = "<div id='success' class='alert alert-success'>Se anulo el presupuesto.</div>";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        mensaje = "<div id='warning' class='alert alert-warning'>" + ex.Message + "</div>";
                        return false;
                    }

                }
                return true;
            }
            else
            {
                mensaje = "<div id='warning' class='alert alert-warning'>El presupuesto esta consumido , no se puede anular ...</div>";
                return false;
            }
        }
        public List<PresupuestoModels> obtenerListaSeleccion(string TipPto, string resp, string aprob , string lin , string zona , string tipgasto, string gasto, string espec)
        {
            //--
            using (var db = new ApplicationDbContext())
            {
                List<PresupuestoModels> model = new List<PresupuestoModels>();
                if (TipPto== ConstantesGlobales.plan_Trab)
                {
                    model = db.tb_Pres
                    .Include(x => x.tipospres)
                    /*.Include(x => x.responsable)
                    .Include(x => x.linea)
                    .Include(x => x.zona)
                    .Include(x => x.concepto)
                    .Include(x => x.especialidad)
                    .Include(x => x.aprobador) */
                    .Include(x => x.moneda)
                   .OrderBy(x => x.idPres)
                   .Where(x => (x.idTipoPres == TipPto && (x.idAccRes.ToString() == resp) && (x.idAccJ.ToString() == aprob) && (x.idLin == lin) && (x.idZon == zona)) && (x.fchIniVigencia<=DateTime.Today && x.fchFinVigencia>= DateTime.Today)&&(x.idEst!= ConstantesGlobales.estadoInactivo) && x.Saldo!=0 )
                   .ToList();
                }
                else if(TipPto == ConstantesGlobales.plan_Mark)
                {
                    if (tipgasto ==null)
                    {
                        tipgasto = "";

                    }
                    if (gasto == null)
                    {
                        gasto = "";

                    }
                    List<PresupuestoModels> model1 = null;
                    List<PresupuestoModels> model2 = null;

                    model1 = db.tb_Pres
                    .Include(x => x.tipospres)
                    .Include(x => x.moneda)
                    .OrderBy(x => x.idPres)
                    .Where(x => x.idAccRes.ToString() == resp && (x.idTipoPres == TipPto) && (x.idTipGas == tipgasto) && (x.idConGas == null) && (x.idEsp == espec) && (x.fchIniVigencia <= DateTime.Today && x.fchFinVigencia >= DateTime.Today) && (x.idEst != ConstantesGlobales.estadoInactivo) && x.Saldo != 0)
                    .ToList();

                    model2 = db.tb_Pres
                    .Include(x => x.tipospres)
                    .Include(x => x.moneda)
                    .OrderBy(x => x.idPres)
                    .Where(x => x.idAccRes.ToString() == resp && (x.idTipoPres == TipPto) && (x.idTipGas == tipgasto) && (x.idConGas == gasto) && (x.idEsp == espec) && (x.fchIniVigencia <= DateTime.Today && x.fchFinVigencia >= DateTime.Today) && (x.idEst != ConstantesGlobales.estadoInactivo) && x.Saldo != 0)
                    .ToList();

                    if(model1.Count()!= 0 || model2.Count()!= 0)
                    { 
                        model= model1.Union(model2).Distinct().ToList();
                    }
                    
                }
                else if(TipPto == ConstantesGlobales.plan_Fuera)
                {
                        model = db.tb_Pres
                        .Include(x => x.tipospres)
                       /*.Include(x => x.responsable)
                       .Include(x => x.linea)
                       .Include(x => x.zona)
                       .Include(x => x.concepto)
                       .Include(x => x.especialidad)
                       .Include(x => x.aprobador) */
                      .Include(x => x.moneda)
                     .OrderBy(x => x.idPres)
                     .Where(x => (x.idTipoPres == TipPto && x.idAccRes.ToString() == resp  && (x.fchIniVigencia <= DateTime.Today && x.fchFinVigencia >= DateTime.Today) && (x.idEst != ConstantesGlobales.estadoInactivo) && x.Saldo != 0))
                     .ToList();
                }
               

                return model;
            }
            //--
        }
        public Boolean modificarSaldo(PresupuestoModels actual, out string mensaje)
        {
            Boolean exito = false;
            var db = new ApplicationDbContext();
            PresupuestoModels anterior = db.tb_Pres.OrderBy(x => x.idPres).Where(x => x.idPres == actual.idPres).FirstOrDefault();
            var monto_Act = anterior.Monto + actual.diferencia;
            var saldo_Act = anterior.Saldo + actual.diferencia;
            var estim_Act = anterior.Estim + actual.diferencia;
            string commandText = "UPDATE tb_Pres SET Monto = @Monto , Saldo = @Saldo , Estim = @Estim ,usuMod = @usuMod , usufchMod = @usufchMod WHERE idPres = @idPres";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@idPres", actual.idPres);
                command.Parameters.AddWithValue("@Monto", monto_Act);
                command.Parameters.AddWithValue("@Saldo", saldo_Act);
                command.Parameters.AddWithValue("@Estim", estim_Act);
                command.Parameters.AddWithValue("@usuMod", actual.usuMod);
                command.Parameters.AddWithValue("@usufchMod", actual.usufchMod);
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    mensaje = "<div id='success' class='alert alert-success'>Se modifico el saldo presupuestado.</div>";
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
        public Boolean modificarSoloSaldo(PresupuestoModels actual, out string mensaje)
        {
            Boolean exito = false;
            var db = new ApplicationDbContext();
            PresupuestoModels anterior = db.tb_Pres.OrderBy(x => x.idPres).Where(x => x.idPres == actual.idPres).FirstOrDefault();
            var saldo_Act = anterior.Saldo + actual.diferencia;
            string commandText = "UPDATE tb_Pres SET  Saldo = @Saldo ,usuMod = @usuMod , usufchMod = @usufchMod WHERE idPres = @idPres";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {

                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@idPres", actual.idPres);
                command.Parameters.AddWithValue("@Saldo", saldo_Act);
                command.Parameters.AddWithValue("@usuMod", actual.usuMod);
                command.Parameters.AddWithValue("@usufchMod", actual.usufchMod);
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    mensaje = "<div id='success' class='alert alert-success'>Se modifico el saldo presupuestado.</div>";
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
        public List<PresupuestoModels> obtenerPresupuestos(List<string> id)
            {
                using (var db = new ApplicationDbContext())
                {
                    List<PresupuestoModels> model = null;
                    try
                    {
                        model = db.tb_Pres.Where(x => id.Contains(x.idPres)).OrderBy(x=>x.Saldo).ToList();
                    }
                    catch (Exception e)
                    {
                        e.Message.ToString();
                    }

                    return model;
                }
            }
        public List<PresupuestoModels> obtenerPresupuestoSimple()
        {
            using (var db = new ApplicationDbContext())
            {
                List<PresupuestoModels> model = null;
                try
                {
                    model = db.tb_Pres
                    .Include(x => x.especialidad)
                    .OrderBy(x => x.idPres).ToList();
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }

                return model;
            }
        }
        public List<PresupuestoModels> obtenerPresupuesto()
        {
            using (var db = new ApplicationDbContext())
            {
                List<PresupuestoModels> model = null;
                try
                {
                    model = db.tb_Pres
                    .Include(x => x.tipospres)
                    .Include(x => x.estado)
                    .Include(x => x.tipogasto)
                    .Include(x => x.concepto)
                    .Include(x => x.especialidad)
                    .Include(x => x.zona)
                    .Include(x => x.linea)
                    .Include(x => x.moneda)
                    .Include(x => x.responsable.empleado)
                    .Include(x => x.aprobador)
                    .Include(x => x.aprobador.empleado)
                    .Include(x => x.movimiento).DefaultIfEmpty()
                    .OrderBy(x => x.idPres).ToList();
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }

                return model;
            }
        }
        public PresupuestoModels obtenerUnPresupuesto(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                PresupuestoModels model = null;
                try
                {
                    model = db.tb_Pres.Where(x => id.Contains(x.idPres)).FirstOrDefault();
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }

                return model;
            }
        }
        public Boolean modificarSaldoEstimado(PresupuestoModels actual, out string mensaje)
        {
            Boolean exito = false;
            var db = new ApplicationDbContext();
            PresupuestoModels anterior = db.tb_Pres.OrderBy(x => x.idPres).Where(x => x.idPres == actual.idPres).FirstOrDefault();
            var estim_Act = anterior.Estim + actual.diferencia;
            string commandText = "UPDATE tb_Pres SET  Estim = @Estim ,usuMod = @usuMod , usufchMod = @usufchMod WHERE idPres = @idPres";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {

                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@idPres", actual.idPres);
                command.Parameters.AddWithValue("@Estim", estim_Act);
                command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                command.Parameters.AddWithValue("@usufchMod", DateTime.Now);
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    mensaje = "<div id='success' class='alert alert-success'>Se modifico el saldo presupuestado.</div>";
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
    }
}