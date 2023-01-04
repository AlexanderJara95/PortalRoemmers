using PortalRoemmers.Areas.Sistemas.Models.Roles;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Roles
{
    public class TipoRolRepositorio
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

                var model = db.tb_TipRol
                  .OrderBy(x => x.idTipRol).Where(x => x.nomTipRol.Contains(search) || x.desTipRol.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipRol.Where(x => x.nomTipRol.Contains(search) || x.desTipRol.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.TipRoles = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipoRolModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoRolModels model = db.tb_TipRol.Find(id);
            return model;
        }
        public string crear(TipoRolModels model)
        {
            Ennumerador enu = new Ennumerador();
            string mensaje = "";
            var db = new ApplicationDbContext();

            //creo su ID
            int idc = enu.buscarTabla("tb_TipRol");
            model.idTipRol = idc.ToString("D2");


            db.tb_TipRol.Add(model);
            try
            {
                db.SaveChanges();
                enu.actualizarTabla("tb_TipRol", idc);
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(TipoRolModels model)
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

            TipoRolModels model = db.tb_TipRol.Find(id);
            db.tb_TipRol.Remove(model);
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

        public List<TipoRolModels> obtenerTipRol()
        {
            using (var db = new ApplicationDbContext())
            {
                var tipRol = db.tb_TipRol.OrderBy(x => x.idTipRol).ToList();
                return tipRol;
            }
        }

    }
}