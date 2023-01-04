using PortalRoemmers.Areas.Sistemas.Models.Enlace;
using PortalRoemmers.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; //permite usar landa
using PortalRoemmers.ViewModels;
using System;

namespace PortalRoemmers.Areas.Sistemas.Services.Enlace
{
    public class TipoEnlaceRepositorio
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

                var model = db.tb_TipEnl
                    .OrderBy(x => x.idTEnl).Where(x => x.nomTEnl.Contains(search) || x.desTEnl.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipEnl.Where(x => x.nomTEnl.Contains(search) || x.desTEnl.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.TEnlace = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipoEnlaceModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoEnlaceModels model = db.tb_TipEnl.Find(id);
            return model;
        }
        public Boolean modificar(TipoEnlaceModels model)
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
        public List<TipoEnlaceModels> obtenerTipoEnlance()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_TipEnl.Include(x=>x.Enlaces).OrderBy(x=>x.idTEnl).ToList();
                return model;
            }
        }
    }
}