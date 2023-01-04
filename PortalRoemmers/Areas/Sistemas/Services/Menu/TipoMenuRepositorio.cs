using PortalRoemmers.Areas.Sistemas.Models.Menu;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Menu
{
    public class TipoMenuRepositorio
    {
        Ennumerador enu = new Ennumerador();
        public IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;
            if (pagina == 0)
            {
                pagina = 1;
            }
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_TipMenu
                  .OrderBy(x => x.nomTipMen).Where(x => x.nomTipMen.Contains(search) || x.desTipMen.Contains(search))
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipMenu.Where(x => x.nomTipMen.Contains(search) || x.desTipMen.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.TipoMenu = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public TipoMenuModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoMenuModels model = db.tb_TipMenu.Find(id);
            return model;
        }
        public string crear(TipoMenuModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            //creo su ID
            string tabla = "tb_TipMenu";
            int idc = enu.buscarTabla(tabla);
            model.idTipMen = idc.ToString("D2");
            db.tb_TipMenu.Add(model);
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
        public string modificar(TipoMenuModels model)
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

            TipoMenuModels model = db.tb_TipMenu.Find(id);
            db.tb_TipMenu.Remove(model);
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
        public List<TipoMenuModels> obtenerTipoMenu()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_TipMenu.OrderBy(x => x.nomTipMen).ToList();
                return model;
            }
        }

    }
}