using System;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using System.Collections.Generic;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System.Linq;
using System.Data.Entity;

namespace PortalRoemmers.Areas.Sistemas.Services.Global
{
    public class ParametroRepositorio
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

                var model = db.tb_Parametro
                  .OrderBy(x => x.idPar).Where(x => x.nomPar.Contains(search) || x.rutPar.ToString().Contains(search) || x.idPar.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Parametro.Where(x => x.nomPar.Contains(search) || x.rutPar.ToString().Contains(search) || x.idPar.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Parametros = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public string crear(ParametroModels model)
        {
            string mensaje = "";
            //creo su ID
            string tabla = "tb_Parametro";
            int idc = enu.buscarTabla(tabla);
            model.idPar = idc.ToString("D8");
            using (var db = new ApplicationDbContext())
            {
                db.tb_Parametro.Add(model);
                try
                {
                    db.SaveChanges();
                    enu.actualizarTabla(tabla, idc);
                    mensaje = idc.ToString("D8");
                }
                catch (Exception e)
                {
                    mensaje = "error";
                }
            }
            return mensaje;
        }
        public string crearDetallePar(List<ParDetalleModels> model,string codPar)
        {
            string mensaje = "";
            //Crear ID para cada Detalle
            foreach(var i in model)
            {
                string tabla = "tb_DetPar" + codPar;
                int idc = enu.buscarTabla(tabla);
                enu.actualizarTabla(tabla, idc);
                i.idDetPar = idc.ToString("D7");
            }
            //Guardar en la BD
            using (var db = new ApplicationDbContext())
            {
                db.tb_DetPar.AddRange(model);
                try
                {
                    db.SaveChanges();
                    mensaje = "<div id='success' class='alert alert-success'>Se modificó el registro.</div>";
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                    mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                }
            }
            return mensaje;
        }
        public ParametroModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            ParametroModels model = db.tb_Parametro.Include(x=>x.detPar).Where(x=>x.idPar==id).SingleOrDefault();
            return model;
        }
        public string modificar(ParametroModels model)
        {
            string mensaje = "";

            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    mensaje = model.idPar.ToString();
                }
                catch (Exception e)
                {
                    mensaje = "error";
                }
            }
            return mensaje;
        }
        public string modificaDetallePar(List<ParDetalleModels> listaActual, string codPar)
        {
            string mensaje = "";

            //////////---------------------//////////

            using (var db = new ApplicationDbContext())
            {
                var listaAnterior = db.tb_DetPar.Where(x => x.idPar.Contains(codPar)).ToList();
                //////////---------------------//////////
                //var result = listaAnterior.Where(a => model.Exists(b => b.idDetPar.Contains(a.idDetPar)));
                var listaDeEliminados = listaAnterior.Where(x => !listaActual.Any(y => y.idDetPar == x.idDetPar)).ToList();
                //////////---------------------//////////
                var listaDeAgregados = listaActual.Where(y => !listaAnterior.Any(x => x.idDetPar == y.idDetPar)).ToList();
                //////////---------------------//////////
                //Agregar ID a la lista Detalle
                if (listaDeAgregados.Count != 0)
                {
                    foreach (var k in listaDeAgregados)
                    {
                        string tabla = "tb_DetPar" + codPar;
                        int idc = enu.buscarTabla(tabla);
                        enu.actualizarTabla(tabla, idc);
                        //------------------------------
                        k.idDetPar = idc.ToString("D7");
                    }
                }
                //////////---------------------//////////
                if (listaDeEliminados.Count!=0)
                {
                    db.tb_DetPar.RemoveRange(listaDeEliminados); 
                }
                if(listaDeAgregados.Count != 0)
                {
                    db.tb_DetPar.AddRange(listaDeAgregados);
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
                //////////---------------------//////////
            }
            return mensaje;
        }
        public string eliminar(string id)
        {
            //--------------------------------------------
            string mensaje = "";
            //var db = new ApplicationDbContext();
            //--------------------------------------------
            using (var db = new ApplicationDbContext())
            {
                ParametroModels padre = db.tb_Parametro.Find(id);
                var lista = db.tb_DetPar.Where(x => x.idPar.Contains(id)).ToList();
                db.tb_DetPar.RemoveRange(lista);
                db.tb_Parametro.Remove(padre);
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
            return mensaje;
        }
    }
}