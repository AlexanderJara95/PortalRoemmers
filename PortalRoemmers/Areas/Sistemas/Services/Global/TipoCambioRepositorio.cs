using System;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Global
{
    public class TipoCambioRepositorio
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

                var model = db.tb_TipoCambio
                   .OrderByDescending(x => x.fchTipoCbio).Where(x => x.monTCCompra.ToString().Contains(search) || x.monTCVenta.ToString().Contains(search) || x.fchTipoCbio.ToString().Contains(search))
                     .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                     .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipoCambio.Where(x => x.monTCCompra.ToString().Contains(search) || x.monTCVenta.ToString().Contains(search) || x.fchTipoCbio.ToString().Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.TiposCambio = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipoCambioModels obtenerItem(DateTime id)
        {
            var db = new ApplicationDbContext();
            TipoCambioModels model = db.tb_TipoCambio.Where(x=>x.fchTipoCbio== id).FirstOrDefault();
            return model;
        }
        public string crear(TipoCambioModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            
            db.tb_TipoCambio.Add(model);
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
        public string modificar(TipoCambioModels model)
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
        public string eliminar(DateTime id)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            TipoCambioModels model = db.tb_TipoCambio.Find(id);
            db.tb_TipoCambio.Remove(model);
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
        public List<TipoCambioModels> obtenerTiposCambios()
        {
            var db = new ApplicationDbContext();
            var cg = db.tb_TipoCambio.OrderBy(x => x.fchTipoCbio).ToList();
            return cg;
        }
    }
}