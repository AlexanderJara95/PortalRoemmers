using System;
using PortalRoemmers.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using PortalRoemmers.Helpers;
using PortalRoemmers.Areas.Sistemas.Models.Solicitud;

namespace PortalRoemmers.Areas.Sistemas.Services.Solicitud
{
    public class TipoSolRepositorio
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

                var model = db.tb_TipSol
                   .OrderBy(x => x.idTipSol).Where(x => x.nomTipSol.Contains(search) || x.descTipSol.Contains(search))
                     .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                     .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipSol.Where(x => x.nomTipSol.Contains(search) || x.descTipSol.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.TipoSol = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipoSolModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoSolModels model = db.tb_TipSol.Find(id);
            return model;
        }
        public string crear(TipoSolModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            
            //creo su ID
            string tabla = "tb_TipSol";
            int idc = enu.buscarTabla(tabla);
            model.idTipSol = idc.ToString();

            db.tb_TipSol.Add(model);
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
        public string modificar(TipoSolModels model)
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

            TipoSolModels model = db.tb_TipSol.Find(id);
            db.tb_TipSol.Remove(model);
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
        public List<TipoSolModels> obtenerTipoSolicitudes()
        {
            var db = new ApplicationDbContext();
            var cg = db.tb_TipSol.OrderBy(x => x.nomTipSol).Where(x=>x.idEst!=ConstantesGlobales.estadoInactivo).ToList();
            return cg;
        }
    }
}