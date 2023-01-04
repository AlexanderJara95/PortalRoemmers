using PortalRoemmers.Areas.Sistemas.Models.Solicitud;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Solicitud
{
    public class TipoPagoRepositorio
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

                var model = db.tb_TipPag
                   .OrderBy(x => x.idTipPag).Where(x => x.nomTipPag.Contains(search) || x.descTipPag.Contains(search))
                     .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                     .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipPag.Where(x => x.nomTipPag.Contains(search) || x.descTipPag.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.TipoPagos = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipoPagoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoPagoModels model = db.tb_TipPag.Find(id);
            return model;
        }
        public string crear(TipoPagoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            
            //creo su ID
            string tabla = "tb_TipPag";
            int idc = enu.buscarTabla(tabla);
            model.idTipPag = idc.ToString();

            db.tb_TipPag.Add(model);
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
        public string modificar(TipoPagoModels model)
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

            TipoPagoModels model = db.tb_TipPag.Find(id);
            db.tb_TipPag.Remove(model);
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
        public List<TipoPagoModels> obtenerTipoPago()
        {
            var db = new ApplicationDbContext();
            var cg = db.tb_TipPag.OrderBy(x => x.nomTipPag).Where(x=>x.idEst!=ConstantesGlobales.estadoInactivo).ToList();
            return cg;
        }
    }
}