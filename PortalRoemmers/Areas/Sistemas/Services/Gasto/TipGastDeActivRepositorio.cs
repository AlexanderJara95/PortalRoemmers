using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;

namespace PortalRoemmers.Areas.Sistemas.Services.Gasto
{
    public class TipGastDeActivRepositorio
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

                var model = db.tb_TipGasAct
                  .OrderBy(x => x.idTipGasAct).Where(x => x.nomTipGasAct.Contains(search) || x.abrTipGasAct.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipGasAct.Where(x => x.nomTipGasAct.Contains(search) || x.abrTipGasAct.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.tipoGastDeActiv = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipGastDeActivModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipGastDeActivModels model = db.tb_TipGasAct.Find(id);
            return model;
        }
        public string crear(TipGastDeActivModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            
            //creo su ID
            string tabla = "tb_TipGasAct";
            int idc = enu.buscarTabla(tabla);
            model.idTipGasAct = idc.ToString();

            db.tb_TipGasAct.Add(model);
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
        public string modificar(TipGastDeActivModels model)
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

            TipGastDeActivModels model = db.tb_TipGasAct.Find(id);
            db.tb_TipGasAct.Remove(model);
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
        public List<TipGastDeActivModels> obtenerTipGastoAct()
        {
            var db = new ApplicationDbContext();
            var tag = db.tb_TipGasAct.OrderBy(x => x.nomTipGasAct).ToList();
            return tag;
        }
    }
}