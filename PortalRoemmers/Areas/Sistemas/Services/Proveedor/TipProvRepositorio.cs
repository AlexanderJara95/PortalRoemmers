using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PortalRoemmers.Areas.Sistemas.Models.Proveedor;
using System.Data.Entity;

namespace PortalRoemmers.Areas.Sistemas.Services.Proveedor
{
    public class TipProvRepositorio
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

                var model = db.tb_TipPro
                  .OrderBy(x => x.idTipPro).Where(x => x.nomTipPro.Contains(search) || x.desTipPro.Contains(search))
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipPro.Where(x => x.nomTipPro.Contains(search) || x.desTipPro.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.TipPro = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipoProveedorModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoProveedorModels model = db.tb_TipPro.Find(id);
            return model;
        }
        public string crear(TipoProveedorModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_TipPro";
            int idc = enu.buscarTabla(tabla);
            model.idTipPro = idc.ToString();

            db.tb_TipPro.Add(model);
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
        public string modificar(TipoProveedorModels model)
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

            TipoProveedorModels model = db.tb_TipPro.Find(id);
            db.tb_TipPro.Remove(model);
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
        public List<TipoProveedorModels> obtenerTipoProv()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_TipPro.OrderBy(x => x.nomTipPro).ToList();
            return model;
        }
    }
}