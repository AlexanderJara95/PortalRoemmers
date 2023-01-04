using PortalRoemmers.Areas.RRHH.Models.Documento;
using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Services.DocumentoRRHH
{
    public class TipoDocumentoRRHHRepositorio
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

                var model = db.tb_TipDocRRHH
                    .OrderBy(x => x.idTipDoc).Where(x => x.nomTipDoc.Contains(search) || x.desTipDoc.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipDocRRHH.Where(x => x.nomTipDoc.Contains(search) || x.desTipDoc.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.TipDocRRHH = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipoDocumentoRRHHModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoDocumentoRRHHModels model = db.tb_TipDocRRHH.Find(id);
            return model;
        }
        public Boolean modificar(TipoDocumentoRRHHModels model)
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
        public List<TipoDocumentoRRHHModels> obtenerTipoEnlance()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_TipDocRRHH.Include(x => x.documentos).OrderBy(x => x.idTipDoc).ToList();
                return model;
            }
        }

    }
}