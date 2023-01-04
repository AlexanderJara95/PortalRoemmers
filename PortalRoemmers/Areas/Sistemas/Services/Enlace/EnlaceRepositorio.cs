using PortalRoemmers.Areas.Sistemas.Models.Enlace;
using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Enlace
{
    public class EnlaceRepositorio
    {
        public IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Enlace
                .Include(x=>x.tipoEnlace)
                .OrderBy(x => x.idEnl).Where(x => x.nomEnl.Contains(search) || x.desEnl.Contains(search) || x.tipoEnlace.nomTEnl.Contains(search))
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.tb_Enlace.OrderBy(x => x.idEnl).Include(x => x.tipoEnlace).Where(x => x.nomEnl.Contains(search) || x.desEnl.Contains(search) || x.tipoEnlace.nomTEnl.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.Enlace = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public Boolean crear(EnlaceModels model)
        {
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.tb_Enlace.Add(model);
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
        public EnlaceModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                EnlaceModels model = db.tb_Enlace.Include(x => x.tipoEnlace).Where(x => x.idEnl == id).FirstOrDefault();
                return model;
            }

        }
        public Boolean modificar(EnlaceModels model)
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
        public Boolean eliminar(string id)
        {
            Boolean mensaje = false;

            using (var db = new ApplicationDbContext())
            {
                EnlaceModels model = db.tb_Enlace.Find(id);
                db.tb_Enlace.Remove(model);
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

        public List<EnlaceModels> obtenerEnlances()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Enlace.OrderBy(x => x.nomEnl).ToList();
                return model;
            }
        }


    }
}