using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Producto
{
    public class FamiliaAXRepositorio
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

                var model = db.tb_FamProdAx
                  .OrderBy(x => x.idFam).Where(x => x.nomFam.Contains(search)|| x.desFam.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_FamProdAx.Where(x => x.nomFam.Contains(search) || x.desFam.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.FamiliaAX = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public FamProdAxModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            FamProdAxModels model = db.tb_FamProdAx.Find(id);
            return model;
        }
        public string crear(FamProdAxModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_FamProdAx";
            int idc = enu.buscarTabla(tabla);
            model.idFam = idc.ToString();

            db.tb_FamProdAx.Add(model);
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
        public string modificar(FamProdAxModels model)
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

            FamProdAxModels model = db.tb_FamProdAx.Find(id);
            db.tb_FamProdAx.Remove(model);
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
        public List<FamProdAxModels> obtenerFamiliaAX()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_FamProdAx.OrderBy(x => x.nomFam).ToList();
            return model;
        }

    }
}