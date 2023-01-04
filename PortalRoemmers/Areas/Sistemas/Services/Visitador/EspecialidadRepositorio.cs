using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Visitador
{
    public class EspecialidadRepositorio
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

                var model = db.tb_Especialidad.Include(x=>x.estado)
                  .OrderBy(x => x.idEsp).Where(x => x.nomEsp.Contains(search) || x.desEsp.Contains(search) || x.estado.nomEst.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Especialidad.Include(x => x.estado).Where(x => x.nomEsp.Contains(search) || x.desEsp.Contains(search) || x.estado.nomEst.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Especialidad = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public EspecialidadModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            EspecialidadModels model = db.tb_Especialidad.Find(id);
            return model;
        }
        public string crear(EspecialidadModels model)
        {
            //creo su ID
            string tabla = "tb_Especialidad";
            string id = enu.buscarTabla(tabla).ToString();
            model.idEsp = id;
            string mensaje = "";
            var db = new ApplicationDbContext();

            db.tb_Especialidad.Add(model);
            try
            {
                db.SaveChanges();
                enu.actualizarTabla(tabla, int.Parse(id));
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(EspecialidadModels model)
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

            EspecialidadModels model = db.tb_Especialidad.Find(id);
            db.tb_Especialidad.Remove(model);
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
        public List<EspecialidadModels> obteneEspecialidades()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Especialidad.OrderBy(x => x.nomEsp!=ConstantesGlobales.estadoInactivo).ToList();
            return model;
        }

    }
}