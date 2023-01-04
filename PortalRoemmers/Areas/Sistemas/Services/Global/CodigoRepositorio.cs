using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Global
{
    public class CodigoRepositorio
    {
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }

            using (var db = new ApplicationDbContext())
            {

                var model = db.tb_IdTablas
                  .OrderBy(x => x.idTabla).Where(x => x.idTabla.Contains(search) || x.nrotabla.ToString().Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_IdTablas.Where(x => x.idTabla.Contains(search) || x.nrotabla.ToString().Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Codigos = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public CodigoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            CodigoModels model = db.tb_IdTablas.Find(id);
            return model;
        }
        public string crear(CodigoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            db.tb_IdTablas.Add(model);
            try
            {
                db.SaveChanges();
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(CodigoModels model)
        {
            string mensaje = "";

            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    mensaje = "<div id='success' class='alert alert-success'>Se modificó el registro.</div>";
                }
                catch (Exception e)
                {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                }
            }
            return mensaje;
        }
        public string eliminar(string id)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            CodigoModels model = db.tb_IdTablas.Find(id);
            db.tb_IdTablas.Remove(model);
            try
            {
                db.SaveChanges();
                mensaje = "<div id='success' class='alert alert-success'>Se eliminó un registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
    }
}