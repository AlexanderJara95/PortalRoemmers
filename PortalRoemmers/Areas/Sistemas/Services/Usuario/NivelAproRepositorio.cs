using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class NivelAproRepositorio
    {
        Ennumerador enu = new Ennumerador();
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }

            using (var db = new ApplicationDbContext())
            {

                var model = db.tb_NivApr
                  .OrderBy(x => x.idNapro).Where(x => x.nomNapro.Contains(search) || x.desNapro.Contains(search) || x.abrNapro.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_NivApr.Where(x => x.nomNapro.Contains(search) || x.desNapro.Contains(search) || x.abrNapro.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Nivel = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public NivelAproModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            NivelAproModels model = db.tb_NivApr.Find(id);
            return model;
        }
        public string crear(NivelAproModels model)
        {
            string tabla = "tb_NivApr";
            string id = enu.buscarTabla(tabla).ToString();
            model.idNapro = id;

            string mensaje = "";
            var db = new ApplicationDbContext();

            db.tb_NivApr.Add(model);
            try
            {
                db.SaveChanges();
                enu.actualizarTabla(tabla, int.Parse(id));
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(NivelAproModels model)
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

            NivelAproModels model = db.tb_NivApr.Find(id);
            db.tb_NivApr.Remove(model);
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

        //listado
        public List<NivelAproModels> obtenerNivelApro()
        {
            var db = new ApplicationDbContext();
            var niv = db.tb_NivApr.OrderBy(x => x.idNapro).ToList();
            return niv;
        }
    }
}