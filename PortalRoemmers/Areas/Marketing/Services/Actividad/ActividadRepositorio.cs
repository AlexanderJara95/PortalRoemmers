using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PortalRoemmers.Models;
using PortalRoemmers.Helpers;
using System.Data.Entity;
using PortalRoemmers.Areas.Marketing.Models.Actividad;
using PortalRoemmers.Areas.Marketing.Services.Estimacion;

namespace PortalRoemmers.Areas.Marketing.Services.Actividad
{
    public class ActividadRepositorio
    {
        EstimacionRepositorio _est = new EstimacionRepositorio();
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search, string primero, string actual)
        {
            //------------------------------------------------------------------------
            DateTime p = DateTime.Parse(primero); //desde
            DateTime a = DateTime.Parse(actual);//hasta
            //------------------------------------------------------------------------
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }

            using (var db = new ApplicationDbContext())
            {

                var model = db.tb_Activ
                    .Include(x=>x.especialidad)
                    .OrderByDescending(x => x.idActiv)
                    .Where(x => (x.nomActiv.Contains(search) || x.desActiv.Contains(search) || x.idActiv.Contains(search) || x.especialidad.nomEsp.Contains(search)) &&  (x.fchIniActiv >= p) &&  (x.fchFinActiv <= a))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Activ.Where(x => (x.nomActiv.Contains(search) || x.desActiv.Contains(search) || x.idActiv.Contains(search) || x.especialidad.nomEsp.Contains(search)) && (x.fchIniVig >= p) && (x.fchFinVig <= a)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Actividades = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public ActividadModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            ActividadModels model = db.tb_Activ.Find(id);
            return model;
        }
        public Boolean crear(ActividadModels model)
        {
            Boolean mensaje = false;
            var db = new ApplicationDbContext();
            db.tb_Activ.Add(model);
            try
            {
                db.SaveChanges();
                mensaje = true;
            }
            catch (Exception e)
            {
                mensaje = false;
            }
            return mensaje;
        }
        public string modificar(ActividadModels model)
        {
            string mensaje = "";
            var activMof = _est.obtenerItem(model.idActiv);
            if(model.idEst == ConstantesGlobales.estadoInactivo && activMof != null)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + "No se puede modificar el estado, ya se encuentra en uso." + "</div>";
            }
            else
            {
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
            }
            return mensaje;
        }
        //
        public string eliminar(string id)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            
            ActividadModels model = db.tb_Activ.Find(id);

            if (model.idEst == ConstantesGlobales.estadoInactivo)
            {
                db.tb_Activ.Remove(model);
                try
                {
                    db.SaveChanges();
                    mensaje = "<div id='success' class='alert alert-success'>Se eliminó un registro.</div>";
                }
                catch (Exception e)
                {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                }
            }
            else
            {
                mensaje = "<div id='success' class='alert alert-success'>El registro no se puede eliminar debido al estado ACTIVO.</div>";
            }
            return mensaje;
        }
        //listados
        public List<ActividadModels> obtenerActividades()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Activ.Include(x=>x.estimacion).OrderBy(x => x.nomActiv).Where(x => x.idEst != ConstantesGlobales.estadoInactivo).ToList();
            return model;
        }
        //Obtener Detalle de Actividad
        public ActividadModels obtenerItemDetalle(string id)
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Activ.Include(y=>y.dMed).Where(x=>x.idActiv == id).FirstOrDefault();
            return model;
        }
        public Boolean guardarDetalleActMedicos(List<DetActiv_MedModels> listaActual, string idAct ,out string mensaje)
        {
            //string mensaje = "";
            if (listaActual.Count() != 0)
            {
                //string mensaje = "";
                //////////---------------------//////////
                using (var db = new ApplicationDbContext())
                {
                    //////////---------------------//////////
                    var listaAnterior = db.tb_DetAct_Med.Where(x => x.idActiv == idAct).ToList();
                    if (listaAnterior != null)
                    {
                        //////////---------------------//////////
                        var listaDeEliminados = listaAnterior.Where(x => !listaActual.Any(y => y.idCli == x.idCli)).ToList();
                        //////////---------------------//////////
                        var listaDeAgregados = listaActual.Where(y => !listaAnterior.Any(x => x.idCli == y.idCli)).ToList();
                        //////////---------------------//////////
                        //////////---------------------//////////
                        if (listaDeEliminados.Count != 0)
                        {
                            db.tb_DetAct_Med.RemoveRange(listaDeEliminados);
                        }
                        if (listaDeAgregados.Count != 0)
                        {
                            db.tb_DetAct_Med.AddRange(listaDeAgregados);
                        }
                    }
                    else
                    {
                        db.tb_DetAct_Med.AddRange(listaActual);

                    }
                    //////////---------------------//////////
                    try
                    {
                        db.SaveChanges();
                        mensaje = "Exito";
                        return true;
                    }
                    catch (Exception e)
                    {
                        mensaje = "Error: " + e.Message;
                        return false;
                    }
                    //////////---------------------//////////

                }
            }
            else
            {
                mensaje = "<div id='success' class='alert alert-success'>No se hay medicos a agregar.</div>";
                return false;
            }

        }
    }
}