using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using PortalRoemmers.Areas.RRHH.Models.Documento;

namespace PortalRoemmers.Areas.RRHH.Services.DocumentoRRHH
{
    public class DocumentoRRHHRepositorio
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
                var model = db.tb_DocRRHH
                .Include(x => x.tipodocumento)
                .OrderBy(x => x.ordDoc).Where(x => x.nomDoc.Contains(search) || x.desDoc.Contains(search) || x.tipodocumento.nomTipDoc.Contains(search))
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.tb_DocRRHH.OrderBy(x => x.ordDoc).Include(x => x.tipodocumento).Where(x => x.nomDoc.Contains(search) || x.desDoc.Contains(search) || x.tipodocumento.nomTipDoc.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.DocRRHH = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public Boolean crear(DocumentoRRHHModels model)
        {
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.tb_DocRRHH.Add(model);
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
        public DocumentoRRHHModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                DocumentoRRHHModels model = db.tb_DocRRHH.Include(x => x.tipodocumento).Where(x => x.idDoc == id).FirstOrDefault();
                return model;
            }

        }
        public Boolean modificar(DocumentoRRHHModels model)
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
                DocumentoRRHHModels model = db.tb_DocRRHH.Find(id);
                db.tb_DocRRHH.Remove(model);
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

        public List<DocumentoRRHHModels> obtenerEnlances()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_DocRRHH.OrderBy(x => x.ordDoc).ToList();
                return model;
            }
        }

    }
}