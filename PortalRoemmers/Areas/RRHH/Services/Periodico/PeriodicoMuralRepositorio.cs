using PortalRoemmers.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; //permite usar landa
using PortalRoemmers.Areas.RRHH.Models.Periodico;
using PortalRoemmers.ViewModels;
using System;

namespace PortalRoemmers.Areas.RRHH.Services
{
    public class PeriodicoMuralRepositorio
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
                var model = db.tb_PeriodicoSec.Include(x=>x.efecIma)
                .OrderBy(x => x.idPerSec).Where(x => x.titPerSec.Contains(search) || x.efecIma.titEfeIma.Contains(search))
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.tb_PeriodicoSec.Include(x => x.efecIma).Where(x => x.titPerSec.Contains(search) || x.efecIma.titEfeIma.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.Periodico = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public Boolean crear(PeriodicoSeccionModels model)
        {
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.tb_PeriodicoSec.Add(model);
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
        public PeriodicoSeccionModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                PeriodicoSeccionModels model = db.tb_PeriodicoSec.Include(x => x.efecIma).Where(x => x.idPerSec == id).FirstOrDefault();
                return model;
            }

        }
        public Boolean modificar(PeriodicoSeccionModels model)
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
                PeriodicoSeccionModels model = db.tb_PeriodicoSec.Find(id);
                db.tb_PeriodicoSec.Remove(model);
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
        public List<PeriodicoSeccionModels> obtenerPeriodicoSeccion()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_PeriodicoSec.Include(x=>x.efecIma).Include(x=>x.contSec).OrderBy(x => x.idPerSec).ToList();
            return model;
        }
        public List<PeriodicoSeccionModels> obtenerPeriodicoVacio()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_PeriodicoSec.Include(x => x.contSec).OrderBy(x => x.idPerSec).Where(x=>x.contSec==null).ToList();
            return model;
        }

    }
}