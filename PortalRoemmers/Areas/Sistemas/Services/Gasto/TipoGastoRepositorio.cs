using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Gasto
{
    public class TipoGastoRepositorio
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

                var model = db.tb_TipGas
                  .OrderBy(x => x.idTipGas).Where(x => x.nomTipGas.Contains(search) || x.descTipGas.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipGas.Where(x => x.nomTipGas.Contains(search) || x.descTipGas.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.TipGasto = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipoGastoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoGastoModels model = db.tb_TipGas.Find(id);
            return model;
        }
        public string crear(TipoGastoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            //creo su ID
            string tabla = "tb_TipGas";
            int idc = enu.buscarTabla(tabla);
            model.idTipGas = idc.ToString();



            db.tb_TipGas.Add(model);
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
        public string modificar(TipoGastoModels model)
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

            TipoGastoModels model = db.tb_TipGas.Find(id);
            db.tb_TipGas.Remove(model);
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
        public List<TipoGastoModels> obtenerTipGasto()
        {
            var db = new ApplicationDbContext();
            var tg = db.tb_TipGas.Where(y=>y.idEst == ConstantesGlobales.estadoActivo).OrderBy(x => x.nomTipGas).ToList();
            return tg;
        }

    }
}