using PortalRoemmers.Areas.RRHH.Models.Periodico;
using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Services.Periodico
{
    public class ContenidoRepositorio
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
                var model = db.tb_ContenidoSec.Include(x => x.perSec)
                .OrderBy(x => x.idConSec).Where(x => x.titConSec.Contains(search) || x.perSec.titPerSec.Contains(search))
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.tb_ContenidoSec.Include(x => x.perSec).Where(x => x.titConSec.Contains(search) || x.perSec.titPerSec.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.Contenido = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public Boolean crear(ContenidoSeccionModels model)
        {
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.tb_ContenidoSec.Add(model);
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
        public ContenidoSeccionModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                ContenidoSeccionModels model = db.tb_ContenidoSec.Include(x => x.perSec).Where(x => x.idConSec == id).FirstOrDefault();
                return model;
            }

        }
        public Boolean modificar(ContenidoSeccionModels model)
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
                ContenidoSeccionModels model = db.tb_ContenidoSec.Find(id);
                db.tb_ContenidoSec.Remove(model);
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
    }
}