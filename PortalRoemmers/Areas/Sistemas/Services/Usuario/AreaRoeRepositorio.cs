using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class AreaRoeRepositorio
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

                var model = db.tb_Area
                  .OrderBy(x => x.idAreRoe).Where(x => x.nomAreRoe.Contains(search) || x.desAreRoe.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Area.Where(x => x.nomAreRoe.Contains(search) || x.desAreRoe.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.AreaRoe = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public AreaRoeModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            AreaRoeModels model = db.tb_Area.Find(id);
            return model;
        }
        public string crear(AreaRoeModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            //creo su ID
            string tabla = "tb_Area";
            int idc = enu.buscarTabla(tabla);
            model.idAreRoe = idc.ToString();

            db.tb_Area.Add(model);
            try
            {
                db.SaveChanges();
                enu.actualizarTabla(tabla, idc);
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(AreaRoeModels model)
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

            AreaRoeModels model = db.tb_Area.Find(id);
            db.tb_Area.Remove(model);
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
        //listar
        public List<AreaRoeModels> obtenerArea()
        {
            var db = new ApplicationDbContext();
            var area = db.tb_Area.OrderBy(x => x.nomAreRoe).ToList();
            return area;
        }
    }
}