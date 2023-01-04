using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Global
{
    public class EstadoRepositorio
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

                var model = db.tb_Estado
                  .OrderBy(x => x.idEst).Where(x => x.nomEst.Contains(search) || x.desEst.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Estado.Where(x => x.nomEst.Contains(search) || x.desEst.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Estado = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public EstadoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            EstadoModels model = db.tb_Estado.Find(id);
            return model;
        }
        public string crear(EstadoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            //creo su ID
            string tabla = "tb_Estado";
            int idc = enu.buscarTabla(tabla);
            model.idEst = idc.ToString();

            db.tb_Estado.Add(model);
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
        public string modificar(EstadoModels model)
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

            EstadoModels model = db.tb_Estado.Find(id);
            db.tb_Estado.Remove(model);
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
        public List<EstadoModels> obtenerEstadoEquipo()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("EQUIPO")).ToList();
            return model;
        }
        public List<EstadoModels> obteneEstadoUsuario()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("USUARIO")).ToList();
            return model;
        }
        public List<EstadoModels> obteneEstadoGlobal()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("GLOBAL")).ToList();
            return model;
        }
        public List<EstadoModels> obteneEstadoLetra()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("B_LETRA")).ToList();
            return model;
        }
        public List<EstadoModels> obteneEstadoProducto()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("PRODUCTO")).ToList();
            return model;
        }
        public List<EstadoModels> obteneEstadoContabilizar()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("FCONTABILIZAR")).ToList();
                return model;
            }
        }
        public List<EstadoModels> obteneEstadoSegunAprob()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("GF")).ToList();
                return model;
            }
        }
        public List<EstadoModels> obteneEstadoLiquidarFiltro()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("FLIQUIDA")).ToList();
                return model;
            }
        }

        public List<EstadoModels> obteneEstadoLiquidar()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("LIQUIDAR")).ToList();
                return model;
            }
        }
        public List<EstadoModels> obteneEstadosGasto()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("GASTO")).ToList();
                return model;
            }
        }
        public List<EstadoModels> obteneEstadosAcambiar()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("CAMBIO")).ToList();
                return model;
            }
        }
        public List<EstadoModels> obteneEstadosAprobarxAdm()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("APR_ADM")).ToList();
                return model;
            }
        }

        public List<EstadoModels> obteneEstadosDeposito()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Estado.OrderBy(x => x.nomEst).Where(x => x.modEst.Contains("FDEPOSITO")).ToList();
                return model;
            }
        }

    }
}