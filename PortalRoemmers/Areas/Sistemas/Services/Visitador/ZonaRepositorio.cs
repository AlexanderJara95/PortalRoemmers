using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Visitador
{
    public class ZonaRepositorio
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

                var model = db.tb_Zona.Include(x=>x.estado)
                  .OrderBy(x => x.idZon).Where(x => x.nomZon.Contains(search) || x.desZon.Contains(search) || x.estado.nomEst.Contains(search))
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Zona.Include(x => x.estado).Where(x => x.nomZon.Contains(search) || x.desZon.Contains(search) || x.estado.nomEst.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Zona = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public ZonaModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            ZonaModels model = db.tb_Zona.Find(id);
            return model;
        }
        public string crear(ZonaModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            //creo su ID
            string tabla = "tb_Zona";
            int idc = enu.buscarTabla(tabla);
            model.idZon = idc.ToString();

            db.tb_Zona.Add(model);
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
        public string modificar(ZonaModels model)
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

            ZonaModels model = db.tb_Zona.Find(id);
            db.tb_Zona.Remove(model);
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
        public List<ZonaModels> obtenerZonas()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Zona.OrderBy(x => x.nomZon).Where(x => x.idEst != ConstantesGlobales.estadoInactivo).ToList();
            return model;
        }
    }
}