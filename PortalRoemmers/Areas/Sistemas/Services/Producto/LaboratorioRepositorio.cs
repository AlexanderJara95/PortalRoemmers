using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Producto
{
    public class LaboratorioRepositorio
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

                var model = db.tb_LabPro
                  .OrderBy(x => x.idLab).Where(x => x.nomLab.Contains(search)|| x.desLab.Contains(search))
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_LabPro.Where(x => x.nomLab.Contains(search) || x.desLab.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Laboratorio = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public LaboratorioModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            LaboratorioModels model = db.tb_LabPro.Find(id);
            return model;
        }
        public string crear(LaboratorioModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_LabPro";
            int idc = enu.buscarTabla(tabla);
            model.idLab = idc.ToString();

            db.tb_LabPro.Add(model);
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
        public string modificar(LaboratorioModels model)
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

            LaboratorioModels model = db.tb_LabPro.Find(id);
            db.tb_LabPro.Remove(model);
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
        public List<LaboratorioModels> obtenerLaboratorio()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_LabPro.OrderBy(x => x.nomLab).ToList();
            return model;
        }
    }
}