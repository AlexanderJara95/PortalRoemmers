using PortalRoemmers.Areas.Ventas.Models.SolicitudGasto;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Ventas.Services.SolicitudGasto
{
    public class LiquidaRepositorio
    {
        public Boolean mergeLiquidacion(LiquidaGastoModels model)
        {
            Boolean exito = true;
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    if (db.tb_LiqGas.Where(x => x.idSolGas== model.idSolGas).Count() != 0)
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
    }
}