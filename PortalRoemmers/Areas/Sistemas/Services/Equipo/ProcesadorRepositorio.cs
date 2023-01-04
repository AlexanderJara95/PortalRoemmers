using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Equipo
{
    public class ProcesadorRepositorio
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

                var model = db.tb_Proce
                  .OrderBy(x => x.idProce).Where(x => x.nomProce.Contains(search) || x.descProce.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Proce.Where(x => x.nomProce.Contains(search) || x.descProce.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Procesadores = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public ProcesadorModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            ProcesadorModels model = db.tb_Proce.Find(id);
            return model;
        }
        public Boolean crear(ProcesadorModels model)
        {
            Boolean mensaje = false;
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_Proce";
            int idc = enu.buscarTabla(tabla);
            model.idProce = idc.ToString();

            db.tb_Proce.Add(model);
            try
            {
                db.SaveChanges();
                enu.actualizarTabla(tabla, idc);
                mensaje = true;
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return mensaje;
        }
        public string modificar(ProcesadorModels model)
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

            ProcesadorModels model = db.tb_Proce.Find(id);
            db.tb_Proce.Remove(model);
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
        public List<ProcesadorModels> obtenerProcesador()
        {
            var db = new ApplicationDbContext();
            var proce = db.tb_Proce.OrderBy(x => x.nomProce).ToList();
            return proce;
        }
    }
}