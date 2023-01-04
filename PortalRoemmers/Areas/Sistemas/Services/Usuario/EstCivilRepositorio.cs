using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class EstCivilRepositorio
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

                var model = db.tb_EstCiv
                  .OrderBy(x => x.idEstCiv).Where(x => x.nomEstCiv.Contains(search) || x.desEstCiv.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_EstCiv.Where(x => x.nomEstCiv.Contains(search) || x.desEstCiv.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Estado_C = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public EstCivilModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            EstCivilModels model = db.tb_EstCiv.Find(id);
            return model;
        }
        public string crear(EstCivilModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_EstCiv";
            int idc = enu.buscarTabla(tabla);
            model.idEstCiv = idc.ToString();

            db.tb_EstCiv.Add(model);
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
        public string modificar(EstCivilModels model)
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

            EstCivilModels model = db.tb_EstCiv.Find(id);
            db.tb_EstCiv.Remove(model);
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

        //listar
        public List<EstCivilModels> obtenerEstCi()
        {
            var db = new ApplicationDbContext();
            var estado = db.tb_EstCiv.OrderBy(x => x.idEstCiv).ToList();
            return estado;
        }
    }
}