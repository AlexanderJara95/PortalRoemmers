using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Entity;

namespace PortalRoemmers.Areas.Sistemas.Services.Presupuesto
{
    public class MovimientoPresRepositorio
    {
        private PresupuestoRepositorio _pre;
        public MovimientoPresRepositorio()
        {
            _pre = new PresupuestoRepositorio();
        }
        public Boolean crear(MovimientoPresModels model)
        {
            Boolean exec = true;
            using (var db = new ApplicationDbContext())
            {
                db.tb_MovPres.Add(model);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    exec = false;
                }
            }
            return exec;
        }

        public Boolean updateSaldoPres(string idPres,double? saldo)
        {
            string commandText = "UPDATE tb_Pres SET  Saldo=@Saldo, usuMod=@usuMod , usufchMod=@usufchMod   WHERE idPres = @idPres ;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idPres", SqlDbType.VarChar);
                command.Parameters["@idPres"].Value = idPres;

                command.Parameters.AddWithValue("@Saldo", saldo);
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
        public Boolean updateSaldoPresEstim(string idPres, double? saldoEstim)
        {
            string commandText = "UPDATE tb_Pres SET  Estim=@Estim, usuMod=@usuMod , usufchMod=@usufchMod   WHERE idPres = @idPres ;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idPres", SqlDbType.VarChar);
                command.Parameters["@idPres"].Value = idPres;

                command.Parameters.AddWithValue("@Estim", saldoEstim);
                command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                command.Parameters.AddWithValue("@usufchMod", DateTime.Today);

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
        public Boolean updateEstMovPres(string sol, string est)
        {
            string commandText = "UPDATE tb_MovPres SET  idEst=@idEst, usuMod=@usuMod , usufchMod=@usufchMod   WHERE idSolGas = @idSolGas ;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idSolGas", SqlDbType.VarChar);
                command.Parameters["@idSolGas"].Value = sol;

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

        public Boolean updateMovPres(string sol,string pre, string est,double mov)
        {
            string commandText = "UPDATE tb_MovPres SET  idEst=@idEst, monSolGas=@monSolGas , usuMod=@usuMod , usufchMod=@usufchMod   WHERE idSolGas = @idSolGas and idPres=@idPres ;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idSolGas", SqlDbType.VarChar);
                command.Parameters["@idSolGas"].Value = sol;
                command.Parameters.Add("@idPres", SqlDbType.VarChar);
                command.Parameters["@idPres"].Value = pre;

                command.Parameters.AddWithValue("@idEst", est);
                command.Parameters.AddWithValue("@monSolGas", mov);
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

        public List<MovimientoPresModels> obtenerMovimientos(string idSolGas)
        {
            using (var db = new ApplicationDbContext())
            {
                List<MovimientoPresModels> model = null;
                try { 
                    model = db.tb_MovPres.Include(x => x.presupuesto).OrderByDescending(x=>x.presupuesto.Saldo).Where(x => x.idSolGas== idSolGas).ToList();
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }

                return model;
            }
        }
        public List<MovimientoPresModels> obtenerMovxPres(string id)
        {
            var db = new ApplicationDbContext();
     
                List<MovimientoPresModels> model = null;
                try
                {
                    model = db.tb_MovPres
                    .Include(x => x.estado)
                    .Include(x => x.solGasto)
                    .Include(x => x.solGasto.concepto)
                    .Include(x => x.solGasto.concepto.tipoGasto)
                    .Include(x => x.solGasto.solicitante.empleado)
                    .Include(x => x.solGasto.estado)
                    .Include(x => x.solGasto.moneda)
                    .Include(x => x.solGasto.dFam).Include(x => x.presupuesto)
                    .Include(x => x.solGasto.liquidacion)
                    .Where(x => x.idPres == id && (x.idEst!=ConstantesGlobales.estadoAnulado  && x.idEst != ConstantesGlobales.estadoNoAtendido && x.idEst != ConstantesGlobales.estadoRechazado))
                    .ToList();
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
                return model;
        }
        public Boolean modDetMovPres(List<MovimientoPresModels> listaActual, string idSolGas)
        {
            string mensaje = "";
            //////////---------------------//////////
            using (var db = new ApplicationDbContext())
            {
                //////////---------------------//////////
                var listaAnterior = db.tb_MovPres.Where(x => x.idSolGas == idSolGas).ToList();
                /////////----------------------//////////
                var listaIguales = listaActual.Where(x => listaAnterior.Any(y => y.idPres == x.idPres)).ToList();
                //////////---------------------//////////
                var listaDeEliminados = listaAnterior.Where(x => !listaActual.Any(y => y.idPres == x.idPres)).ToList();
                //////////---------------------//////////
                var listaDeAgregados = listaActual.Where(y => !listaAnterior.Any(x => x.idPres == y.idPres)).ToList();
                //////////---------------------//////////
                if (listaIguales.Count != 0)
                {
                    string msm = "";
                    PresupuestoModels objPI = new PresupuestoModels();
                    foreach (var y in listaIguales)
                    {
                        //UPDATE en movimientos con igual presupuesto
                        bool verdad = updateMovPres(y.idSolGas,y.idPres,y.idEst,y.monSolGas);
                        //****
                        if(verdad)
                        { 
                            var z = listaAnterior.Where(x => x.idPres == y.idPres).SingleOrDefault();
                            var ppt = _pre.obtenerUnPresupuesto(y.idPres);
                            double imp = z.monSolGas;
                            string mon1 = ppt.idMon;
                            string mon2 = z.idMon;
                            double dif = 0;
                            if (mon1 != mon2)
                            {
                                //presupuesto en soles con sol. moneda en dolares
                                if (mon1 == ConstantesGlobales.monedaDol)
                                {
                                    dif = Math.Round(z.monSolGas / z.valtipCam, 2);
                                }
                                //presupuesto en dolares con sol. moneda en soles
                                if (mon1 == ConstantesGlobales.monedaSol)
                                {
                                    dif = Math.Round(z.monSolGas * z.valtipCam, 2);
                                }
                            }
                            else
                            {
                                dif = imp;
                            }
                            //****
                            objPI.idPres = y.idPres;
                            objPI.diferencia = dif - y.diferencia;
                            objPI.usuMod = y.usuMod;
                            objPI.usufchMod = y.usufchMod;
                            _pre.modificarSoloSaldo(objPI, out msm);
                            objPI = new PresupuestoModels();
                        }
                    }
                }
                if (listaDeEliminados.Count != 0)
                {
                    db.tb_MovPres.RemoveRange(listaDeEliminados);
                    //Actualizacion de Presupuesto**********************
                    string msm = "";
                    PresupuestoModels objPE = new PresupuestoModels();
                    foreach (var z in listaDeEliminados)
                    {
                        //*****
                        var t = listaAnterior.Where(x => x.idPres == z.idPres).SingleOrDefault();
                        var ppt = _pre.obtenerUnPresupuesto(t.idPres);
                        double imp = t.monSolGas;
                        string mon1 = ppt.idMon;
                        string mon2 = t.idMon;
                        double dif = 0;
                        if (mon1 != mon2)
                        {
                            //presupuesto en soles con sol. moneda en dolares
                            if (mon1 == ConstantesGlobales.monedaDol)
                            {
                                dif = Math.Round(t.monSolGas / t.valtipCam, 2);
                            }
                            //presupuesto en dolares con sol. moneda en soles
                            if (mon1 == ConstantesGlobales.monedaSol)
                            {
                                dif = Math.Round(t.monSolGas * t.valtipCam, 2);
                            }
                        }
                        else
                        {
                            dif = imp;
                        }
                        //****
                        objPE.idPres = z.idPres;
                        objPE.diferencia = dif;
                        objPE.usuMod = z.usuMod;
                        objPE.usufchMod = z.usufchMod;
                        _pre.modificarSoloSaldo(objPE, out msm);
                        objPE = new PresupuestoModels();
                    }
                    //**************************************************
                }
                if (listaDeAgregados.Count != 0)
                {
                    db.tb_MovPres.AddRange(listaDeAgregados);
                    //Actualizacion de Presupuesto**********************
                    string msm = "";
                    PresupuestoModels objPA = new PresupuestoModels();
                    foreach (var z in listaDeAgregados)
                    {
                        objPA.idPres = z.idPres;
                        objPA.diferencia = z.diferencia * (-1);
                        objPA.usuMod = z.usuMod;
                        objPA.usufchMod = z.usufchMod;
                        _pre.modificarSoloSaldo(objPA, out msm);
                        objPA = new PresupuestoModels();
                    }
                    //**************************************************
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
    }
}