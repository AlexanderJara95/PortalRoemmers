using System;
using System.Collections.Generic;
using System.Linq;
using PortalRoemmers.Security;
using System.Data.SqlClient;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using System.Data.Entity; //permite usar landa
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Presupuesto
{
    public class TipoPresupuestoRepositorio
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

                var model = db.tb_TipPres.Include(x => x.estado)
                    .OrderBy(x => x.idTipoPres).Where(x=>x.idTipoPres.Contains(search) || x.nomTipPres.Contains(search) || x.descTipoPres.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipPres.Where(x => x.idTipoPres.Contains(search) || x.nomTipPres.Contains(search) || x.descTipoPres.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.TipPres = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public TipoPresupuestoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoPresupuestoModels model = db.tb_TipPres.Find(id);
            return model;
        }
        public string crear(TipoPresupuestoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            //creo su ID
            string tabla = "tb_tipPres";
            int idc = enu.buscarTabla(tabla);
            model.idTipoPres = idc.ToString("D3");

            db.tb_TipPres.Add(model);
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
        public string modificar(TipoPresupuestoModels model)
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

            TipoPresupuestoModels model = db.tb_TipPres.Find(id);
            db.tb_TipPres.Remove(model);
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
        public List<TipoPresupuestoModels> obtenerTipPres()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_TipPres.OrderBy(x => x.nomTipPres).Where(x=>x.idEst!=ConstantesGlobales.estadoInactivo).ToList();
            return model;
        }
    }
}