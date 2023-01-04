using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Producto
{
    public class AreaTerapeuticaRepositorio
    {
        Ennumerador enu = new Ennumerador();
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search)
        {
            //-------------------------------------------
            int cantidadRegistrosPorPagina = 10;
            //-------------------------------------------
            if (pagina == 0)
            {
                pagina = 1;
            }
            //-------------------------------------------
            using (var db = new ApplicationDbContext())
            {
                //-------------------------------------------
                var model = db.tb_AreaTerap
                  .OrderBy(x => x.idAreaTerap).Where(x => x.numAreaTerap.Contains(search) || x.desAreaTerap.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_AreaTerap.Where(x => x.numAreaTerap.Contains(search) || x.desAreaTerap.Contains(search)).Count();
                //-------------------------------------------
                var modelo = new ViewModels.IndexViewModel();
                modelo.AreaTerapeutica = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;
                //-------------------------------------------
                return modelo;
                //-------------------------------------------
            }
        }
        public AreaTerapeuticaModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            AreaTerapeuticaModels model = db.tb_AreaTerap.Find(id);
            return model;
        }
        public string crear(AreaTerapeuticaModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_AreaTerap";
            int idc = enu.buscarTabla(tabla);
            model.idAreaTerap = idc.ToString();

            db.tb_AreaTerap.Add(model);
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
        public string modificar(AreaTerapeuticaModels model)
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

            AreaTerapeuticaModels model = db.tb_AreaTerap.Find(id);
            db.tb_AreaTerap.Remove(model);
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
        //listados
        public List<AreaTerapeuticaModels> obtenerAreaTerap()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_AreaTerap.OrderBy(x => x.idAreaTerap).ToList();
            return model;
        }
    }
}