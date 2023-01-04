using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity; //permite usar landa
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Equipo
{
    public class ModeloERepositorio
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

                var model = db.tb_ModEqui
                  .Include(x=>x.fabrica)
                  .OrderBy(x => x.idFabrica).Where(x => x.nomMolEq.Contains(search) || x.descMolEq.Contains(search)|| x.fabrica.nomFabri.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_ModEqui.Where(x => x.nomMolEq.Contains(search) || x.descMolEq.Contains(search) || x.fabrica.nomFabri.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.ModelosE = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public ModEquiModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            ModEquiModels model = db.tb_ModEqui.Find(id);
            return model;
        }
        public Boolean crear(ModEquiModels model)
        {
            Boolean mensaje = false;
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_ModEqui";
            int idc = enu.buscarTabla(tabla);
            model.idMolEq = idc.ToString();

            db.tb_ModEqui.Add(model);
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
        public string modificar(ModEquiModels model)
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

            ModEquiModels model = db.tb_ModEqui.Find(id);
            db.tb_ModEqui.Remove(model);
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
        public List<ModEquiModels> obtenerModelosEquipo()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_ModEqui.OrderBy(x => x.nomMolEq).ToList();
            return model;
        }

    }
}