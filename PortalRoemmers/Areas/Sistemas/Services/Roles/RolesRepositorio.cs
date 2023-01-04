using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System.Linq;
using System.Data.Entity; //permite usar landa
using System.Collections.Generic;
using PortalRoemmers.Areas.Sistemas.Models.Roles;
using System;
using PortalRoemmers.Helpers;

namespace PortalRoemmers.Areas.Sistemas.Services.Roles
{
    public class RolesRepositorio
    {
        Ennumerador enu = new Ennumerador();

        public IndexViewModel obtenerTodos(int pagina, string search, string id)
        {
            int cantidadRegistrosPorPagina = 10;
            if (pagina == 0)
            {
                pagina = 1;
            }
            using (var db = new ApplicationDbContext())
            {
                var roles = db.tb_Roles.Include(x => x.TipRol).Include(y=>y.Parent).OrderBy(x => x.rolId).Where(x => (x.Parent.roltitu.Contains(search) || x.roltitu.Contains(search) || x.TipRol.desTipRol.Contains(search)) && x.ParentId == id)
                                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Roles.Where(x => (x.Parent.roltitu.Contains(search) || x.roltitu.Contains(search)||x.TipRol.desTipRol.Contains(search)) && x.ParentId == id).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Roles = roles;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public string crear(RolesModels model) {
            string mensaje = "";
            using (var db = new ApplicationDbContext())
            {
                //creo su ID
                int idc = enu.buscarTabla("tb_Roles");
                model.rolId = idc.ToString("D6");

                db.tb_Roles.Add(model);
                try {
                    db.SaveChanges();
                    enu.actualizarTabla("tb_Roles", idc);
                    mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                }
                catch (Exception e) {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                }
               
            }
            return mensaje;
        } 
        public RolesModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                RolesModels rol= db.tb_Roles.Find(id);
                return rol;
            }
        }
        public string modificar(RolesModels model)
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
            using (var db = new ApplicationDbContext())
            {
                RolesModels r = db.tb_Roles.Find(id);
                db.tb_Roles.Remove(r);
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

        public List<RolesModels> obtenerRolCondicion(string cod)
        {
            string rolTip = "";

            switch (cod)
            {
                case "02"://Controlador
                    rolTip = ConstantesGlobales.tipRol_Total;//busco solo areas
                    break;
                case "03"://Controlador
                    rolTip = ConstantesGlobales.tipRol_Area;//busco solo areas
                    break;
                case "04"://vista
                    rolTip = ConstantesGlobales.tipRol_Control;//busco solo controladores
                    break;
                default:
                    rolTip = ConstantesGlobales.tipRol_Ninguno;//Ninguno
                    break;
            }

            using (var db = new ApplicationDbContext())
            {
                var rol = db.tb_Roles.Where(x => x.rolTip == rolTip).ToList();
                return rol;
            }
        }
        public List<RolesModels> obtenerTodoRoles()
        {
            var db = new ApplicationDbContext();
            var roles = db.tb_Roles.Include(x=>x.TipRol).ToList();
            return roles;
        }
    }
}