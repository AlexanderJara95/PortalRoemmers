using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Gasto
{
    public class ConceptoGastoRepositorio
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

                var model = db.tb_ConGas
                  .Include(x => x.tipoGasto)
                  .OrderBy(x => x.idConGas).Where(x => x.nomConGas.Contains(search) || x.descConGas.Contains(search) || x.tipoGasto.nomTipGas.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_ConGas.Where(x => x.nomConGas.Contains(search) || x.descConGas.Contains(search) || x.tipoGasto.nomTipGas.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.ConGasto = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public ConceptoGastoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            ConceptoGastoModels model = db.tb_ConGas.Find(id);
            return model;
        }
        public string crear(ConceptoGastoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            //creo su ID
            string tabla = "tb_ConGas";
            int idc = enu.buscarTabla(tabla);
            model.idConGas = idc.ToString();

            db.tb_ConGas.Add(model);
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
        public string modificar(ConceptoGastoModels model)
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

            ConceptoGastoModels model = db.tb_ConGas.Find(id);
            db.tb_ConGas.Remove(model);
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
        public List<ConceptoGastoModels> obtenerConceptoGastos()
        {
            var db = new ApplicationDbContext();
            var cg = db.tb_ConGas.OrderBy(x => x.nomConGas).Where(x=>x.idEst!=ConstantesGlobales.estadoInactivo).ToList();
            return cg;
        }
        public List<ConceptoGastoModels> obtenerConceptoGastosxTipoDeGasto(string idTipGass)
        {
            var db = new ApplicationDbContext();
            var cg = db.tb_ConGas.OrderBy(x => x.nomConGas).Where(x => x.idEst != ConstantesGlobales.estadoInactivo && x.idTipGas== idTipGass).ToList();
            return cg;
        }


    }
}