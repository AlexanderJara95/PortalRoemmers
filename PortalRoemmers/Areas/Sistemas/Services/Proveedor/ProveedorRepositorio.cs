using PortalRoemmers.Areas.Sistemas.Models.Proveedor;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Proveedor
{
    public class ProveedorRepositorio
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

                var model = db.tb_Proveedor
                  .OrderBy(x => x.idPro).Where(x => x.nomProv.Contains(search) || x.niffPro.Contains(search))
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Proveedor.OrderBy(x => x.idPro).Where(x => x.nomProv.Contains(search) || x.niffPro.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Proveedores = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public List<ProveedorModels> obtenerLaboratorio()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Proveedor.Where(x=>x.idEst==ConstantesGlobales.estadoActivo).OrderBy(x => x.nomProv).ToList();
            return model;
        }
        public ProveedorModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            ProveedorModels model = db.tb_Proveedor.Find(id);
            return model;
        }
        public Boolean crear(ProveedorModels model, out string mensaje)
        {
            //creo su ID
            string tabla = "tb_Proveedor";
            string id = enu.buscarTabla(tabla).ToString();
            model.idPro = id;

            //string mensaje = "";
            bool resultado = false;
            var db = new ApplicationDbContext();
            model.idEst = ConstantesGlobales.estadoActivo;

            //Validar que no exista otro igual en Cuenta AX o NiifPro
            var validacion1 = db.tb_Proveedor.Where(x => x.cuentaAX == model.cuentaAX).FirstOrDefault();
            if (validacion1 != null && validacion1.cuentaAX != 0)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>La cuenta Ax se repite en otro Proveedor</div>";
                return resultado;
            }
            var validacion2 = db.tb_Proveedor.Where(x => x.niffPro== model.niffPro).FirstOrDefault();
            if (validacion2 != null)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>El RUC o Niif se repite en otro Proveedor</div>";
                return resultado;
            }

            db.tb_Proveedor.Add(model);
            try
            {
                db.SaveChanges();
                enu.actualizarTabla(tabla, int.Parse(id));
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                resultado = true;
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                resultado = false;
            }
            return resultado;
        }
        public string modificar(ProveedorModels model)
        {
            string mensaje = "";

            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                //Validar que no exista otro igual en Cuenta AX o NiifPro
                var validacion1 = db.tb_Proveedor.Where(x => x.cuentaAX == model.cuentaAX).ToList();
                if (validacion1 != null && validacion1.Count() == 1)
                {
                    return mensaje = "<div id='warning' class='alert alert-warning'>La cuenta Ax se repite en otro Proveedor</div>";
                     //resultado;
                }
                var validacion2 = db.tb_Proveedor.Where(x => x.niffPro == model.niffPro).ToList();
                if (validacion2 != null && validacion2.Count() == 1)
                {
                    return mensaje = "<div id='warning' class='alert alert-warning'>El RUC o Niif se repite en otro Proveedor</div>";
                     //resultado;
                }
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

            ProveedorModels model = db.tb_Proveedor.Find(id);
            db.tb_Proveedor.Remove(model);
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
        public List<ProveedorModels> obtenerProveedores()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Proveedor.OrderBy(x => x.nomProv).Where(x => x.idEst != ConstantesGlobales.estadoInactivo).ToList();
            return model;
        }
    }
}