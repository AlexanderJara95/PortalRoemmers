using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PortalRoemmers.Areas.Contabilidad.Models.Letra;
using PortalRoemmers.Models;

namespace PortalRoemmers.Areas.Contabilidad.Services.Letra
{
    public class FirLetService
    {
        public Boolean mergeFirmas(FirmasLetraModels model)
        {
            Boolean exito = true;
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    if (db.tb_FirLet.Where(x => x.idLetra == model.idLetra && x.idAcc == model.idAcc && x.idEst == model.idEst).Count() != 0)
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