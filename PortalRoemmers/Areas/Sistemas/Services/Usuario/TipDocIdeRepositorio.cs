using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class TipDocIdeRepositorio
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

                var model = db.tb_DocIde
                  .OrderBy(x => x.idTipDoc).Where(x => x.nomTipDoc.Contains(search) || x.desTipDoc.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_DocIde.Where(x => x.nomTipDoc.Contains(search) || x.nomTipDoc.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.T_Doc = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipDocIdeModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipDocIdeModels model = db.tb_DocIde.Find(id);
            return model;
        }
        public string crear(TipDocIdeModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            //creo su ID
            string tabla = "tb_TipDocIde";
            int idc = enu.buscarTabla(tabla);
            model.idTipDoc = idc.ToString();


            db.tb_DocIde.Add(model);
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
        public string modificar(TipDocIdeModels model)
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

            TipDocIdeModels model = db.tb_DocIde.Find(id);
            db.tb_DocIde.Remove(model);
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
        public List<TipDocIdeModels> obtenerTipDocs()
        {
            using (var db = new ApplicationDbContext())
            {
                //  var tipDocs = db.tb_DocIde.Where(x => x.estTipDoc == 1).OrderBy(x => x.idTipDoc).Select(h => new SelectListItem { Value = h.idTipDoc.ToString(), Text = h.desTipDoc }).ToList();
                var tipDocs = db.tb_DocIde.OrderBy(x => x.idTipDoc).ToList();
                return tipDocs;
            }
        }
    }
}