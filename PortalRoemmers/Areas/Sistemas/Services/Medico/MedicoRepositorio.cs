using PortalRoemmers.Areas.Sistemas.Models.Medico;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Medico
{
    public class MedicoRepositorio
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

                var model = db.tb_Medico
                  .OrderBy(x => x.idTipCli).Where(x => x.nomCli.Contains(search) || x.nroDocCli.Contains(search) ||x.nroDocCli.Contains(search) || x.nroMatCli.Contains(search))
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Medico.Where(x => x.nomCli.Contains(search) || x.nroDocCli.Contains(search) || x.nroDocCli.Contains(search) || x.nroMatCli.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Clientes = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public MedicoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            MedicoModels model = db.tb_Medico.Find(id);
            return model;
        }
        public string crear(MedicoModels model)
        {
            //creo su ID
            string tabla = "tb_Medico";
            string id = enu.buscarTabla(tabla).ToString();
            model.idCli = id;

            string mensaje = "";
            var db = new ApplicationDbContext();
            model.idEst = ConstantesGlobales.estadoActivo;

            db.tb_Medico.Add(model);
            try
            {
                db.SaveChanges();
                enu.actualizarTabla(tabla, int.Parse(id));
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(MedicoModels model)
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

            MedicoModels model = db.tb_Medico.Find(id);
            db.tb_Medico.Remove(model);
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
        public List<MedicoModels> obtenerClientes()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Medico.OrderBy(x => x.nomCli).Where(x=>x.idEst!=ConstantesGlobales.estadoInactivo).ToList();
            return model;
        }
        public MedicoModels obtenerItemxCMP(string cmp)
        {
            //string medico="";
            var db = new ApplicationDbContext();
            MedicoModels model = db.tb_Medico.Where(x=>x.nroMatCli == cmp).FirstOrDefault();
            /*if (model != null)
            {
                medico = model.nomCli;
            }*/
            return model;
        }

    }
}