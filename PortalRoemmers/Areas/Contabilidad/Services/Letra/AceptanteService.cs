using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PortalRoemmers.Models;
using PortalRoemmers.Areas.Contabilidad.Models.Letra;
using System.Data.Entity;
using PortalRoemmers.Helpers;

namespace PortalRoemmers.Areas.Contabilidad.Services.Letra
{
    public class AceptanteService
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

                var model = db.tb_Aceptante
                    .Include(x => x.estado)
                    .OrderBy(x => x.idAcep).Where(x => x.nomAceptante.Contains(search) || x.niffAceptante.Contains(search) || x.locAceptante.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Aceptante.Where(x => x.nomAceptante.Contains(search) || x.niffAceptante.Contains(search) || x.locAceptante.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Aceptantes = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public AceptanteModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            AceptanteModels model = db.tb_Aceptante.Find(id);
            return model;
        }
        public string crear(AceptanteModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //-----------------------------------
            //creo su ID
            string tabla = "tb_Aceptante";
            string id = enu.buscarTabla(tabla).ToString();
            enu.actualizarTabla(tabla, Int32.Parse(id));
            model.idAcep = id;
            //-----------------------------------
            db.tb_Aceptante.Add(model);
            try
            {
                db.SaveChanges();
                mensaje = "<div id='success' class='alert alert-success'>Se creó el registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(AceptanteModels model)
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

            AceptanteModels model = db.tb_Aceptante.Find(id);

            if (model.idEst == ConstantesGlobales.estadoInactivo)
            {
                db.tb_Aceptante.Remove(model);
                try
                {
                    db.SaveChanges();
                    mensaje = "<div id='success' class='alert alert-success'>Se eliminó un registro.</div>";
                }
                catch (Exception e)
                {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                }
            }
            else
            {
                mensaje = "<div id='warning' class='alert alert-warning'>El registro no se puede eliminar debido al estado ACTIVO.</div>";
            }
            return mensaje;
        }
        //listados
        public List<AceptanteModels> obtenerAceptantes()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Aceptante.OrderBy(x => x.idAcep).Where(x => x.idEst != ConstantesGlobales.estadoInactivo).ToList();
            return model;
        }
    }
}