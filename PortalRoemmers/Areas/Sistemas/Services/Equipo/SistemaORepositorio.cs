using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace PortalRoemmers.Areas.Sistemas.Services.Equipo
{
    public class SistemaORepositorio
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

                var model = db.tb_SisOp
                  .OrderBy(x => x.idSo).Where(x => x.nomSo.Contains(search) || x.descSo.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_SisOp.Where(x => x.nomSo.Contains(search) || x.descSo.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.S_Operativos = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public SistemaOModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            SistemaOModels model = db.tb_SisOp.Find(id);
            return model;
        }
        public string crear(SistemaOModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_SisOp";
            int idc = enu.buscarTabla(tabla);
            model.idSo = idc.ToString();

            db.tb_SisOp.Add(model);
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
        public string modificar(SistemaOModels model)
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

            SistemaOModels model = db.tb_SisOp.Find(id);
            db.tb_SisOp.Remove(model);
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
        public List<SistemaOModels> obtenerSO()
        {
            var db = new ApplicationDbContext();
            var so = db.tb_SisOp.OrderBy(x => x.nomSo).ToList();
            return so;
        }
    }
}