using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Producto
{
    public class ProductoRepositorio
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

                var model = db.tb_Producto
                  .Include(y => y.usuarioGP)
                  .Include(c => c.familia)
                  .OrderBy(x => x.idProAX).Where(x => x.idProAX.Contains(search) || x.nomPro.Contains(search) || x.familia.nomFam.Contains(search) || x.usuarioGP.apePatEmp.Contains(search) )
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Producto.Where(x => x.idProAX.Contains(search) || x.nomPro.Contains(search) || x.familia.nomFam.Contains(search) || x.usuarioGP.apePatEmp.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Productos = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public ProductoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            ProductoModels model = db.tb_Producto.Find(id);
            return model;
        }
        public string crear(ProductoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            
            db.tb_Producto.Add(model);
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
        public string modificar(ProductoModels model)
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

            ProductoModels model = db.tb_Producto.Find(id);
            db.tb_Producto.Remove(model);
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
        //listados
        public List<ProductoModels> obtenerProductos()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Producto
                .Include(x=>x.familiaRoe)
                .Include(x=>x.areaTerap)
                .OrderBy(x => x.idProAX).Where(x=>x.idEst!=ConstantesGlobales.estadoDescontinuado).ToList();
            return model;
        }
        //Obtener Area terapeutica del Producto
        public string obtenerAT(string idFamRoe)
        {
            string resultado = "";
            var db = new ApplicationDbContext();
            var model = db.tb_Producto.Where(x => x.idFamRoe == idFamRoe).Select(x=>x.idAreaTerap).Distinct().ToList();
            if (model.Count() == 1)
            {
                resultado = model.FirstOrDefault();
            }
            else
            {
                foreach (var item in model)
                {
                    if (resultado != "")
                    {
                        resultado += "|";
                    }
                    resultado += item; 
                }
            }
            return resultado;
        }

    }
}