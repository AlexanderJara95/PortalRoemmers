using PortalRoemmers.Areas.RRHH.Models.Galeria;
using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.RRHH.Services
{
    public class TipoGaleriaRepositorio
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
                var model = db.tb_TipGal
                .OrderBy(x => x.idTipGal).Where(x => x.titTipGal.Contains(search) || x.desTipGal.Contains(search))
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.tb_TipGal.Where(x => x.titTipGal.Contains(search) || x.desTipGal.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.TipoGaleria = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public Boolean crear(TipoGaleriaModels model)
        {
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.tb_TipGal.Add(model);
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
        public TipoGaleriaModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                TipoGaleriaModels model = db.tb_TipGal.Where(x => x.idTipGal == id).FirstOrDefault();
                return model;
            }

        }
        public Boolean modificar(TipoGaleriaModels model)
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
                TipoGaleriaModels model = db.tb_TipGal.Find(id);
                db.tb_TipGal.Remove(model);
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
        public List<TipoGaleriaModels> obtenerTipoGalerias()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_TipGal.OrderBy(x => x.idTipGal).ToList();
            return model;
        }
    }
}