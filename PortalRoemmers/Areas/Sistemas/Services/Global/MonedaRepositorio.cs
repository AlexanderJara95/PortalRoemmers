using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Global
{
    public class MonedaRepositorio
    {
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }

            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Moneda
                .OrderBy(x => x.idMon).Where(x => x.nomMon.Contains(search) || x.abrMon.Contains(search))
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Moneda.Where(x => x.nomMon.Contains(search) || x.abrMon.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Moneda = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public MonedaModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            MonedaModels model = db.tb_Moneda.Find(id);
            return model;
        }
        public string crear(MonedaModels model)
        {
            Ennumerador enu = new Ennumerador();
            string mensaje = "";
            var db = new ApplicationDbContext();

            //creo su ID
            string idc = enu.buscarTabla("tb_Moneda").ToString();
            model.idMon = idc;

            db.tb_Moneda.Add(model);
            try
            {
                db.SaveChanges();
                enu.actualizarTabla("tb_Moneda", int.Parse(idc));
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(MonedaModels model)
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

            MonedaModels model = db.tb_Moneda.Find(id);
            db.tb_Moneda.Remove(model);
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
        public List<MonedaModels> obteneMonedas()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Moneda.OrderBy(x => x.nomMon).ToList();
            return model;
        }
    }
}