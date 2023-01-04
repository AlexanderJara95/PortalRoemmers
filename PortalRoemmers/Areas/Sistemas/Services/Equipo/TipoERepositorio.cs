using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace PortalRoemmers.Areas.Sistemas.Services.Equipo
{
    public class TipoERepositorio
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

                var model = db.tb_TipEqui
                  .OrderBy(x => x.idTipEqui).Where(x => x.nomTipEqui.Contains(search) || x.desTipEqui.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipEqui.Where(x => x.nomTipEqui.Contains(search) || x.desTipEqui.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.T_Equipos = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipoEquipoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoEquipoModels model = db.tb_TipEqui.Find(id);
            return model;
        }
        public string crear(TipoEquipoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_TipEqui";
            int idc = enu.buscarTabla(tabla);
            model.idTipEqui = idc.ToString();

            db.tb_TipEqui.Add(model);
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
        public string modificar(TipoEquipoModels model)
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

            TipoEquipoModels model = db.tb_TipEqui.Find(id);
            db.tb_TipEqui.Remove(model);
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
        public List<TipoEquipoModels> obtenerTiposEquipo()
        {
            var db = new ApplicationDbContext();
            var tipEqui = db.tb_TipEqui.OrderBy(x => x.nomTipEqui).ToList();
            return tipEqui;
        }
    }
}