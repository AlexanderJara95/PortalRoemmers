using PortalRoemmers.Models;
using System;
using System.Linq;
using System.Data.Entity; //permite usar landa
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Helpers;
using System.Collections.Generic;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class CargoRepositorio
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

                var model = db.tb_Cargo
                    .OrderBy(x => x.idCarg).Where(x => x.desCarg.Contains(search) || x.abreCarg.Contains(search) )
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Cargo.Where(x => x.desCarg.Contains(search) || x.abreCarg.Contains(search) ).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Cargo = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public CargoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            CargoModels model = db.tb_Cargo.Find(id);
            return model;
        }
        public string crear(CargoModels model)
        {
            //creo su ID
            string tabla = "tb_Cargo";
            string id = enu.buscarTabla(tabla).ToString();
            model.idCarg = id;

            string mensaje = "";
            var db = new ApplicationDbContext();

            db.tb_Cargo.Add(model);
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
        public string modificar(CargoModels model)
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

            CargoModels model = db.tb_Cargo.Find(id);
            db.tb_Cargo.Remove(model);
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
        public List<CargoModels> obtenerCargo()
        {
            using (var db = new ApplicationDbContext())
            {
                // var cargo = db.tb_Cargo.OrderBy(x => x.idCarg).Select(h => new SelectListItem { Value = h.idCarg.ToString(), Text = h.DesCarg }).ToList();
                var cargo = db.tb_Cargo.OrderBy(x => x.idCarg).ToList();
                return cargo;
            }
        }
    }
}