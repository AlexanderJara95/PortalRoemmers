using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Helpers
{
    public class Ennumerador
    {
        public int buscarTabla(string nom)
        {
            int valor = 0;
             
            using (var db = new ApplicationDbContext())
            {
                var result = (from p in db.tb_IdTablas
                              where p.idTabla == nom
                              select p ?? null);
                try
                {
                    if (result.Count() == 0)
                    {
                        CodigoModels tb = new CodigoModels();
                        tb.idTabla = nom;
                        tb.nrotabla = 0;
                        db.tb_IdTablas.Add(tb);
                        db.SaveChanges();
                        valor = 0;
                    }
                    else
                    {
                        valor = result.Select(x => x.nrotabla).SingleOrDefault();
                    }
                }
                catch (Exception e)
                {
                    e.Message.ToString(); 
                }
            }

            return valor + 1;
        }

        public void actualizarTabla(string tb, int nro)
        {
            using (var db = new ApplicationDbContext())
            {
                var update = (from p in db.tb_IdTablas
                              where p.idTabla == tb
                              select p).SingleOrDefault();
                if (update != null)
                {
                    update.nrotabla = nro;
                    db.SaveChanges();
                }
            }
        }

    }
}