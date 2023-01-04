using PortalRoemmers.Areas.Sistemas.Models.Medico;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Medico
{
    public class TipMedRepositorio
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

                var model = db.tb_TipMed
                  .OrderBy(x => x.idTipCli).Where(x => x.nomTipCli.Contains(search) || x.desTipCli.Contains(search))
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_TipMed.Where(x => x.nomTipCli.Contains(search) || x.desTipCli.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.TipCli = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public TipoMedicoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            TipoMedicoModels model = db.tb_TipMed.Find(id);
            return model;
        }
        public string crear(TipoMedicoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_TipCli";
            int idc = enu.buscarTabla(tabla);
            model.idTipCli = idc.ToString();

            db.tb_TipMed.Add(model);
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
        public string modificar(TipoMedicoModels model)
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

            TipoMedicoModels model = db.tb_TipMed.Find(id);
            db.tb_TipMed.Remove(model);
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
        public List<TipoMedicoModels> obtenerTipoCliente()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_TipMed.OrderBy(x => x.nomTipCli).ToList();
            return model;
        }

    }
}