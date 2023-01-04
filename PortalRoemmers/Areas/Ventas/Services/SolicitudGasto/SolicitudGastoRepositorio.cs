using PortalRoemmers.Areas.Ventas.Models.SolicitudGasto;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; //permite usar landa
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Security;
using System.Data.SqlClient;
using System.Data;
using PortalRoemmers.Areas.Marketing.Models.SolicitudGastoMkt;

namespace PortalRoemmers.Areas.Ventas.Services.SolicitudGasto
{
    public class SolicitudGastoRepositorio
    {
        Ennumerador enu = new Ennumerador();
        UsuarioRepositorio _usu = new UsuarioRepositorio();
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search,string modulo, string primero, string actual)
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

                var model = db.tb_SolGastos
                    .Include(x => x.estado)
                    .Include(x => x.concepto.tipoGasto)
                    .Include(x => x.dFirma.Select(y => y.gasto))
                    .Include(x => x.dFirma.Select(y => y.estado))
                    .Include(x => x.dFirma.Select(y => y.solicitante))
                    .Include(x => x.moneda)
                    .Include(x => x.responsable.empleado)
                    .Include(x => x.concepto)
                    .Include(x => x.zona).Include(x => x.linea)
                    .Include(x => x.especialidad)
                    .Include(x => x.dFam.Select(y => y.familia))
                    .Include(x => x.dMed.Select(y => y.cliente))
                    .Include(x => x.dResp.Select(y => y.responsable))
                    .Include(x => x.dFil)
                    .Include(x => x.liquidacion).DefaultIfEmpty()
                    .Include(x => x.liquidacion.estado).DefaultIfEmpty()
                    .OrderByDescending(x => x.idSolGas).Where(x => ((x.idAccRes == SessionPersister.UserId || x.idAccSol == SessionPersister.UserId)  && x.idEst != ConstantesGlobales.estadoAnulado && x.modSolGas == modulo) && ((x.usufchCrea >= p) && (x.usufchCrea <= a)) && (x.titSolGas.Contains(search) || (x.obsSolGas.Contains(search))  || x.idSolGas.Contains(search) || x.estado.nomEst.Contains(search)))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.tb_SolGastos.Where(x => ((x.idAccRes == SessionPersister.UserId || x.idAccSol == SessionPersister.UserId)  && x.idEst != ConstantesGlobales.estadoAnulado && x.modSolGas == modulo ) && ((x.usufchCrea >= p) && (x.usufchCrea <= a)) && (x.titSolGas.Contains(search) || (x.obsSolGas.Contains(search)) || x.idSolGas.Contains(search) || x.estado.nomEst.Contains(search)) ).Count();
                var modelo = new ViewModels.IndexViewModel();
                modelo.SolGasto = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public Boolean crear(SolicitudGastoModels model)
        {
            Boolean mensaje = false;
            //creo su ID
            string tabla = "tb_SolGastos";
            int idc = int.Parse(model.idSolGas.TrimStart('0'));
            using (var db = new ApplicationDbContext()) {
                model.idNapro = _usu.obtenerNivelAproUsu(model.idAccRes);

                db.tb_SolGastos.Add(model);
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
            }
            return mensaje;
        }
        public SolicitudGastoModels obtenerItem(string id)
        {
            using(var db = new ApplicationDbContext()){
                SolicitudGastoModels model = db.tb_SolGastos
                                            .Include(x => x.movimiento)
                                            .Include(x => x.movimiento.Select(z => z.presupuesto))
                                            .Include(x => x.movimiento.Select(z => z.presupuesto.moneda))
                                            .Include(x => x.movimiento.Select(z => z.presupuesto.especialidad))
                                            .Include(x => x.movimiento.Select(z => z.presupuesto.tipospres))
                                            .Include(x => x.movimiento.Select(z => z.presupuesto.tipogasto))
                                            .Include(x => x.movimiento.Select(z => z.presupuesto.concepto))
                                            .Include(x => x.solicitud).Include(x => x.pago)
                                            .Include(x => x.estado)
                                            .Include(x => x.responsable.empleado)
                                            .Include(x => x.solicitante.empleado)
                                            .Include(x => x.aprobador.empleado)
                                            .Include(x => x.dFirma)
                                            .Include(x => x.dFirma.Select(z=>z.estado))
                                            .Include(x => x.concepto.tipoGasto)
                                            .Include(x => x.concepto)
                                            .Include(x => x.zona)
                                            .Include(x => x.linea)
                                            .Include(x => x.especialidad)
                                            .Include(x => x.moneda)
                                            .Include(y => y.dResp.Select(a=>a.responsable))
                                            .Include(z => z.dMed.Select(a=>a.cliente))
                                            .Include(y => y.dFam.Select(a => a.familia))
                                            .Include(m => m.dGas.Select(n=>n.actividad).Select(o=>o.estimacion).Select(u=>u.moneda))
                                            .Include(j => j.dGas.Select(k=>k.gasto).Select(g=>g.tipoGastoActividad))
                                            .Include(k => k.dAre.Select(o=>o.areaTerap))
                                            .Where(x => x.idSolGas == id).FirstOrDefault();
                return model;
            }
            
        }
        public Boolean modificar(SolicitudGastoModels model)
        {
            Boolean ok = false;
            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    ok = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            return ok;
        }
        public Boolean crearDetalleF(List<DetSolGasto_FamModels> model)
        {
            if (model.Count() != 0)
            {
                using (var db = new ApplicationDbContext())
                {
                    db.tb_DetSolGas_Fam.AddRange(model);
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
            else
            {
                return true;
            }
        }
        public Boolean crearDetalleM(List<DetSolGasto_MedModels> model)
        {
            if (model.Count() != 0)
            {
                using (var db = new ApplicationDbContext())
                {
                    db.tb_DetSolGas_Med.AddRange(model);
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
            else
            {
                return true;
            }
        }
        public Boolean crearDetalleR(List<DetSolGasto_RespModels> model)
        {
            if (model.Count() != 0)
            {
                    using (var db = new ApplicationDbContext())
                {
                    db.tb_DetSolGas_Res.AddRange(model);
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
            else
            {
                return true;
            }
        }
        public Boolean crearDetalleD(List<DetSolGasto_DocModels> model)
        {
            if (model.Count() != 0)
            {
                using (var db = new ApplicationDbContext())
                {
                    db.tb_DetSolGas_Doc.AddRange(model);
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
            else
            {
                return true;
            }
        }
        public Boolean crearDetalleG(List<DetSolGasto_GasModels> model)
        {
            if (model.Count() != 0)
            {
                using (var db = new ApplicationDbContext())
                {
                    db.tb_DetSolGas_Gas.AddRange(model);
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
            else
            {
                return true;
            }
        }
        public Boolean crearDetalleA(List<DetSolGasto_AreaTerapModels> model)
        {
            if (model.Count() != 0)
            {
                using (var db = new ApplicationDbContext())
                {
                    db.tb_DetSolGas_ATer.AddRange(model);
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
            else
            {
                return true;
            }
        }
        public Boolean mergeFirmas(FirmasSoliGastoModels model)
        {
            Boolean exito = true;
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    if (db.tb_FirSolGas.Where(x => x.idSolGas == model.idSolGas && x.idAcc==model.idAcc &&x.idEst==model.idEst).Count() != 0)
                    {
                        db.Entry(model).State = EntityState.Modified;
                    }
                    else
                    {
                        db.Entry(model).State = EntityState.Added;
                    }

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    exito = false;
                    e.Message.ToString();
                }
            }
            return exito;
        }
        public Boolean updateEstadoSol(string sol, string apr,string est)
        {
           

            string commandText = "UPDATE tb_SolGastos SET idEst = @idEst, idNapro=@idNapro, idAccApro=@idAccApro, usuMod=@usuMod , usufchMod=@usufchMod   WHERE idSolGas = @idSolGas ;";

            if (apr=="")
            {
                commandText = "UPDATE tb_SolGastos SET idEst = @idEst, usuMod=@usuMod , usufchMod=@usufchMod   WHERE idSolGas = @idSolGas ;";
            }
 
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                command.Parameters.Add("@idSolGas", SqlDbType.VarChar);
                command.Parameters["@idSolGas"].Value = sol;

                if (apr == "")
                {
                    command.Parameters.AddWithValue("@idEst", est);
                    command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                    command.Parameters.AddWithValue("@usufchMod", DateTime.Now);
                }
                else
                {
                    command.Parameters.AddWithValue("@idEst", est);
                    command.Parameters.AddWithValue("@idNapro", SessionPersister.NivApr);
                    command.Parameters.AddWithValue("@idAccApro", apr);
                    command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                    command.Parameters.AddWithValue("@usufchMod", DateTime.Now);
                }

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
        public List<SolicitudGastoModels> obtenerSolicitudesVarios(string[] sols)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_SolGastos
                    .Include(x => x.movimiento.Select(y => y.presupuesto.tipospres))
                    .Include(x => x.dFirma.Select(y => y.estado))
                    .Include(x => x.dMed.Select(y => y.cliente))
                    .Include(x => x.dGas.Select(y =>y.actividad ))
                    .Include(x => x.dGas.Select(y => y.gasto.tipoGastoActividad))
                    .Include(x => x.dFam.Select(y=>y.familia))
                    .Include(x => x.dResp.Select(y => y.responsable))
                    .Include(x => x.linea).Include(x => x.zona)
                    .Include(x => x.moneda)
                    .Include(x => x.pago)
                    .Include(x => x.solicitud)
                    .Include(x => x.especialidad)
                    .Include(x => x.concepto)
                    .Include(x => x.concepto.tipoGasto)
                    .Include(x => x.solicitante.empleado)
                    .Include(k => k.dAre.Select(o => o.areaTerap))
                    .Where(x => sols.Contains(x.idSolGas)).ToList();
                return model;
            }
        }
        public List<SolicitudGastoModels> obtenerSolicitudesGeneral(string mod,string solicitante,string estado,string inicio, string fin)
        {//esta lento esta consulta deberia tener where para q no liste todos
            using (var db = new ApplicationDbContext())
            {
                DateTime i = DateTime.Parse(inicio); //desde
                DateTime f = DateTime.Parse(fin).AddHours(23).AddMinutes(59);//hasta

                var model = db.tb_SolGastos.Include(x => x.solicitud).Include(x => x.pago).Include(x => x.solicitante.empleado).Include(x => x.estado).Include(x => x.dFirma).Include(x => x.dFirma.Select(y => y.gasto)).Include(x => x.dFirma.Select(y => y.estado)).Include(x => x.dFirma.Select(y => y.solicitante)).Include(x => x.moneda)
                    .Include(x => x.responsable.empleado).Include(x => x.concepto).Include(x => x.concepto.tipoGasto).Include(x => x.zona).Include(x => x.linea).Include(x => x.especialidad)
                    .Include(x => x.dFam.Select(y => y.familia)).Include(x => x.dMed.Select(y => y.cliente)).Include(x => x.dResp.Select(y => y.responsable)).Include(x => x.dGas.Select(y => y.gasto))
                    .Where(x => (x.modSolGas == mod) && (x.idAccRes == (solicitante == "" ? x.idAccRes : solicitante)) && (x.idEst == (estado == "" ? x.idEst : estado)) && ((x.usufchCrea >= i) && (x.usufchCrea <= f)))
                    .ToList();

                return model;
            }
        }
        public List<SolicitudGastoModels> obtenerSolicitudesSeguimiento(string idAcc, string primero, string actual)
        {//esta lento esta consulta deberia tener where para q no liste todos
            using (var db = new ApplicationDbContext())
            {
                //************************************************************************
                DateTime p = DateTime.Parse(primero); //desde
                DateTime a = DateTime.Parse(actual).AddHours(23).AddMinutes(59);//hasta
                //************************************************************************
                var model = db.tb_SolGastos
                    .Include(x => x.solicitud)
                    .Include(x => x.pago)
                    .Include(x => x.solicitante.empleado)
                    .Include(x => x.estado)
                    .Include(x => x.dFirma)
                    .Include(x => x.dFirma.Select(y => y.gasto))
                    .Include(x => x.dFirma.Select(y => y.estado))
                    .Include(x => x.dFirma.Select(y => y.solicitante))
                    .Include(x => x.moneda)
                    .Include(x => x.responsable.empleado)
                    .Include(x => x.concepto)
                    .Include(x => x.concepto.tipoGasto)
                    .Include(x => x.zona)
                    .Include(x => x.linea)
                    .Include(x => x.especialidad)
                    .Include(x => x.dFam.Select(y => y.familia))
                    .Include(x => x.dMed.Select(y => y.cliente))
                    .Include(x => x.dResp.Select(y => y.responsable))
                    .Include(x => x.dGas.Select(y => y.gasto))
                    //.Where(x => x.dFirma.Select(y => y.idAcc).Contains(idAcc))
                    .Where(x => ((x.dFirma.Where(y=>y.idAcc==idAcc).Select(z => z.usufchCrea).FirstOrDefault() >= p) && (x.dFirma.Where(y=>y.idAcc==idAcc).Select(z => z.usufchCrea).FirstOrDefault() <= a))).OrderBy(x=>x.dFirma.Where(y => y.idAcc == idAcc).Select(z => z.usufchCrea).FirstOrDefault()).ToList();

                return model;
            }
        }
        public List<SolicitudGastoModels> obtenerSolicitudesContabilizar(string idTipPag,string idEst, string primero,string actual)
        {
            using (var db = new ApplicationDbContext())
            {
                
                DateTime p = DateTime.Parse(primero); //desde
                DateTime a = DateTime.Parse(actual).AddHours(23).AddMinutes(59);//hasta

                List<SolicitudGastoModels> model = new List<SolicitudGastoModels>();

                if (idEst == ConstantesGlobales.estadoAtendido || idEst == ConstantesGlobales.estadoNoAtendido || idEst == ConstantesGlobales.estadoEnGiro)
                {
                    model = db.tb_SolGastos
                        .Include(x => x.solicitud)
                        .Include(x => x.pago).Include(x => x.solicitante.empleado)
                        .Include(x => x.estado).Include(x => x.dFirma)
                        .Include(x => x.dFirma.Select(y => y.gasto))
                        .Include(x => x.dFirma.Select(y => y.estado))
                        .Include(x => x.dFirma.Select(y => y.solicitante))
                        .Include(x => x.moneda).Include(x => x.responsable.empleado)
                        .Include(x => x.concepto).Include(x => x.concepto.tipoGasto)
                        .Include(x => x.zona).Include(x => x.linea)
                        .Include(x => x.especialidad)
                        .Include(x => x.dFam.Select(y => y.familia))
                        .Include(x => x.dMed.Select(y => y.cliente))
                        .Include(x => x.dResp.Select(y => y.responsable))
                        .Include(x => x.dGas.Select(y => y.gasto))
                        .OrderBy(x => x.idSolGas)
                        .Where(x => (x.idTipPag == idTipPag) && (x.idEst == idEst) && ((x.dFirma.Where(y => y.idEst == idEst).Select(y => y.usufchCrea).FirstOrDefault() >= p) && ((x.dFirma.Where(y => y.idEst == idEst).Select(y => y.usufchCrea).FirstOrDefault() <= a)))).ToList();
                }
                else
                {
                    model = db.tb_SolGastos
                        .Include(x => x.solicitud)
                        .Include(x => x.pago)
                        .Include(x => x.solicitante.empleado)
                        .Include(x => x.estado)
                        .Include(x => x.dFirma)
                        .Include(x => x.dFirma.Select(y => y.gasto))
                        .Include(x => x.dFirma.Select(y => y.estado))
                        .Include(x => x.dFirma.Select(y => y.solicitante))
                        .Include(x => x.moneda)
                        .Include(x => x.responsable.empleado)
                        .Include(x => x.concepto)
                        .Include(x => x.concepto.tipoGasto)
                        .Include(x => x.zona)
                        .Include(x => x.linea)
                        .Include(x => x.especialidad)
                        .Include(x => x.dFam.Select(y => y.familia))
                        .Include(x => x.dMed.Select(y => y.cliente))
                        .Include(x => x.dResp.Select(y => y.responsable))
                        .Include(x => x.dGas.Select(y => y.gasto))
                        .OrderBy(x => x.idSolGas)
                     .Where(x => (x.idTipPag == idTipPag) && (x.idEst == idEst) && ((x.usufchCrea >= p) && (x.usufchCrea <= a))).ToList();
                }
               
                return model;
            }
        }
        public List<SolicitudGastoModels> obtenerSolicitudesLiquidar( string idEst, string primero, string actual)
        {
            using (var db = new ApplicationDbContext())
            {

                DateTime p = DateTime.Parse(primero); //desde
                DateTime a = DateTime.Parse(actual).AddHours(23).AddMinutes(59);//hasta

                var model = db.tb_SolGastos.Include(x => x.solicitud).Include(x => x.pago).Include(x => x.solicitante.empleado).Include(x => x.estado).Include(x => x.dFirma).Include(x => x.dFirma.Select(y => y.gasto)).Include(x => x.dFirma.Select(y => y.estado)).Include(x => x.dFirma.Select(y => y.solicitante)).Include(x => x.moneda).Include(x => x.responsable.empleado).Include(x => x.concepto).Include(x => x.concepto.tipoGasto).Include(x => x.zona).Include(x => x.linea).Include(x => x.especialidad).Include(x => x.dFam.Select(y => y.familia)).Include(x => x.dMed.Select(y => y.cliente)).Include(x => x.dResp.Select(y => y.responsable)).Include(x => x.dGas.Select(y => y.gasto)).OrderBy(x => x.idSolGas)
                .Where(x => (x.idEst == idEst) && ((x.usufchCrea >= p) && (x.usufchCrea <= a))).ToList();

                return model;
            }
        }
        public List<SolicitudGastoModels> obtenerSolicitudesAprobar(string id,List<string> estados,string idTipPag,  string idEst, string primero, string actual)
        {
            using (var db = new ApplicationDbContext())
            {
                DateTime p = DateTime.Parse(primero); //desde
                DateTime a = DateTime.Parse(actual).AddHours(23).AddMinutes(59);//hasta
                List<SolicitudGastoModels> model = new List<SolicitudGastoModels>();

                if (idEst == ConstantesGlobales.estadoAprobado || idEst == ConstantesGlobales.estadoRechazado)
                {
                    model = db.tb_SolGastos
                    .Include(x => x.solicitud)
                    .Include(x => x.pago)
                    .Include(x => x.solicitante.empleado)
                    .Include(x => x.estado)
                    .Include(x => x.dFirma)
                    .Include(x => x.dFirma.Select(y => y.gasto))
                    .Include(x => x.dFirma.Select(y => y.estado))
                    .Include(x => x.dFirma.Select(y => y.solicitante))
                    .Include(x => x.moneda)
                    .Include(x => x.responsable.empleado)
                    .Include(x => x.concepto)
                    .Include(x => x.concepto.tipoGasto)
                    .Include(x => x.zona)
                    .Include(x => x.linea)
                    .Include(x => x.especialidad)
                    .Include(x => x.dFam.Select(y => y.familia))
                    .Include(x => x.dMed.Select(y => y.cliente))
                    .Include(x => x.dResp.Select(y => y.responsable))
                    .Include(x => x.dGas.Select(y => y.gasto))
                    .Include(x => x.dAre.Select(y => y.areaTerap))
                    .OrderBy(x => x.idSolGas)
                    //.Where(x => x.idAccApro == id && (!estados.Contains(x.idEst)) && x.idTipPag == idTipPag).ToList();
                    .Where(x => (x.idTipPag == idTipPag) && ((x.dFirma.Where(y => (y.idAcc == id) && (y.idEst==idEst)).Select(z => z.usufchCrea).FirstOrDefault() >= p) && ((x.dFirma.Where(y => (y.idAcc == id) && (y.idEst==idEst)).Select(z => z.usufchCrea).FirstOrDefault() <= a)))).ToList();
                }
                else
                {
                    model = db.tb_SolGastos
                    .Include(x => x.solicitud)
                    .Include(x => x.pago)
                    .Include(x => x.solicitante.empleado)
                    .Include(x => x.estado)
                    .Include(x => x.dFirma)
                    .Include(x => x.dFirma.Select(y => y.gasto))
                    .Include(x => x.dFirma.Select(y => y.estado))
                    .Include(x => x.dFirma.Select(y => y.solicitante))
                    .Include(x => x.moneda)
                    .Include(x => x.responsable.empleado)
                    .Include(x => x.concepto)
                    .Include(x => x.concepto.tipoGasto)
                    .Include(x => x.zona)
                    .Include(x => x.linea)
                    .Include(x => x.especialidad)
                    .Include(x => x.dFam.Select(y => y.familia))
                    .Include(x => x.dMed.Select(y => y.cliente))
                    .Include(x => x.dResp.Select(y => y.responsable))
                    .Include(x => x.dGas.Select(y => y.gasto))
                    .Include(x => x.dAre.Select(y => y.areaTerap))
                    .OrderBy(x => x.idSolGas)
                    .Where(x => x.idAccApro == id && (!estados.Contains(x.idEst)) && x.idTipPag == idTipPag).ToList();
                }

                return model;
            }
        }
        public List<SolicitudGastoModels> obtenerSolicitudesAprobarxAdm(string idTipPag, string id ,string idEst, string primero, string actual)
        {
            using (var db = new ApplicationDbContext())
            {
                DateTime p = DateTime.Parse(primero); //desde
                DateTime a = DateTime.Parse(actual).AddHours(23).AddMinutes(59);//hasta

                List<SolicitudGastoModels> model = new List<SolicitudGastoModels>();

                model = db.tb_SolGastos.Include(x => x.solicitud).Include(x => x.pago).Include(x => x.solicitante.empleado).Include(x=>x.aprobador.empleado).Include(x => x.estado).Include(x => x.dFirma).Include(x => x.dFirma.Select(y => y.gasto)).Include(x => x.dFirma.Select(y => y.estado)).Include(x => x.dFirma.Select(y => y.solicitante)).Include(x => x.moneda).Include(x => x.responsable.empleado).Include(x => x.concepto).Include(x => x.concepto.tipoGasto).Include(x => x.zona).Include(x => x.linea).Include(x => x.especialidad).Include(x => x.dFam.Select(y => y.familia)).Include(x => x.dMed.Select(y => y.cliente)).Include(x => x.dResp.Select(y => y.responsable)).Include(x => x.dGas.Select(y => y.gasto)).OrderBy(x => x.idSolGas)
                     .Where(x => x.idAccApro == id && (x.idTipPag == idTipPag) && (x.idEst == idEst) && ((x.usufchCrea >= p) && (x.usufchCrea <= a))).ToList();

                return model;
            }
        }
        public List<SolicitudGastoModels> obtenerSolicitudesaExportar(string search, string modulo)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_SolGastos.Include(x => x.solicitud).Include(x => x.estado).Include(x => x.moneda).Include(x => x.responsable.empleado).Include(x => x.concepto).Include(x => x.concepto.tipoGasto).Include(x => x.zona).Include(x => x.linea).Include(x => x.especialidad)
                    .OrderByDescending(x => x.idSolGas).Where(x => ((x.idAccRes == SessionPersister.UserId || x.idAccSol == SessionPersister.UserId) && x.idEst != ConstantesGlobales.estadoAnulado && x.modSolGas == modulo) && (x.titSolGas.Contains(search) || (x.obsSolGas.Contains(search)) /*|| x.fchEveSolGas.Contains(search) */ || x.idSolGas.Contains(search) || x.estado.nomEst.Contains(search)))
                    .ToList();

                return model;
            }
        }
        public List<DetSolGasto_GasModels> obtenerDetSolGastosxEst()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_DetSolGas_Gas.Include(x => x.solicitud).Include(x => x.gasto).Include(x => x.solicitud.estado).Include(x => x.solicitud.moneda).OrderBy(x => x.idSolGas).ToList();
                return model;
            }
        }
        public List<DetSolGasto_GasModels> obtenerDetSolGastos(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_DetSolGas_Gas.Include(x=>x.solicitud).Include(x=>x.gasto).Include(x=>x.solicitud.estado).Where(x=>x.idSolGas== id).OrderBy(x => x.idSolGas).ToList();
                return model;
            }
        }
        public List<DetSolGasto_DocModels> obtenerDetDocumentos()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_DetSolGas_Doc.Include(x=>x.solicitud).Where(x=>x.solicitud.idEst!=ConstantesGlobales.estadoAnulado).ToList();
                return model;
            }
        }
        public Boolean crearDetalleFile(DetSolGasto_FileModels model)
        {
            Boolean mensaje = false;
            //creo su ID
            string tabla = "tb_DetSolGas_File";
            int idc = enu.buscarTabla(tabla);
            model.idFile = idc;
            //----------
            //Crear en la BD
            using (var db = new ApplicationDbContext())
            {
                db.tb_DetSolGas_File.Add(model);
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
            }
            return mensaje;
        }
        public List<DetSolGasto_FileModels> obtenerDetSolFiles()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_DetSolGas_File.Include(x => x.solicitud).OrderBy(x => x.idFile).ToList();
                return model;
            }
        }
        public Boolean eliminarDetalleFile(string idSol,int id)
        {   
            //--------------------------------------------------------------------
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_DetSolGas_File.Include(x => x.solicitud).Where(x => x.idSolGas == idSol && x.idFile == id).ToList();
                db.tb_DetSolGas_File.RemoveRange(model);
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
    }
}