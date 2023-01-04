using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace PortalRoemmers.Areas.Sistemas.Services.Visitador
{
    public class Pro_Lin_Repositorio
    {
        public Boolean crear(List<Pro_LIn_Models> model)
        {
            Boolean resultado = true;
            var db = new ApplicationDbContext();

            db.tb_Pro_Lin.AddRange(model);
            try
            {
                db.SaveChanges();

            }
            catch (Exception e)
            {
                resultado =false;
            }
            return resultado;
        }
        public Boolean eliminar(string[] proAc, string lin)
        {
   
            bool respuesta = true;

            using (var db = new ApplicationDbContext())
            {
                List<Pro_LIn_Models> proLin = (from p in db.tb_Pro_Lin
                                               where proAc.Contains(p.idProAX) && p.idLin == lin select p).ToList();


                db.tb_Pro_Lin.RemoveRange(proLin);
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
        public List<Pro_LIn_Models> obtenerProLinID(string id)
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Pro_Lin.Where(x => x.idLin == id).ToList();
            return model;
        }
        public List<Pro_LIn_Models> obtenerLinZonxUsu(string id)
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Pro_Lin.Include(x=>x.linea).Include(x => x.producto).Where(x => x.producto.idEmp == id).ToList();
            return model;
        }


    }
}