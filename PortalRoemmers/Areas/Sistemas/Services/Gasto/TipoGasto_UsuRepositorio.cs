using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System;
namespace PortalRoemmers.Areas.Sistemas.Services.Gasto
{
    public class TipoGasto_UsuRepositorio
    {
        public Boolean crear(List<TipGas_Usu_Models> model)
        {
            Boolean resultado = true;
            var db = new ApplicationDbContext();

            db.tb_TipGas_Usu.AddRange(model);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                resultado = false;
            }
            return resultado;
        }
        public Boolean eliminar(string[] asignados, string id)
        {

            bool respuesta = true;

            using (var db = new ApplicationDbContext())
            {
                List<TipGas_Usu_Models> tipgasUsu = (from p in db.tb_TipGas_Usu
                                                     where asignados.Contains(p.idAcc) && p.idTipGas == id
                                               select p).ToList();
                db.tb_TipGas_Usu.RemoveRange(tipgasUsu);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }

        public List<TipGas_Usu_Models> obtenerUsuarioxTipoDeGasto(string idTipGas)
        {
            var model = new List<TipGas_Usu_Models>();
            using (var db = new ApplicationDbContext())
            {
                model = db.tb_TipGas_Usu.Include(x=>x.cuentaDeUsuario.empleado).Where(x => x.idTipGas == idTipGas).ToList();
            }
            return model;
        }

        public List<TipGas_Usu_Models> obtenerTipoDeGastoXusuario(string idAccRes)
        {
            var model = new List<TipGas_Usu_Models>();
            using (var db = new ApplicationDbContext())
            {
                model = db.tb_TipGas_Usu.Include(x => x.tiposDeGastos).Where(x => x.idAcc == idAccRes).ToList();
            }
            return model;

        }
    }
}