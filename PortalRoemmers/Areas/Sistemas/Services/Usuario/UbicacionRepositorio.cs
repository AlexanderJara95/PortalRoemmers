using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class UbicacionRepositorio
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

                var model = db.tb_Ubic
                  .OrderBy(x => new { x.cDepartamento,x.cProvincia }).Where(x => x.cPais.Contains(search) || x.cDepartamento.Contains(search) || x.cProvincia.Contains(search) || x.cDistrito.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Ubic.Where(x => x.cPais.Contains(search) || x.cDepartamento.Contains(search) || x.cProvincia.Contains(search) || x.cDistrito.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Ubicacion = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public UbicacionModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            UbicacionModels model = db.tb_Ubic.Find(id);
            return model;
        }
        public string crear(UbicacionModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            db.tb_Ubic.Add(model);
            try
            {
                db.SaveChanges();
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(UbicacionModels model)
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

            UbicacionModels model = db.tb_Ubic.Find(id);
            db.tb_Ubic.Remove(model);
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
        public IDictionary<string, string> busquedaPais(string term)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.tb_Ubic.Where(x => x.cPais.Contains(term)).Select(x=>new { x.cCod_Pais,x.cPais }).Distinct().ToDictionary(x => x.cCod_Pais, x => x.cPais);
            }
        }
        public IDictionary<string, string> busquedaDepa(string term,string pais)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.tb_Ubic.Where(x => x.cDepartamento.Contains(term)&& x.cCod_Pais== pais).Select(x => new { x.cCod_Dpto, x.cDepartamento }).Distinct().ToDictionary(x => x.cCod_Dpto, x => x.cDepartamento);
            }
        }
        public IDictionary<string, string> busquedaProv(string term, string pais, string depa)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.tb_Ubic.Where(x => x.cProvincia.Contains(term) && x.cCod_Pais == pais && x.cCod_Dpto == depa).Select(x => new { x.cCod_Provincia, x.cProvincia }).Distinct().ToDictionary(x => x.cCod_Provincia, x => x.cProvincia);
            }
        }
        public IDictionary<string, string> busquedaDist(string term, string pais, string depa, string prov)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.tb_Ubic.Where(x => x.cDistrito.Contains(term) && x.cCod_Pais == pais && x.cCod_Dpto == depa && x.cCod_Provincia == prov).Select(x => new { x.cCod_Distrito, x.cDistrito }).Distinct().ToDictionary(x => x.cCod_Distrito, x => x.cDistrito);
            }
        }
        public List<UbicacionModels> ubicacionPersonal()
        {
            var db = new ApplicationDbContext();
            var ubi = db.tb_Ubic.OrderBy(x => x.cPais).ToList();
            return ubi;
        }



    }
}