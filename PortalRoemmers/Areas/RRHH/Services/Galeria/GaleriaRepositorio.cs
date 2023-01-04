using PortalRoemmers.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; //permite usar landa
using PortalRoemmers.Areas.RRHH.Models.Galeria;
using PortalRoemmers.ViewModels;
using System;

namespace PortalRoemmers.Areas.RRHH.Services
{
    public class GaleriaRepositorio
    {
        public IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Galeria.Include(x=>x.tipoGal)
                .OrderBy(x => x.idGaleria).Where(x => x.titGaleria.Contains(search) || x.tipoGal.titTipGal.Contains(search))
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.tb_Galeria.Include(x => x.tipoGal).Where(x => x.titGaleria.Contains(search) || x.tipoGal.titTipGal.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.Galeria = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public Boolean crear(GaleriaModels model)
        {
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.tb_Galeria.Add(model);
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            return mensaje;
        }
        public GaleriaModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                GaleriaModels model = db.tb_Galeria.Include(x => x.tipoGal).Where(x => x.idGaleria == id).FirstOrDefault();
                return model;
            }

        }
        public Boolean modificar(GaleriaModels model)
        {
            Boolean ok = false;
            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    ok = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            return ok;
        }
        public Boolean eliminar(string id)
        {
            Boolean mensaje = false;

            using (var db = new ApplicationDbContext())
            {
                GaleriaModels model = db.tb_Galeria.Find(id);
                db.tb_Galeria.Remove(model);
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            return mensaje;
        }
        public List<GaleriaModels> obtenerGaleria()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Galeria.Include(x=>x.tipoGal).OrderBy(x => x.idGaleria).ToList();
            return model;
        }
    }
}