using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using PortalRoemmers.Areas.Sistemas.Models.Gasto;

namespace PortalRoemmers.Areas.Sistemas.Services.Gasto
{
    public class ActividadGastoRepositorio
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
                var model = db.tb_GasAct
                    .Include(x => x.tipoGastoActividad)
                    .OrderBy(x => x.nomActGas)
                    .Where(x => x.nomActGas.Contains(search) || x.descActGas.Contains(search) || x.tipoGastoActividad.nomTipGasAct.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_GasAct.Where(x => x.nomActGas.Contains(search) || x.descActGas.Contains(search) || x.tipoGastoActividad.nomTipGasAct.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.GastosDeActiv = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public ActividadGastoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            ActividadGastoModels model = db.tb_GasAct.Find(id);
            return model;
        }
        public string crear(ActividadGastoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            
            //creo su ID
            string tabla = "tb_GasAct";
            int idc = enu.buscarTabla(tabla);
            model.idActGas = idc.ToString();

            db.tb_GasAct.Add(model);
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
        public string modificar(ActividadGastoModels model)
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

            ActividadGastoModels model = db.tb_GasAct.Find(id);
            db.tb_GasAct.Remove(model);
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
        public List<ActividadGastoModels> obtenerActividadGastos()
        {
            var db = new ApplicationDbContext();
            var cg = db.tb_GasAct.OrderBy(x => x.idActGas).Where(x => x.idEst != ConstantesGlobales.estadoInactivo).ToList();
            return cg;
        }
        //listado
        public List<ActividadGastoModels> obtenerActividadGastosPorTipo(string TipActGas)
        {
            var db = new ApplicationDbContext();
            var cg = db.tb_GasAct.Include(x => x.tipoGastoActividad).OrderBy(x => x.idActGas).Where(x => x.idEst != ConstantesGlobales.estadoInactivo && x.idTipGasAct == TipActGas).ToList();
            return cg;
        }
    }
}