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
    public class LineaRepositorio
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

                var model = db.tb_Linea.Include(x=>x.estado)
                  .OrderBy(x => x.idLin).Where(x => x.nomLin.Contains(search) || x.desLin.Contains(search)|| x.estado.nomEst.Contains(search))
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Linea.Include(x => x.estado).Where(x => x.nomLin.Contains(search) || x.desLin.Contains(search) || x.estado.nomEst.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Linea = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public LineaModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            LineaModels model = db.tb_Linea.Find(id);
            return model;
        }
        public string crear(LineaModels model)
        {

            //creo su ID
            string tabla = "tb_Linea";
            string id = enu.buscarTabla(tabla).ToString();
            model.idLin = id;

            string mensaje = "";
            var db = new ApplicationDbContext();

            db.tb_Linea.Add(model);
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
        public string modificar(LineaModels model)
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

            LineaModels model = db.tb_Linea.Find(id);
            db.tb_Linea.Remove(model);
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
        public List<LineaModels> obtenerLineas()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Linea.Where(x=>x.idEst== ConstantesGlobales.estadoActivo).OrderBy(x => x.nomLin).ToList();
            return model;
        }

    }
}