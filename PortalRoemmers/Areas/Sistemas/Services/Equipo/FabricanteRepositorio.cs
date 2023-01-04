using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace PortalRoemmers.Areas.Sistemas.Services.Equipo
{
    public class FabricanteRepositorio
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

                var model = db.tb_Fabricante
                  .OrderBy(x => x.idFabrica).Where(x => x.nomFabri.Contains(search)|| x.descFabri.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Fabricante.Where(x => x.nomFabri.Contains(search) || x.descFabri.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Fabricantes = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public FabricanteEquipoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            FabricanteEquipoModels model = db.tb_Fabricante.Find(id);
            return model;
        }
        public string crear(FabricanteEquipoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_Fabricante";
            int idc = enu.buscarTabla(tabla);
            model.idFabrica = idc.ToString();

            db.tb_Fabricante.Add(model);
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
        public string modificar(FabricanteEquipoModels model)
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

            FabricanteEquipoModels model = db.tb_Fabricante.Find(id);
            db.tb_Fabricante.Remove(model);
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
        public List<FabricanteEquipoModels> obtenerFabricantes()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Fabricante.OrderBy(x => x.nomFabri).ToList();
            return model;
        }
    }
}