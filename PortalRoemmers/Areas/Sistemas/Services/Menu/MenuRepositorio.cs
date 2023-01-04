using PortalRoemmers.Areas.Sistemas.Models.Menu;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity; //permite usar landa
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Menu
{
    public class MenuRepositorio
    {
        Ennumerador enu = new Ennumerador();

        public IndexViewModel obtenerTodos(int pagina, string search,string id)
        {
            int cantidadRegistrosPorPagina = 10;
            if (pagina == 0)
            {
                pagina = 1;
            }
            using (var db = new ApplicationDbContext())
            {
                var menu = db.tb_Menu.Include(x => x.tipMenu).Include(y => y.Parent).OrderBy(x =>new { x.tipMenu.desTipMen, x.tiMen }).Where(x => (x.Parent.tiMen.Contains(search) || x.tiMen.Contains(search) || x.tipMenu.desTipMen.Contains(search)) && x.ParentId==id)
                                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Menu.Where(x => (x.Parent.tiMen.Contains(search) || x.tiMen.Contains(search) || x.tipMenu.desTipMen.Contains(search)) && x.ParentId == id).Count();

                var modelo = new IndexViewModel();
                modelo.Menu = menu;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public string crear(MenuModels model)
        {
            string mensaje = "";
            using (var db = new ApplicationDbContext())
            {
                //creo su ID
                string tabla = "tb_Menu";
                int idc = enu.buscarTabla(tabla);
                model.idMen = idc.ToString("D6");

                db.tb_Menu.Add(model);
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

            }
            return mensaje;
        }
        public MenuModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                MenuModels rol = db.tb_Menu.Find(id);
                return rol;
            }
        }
        public string modificar(MenuModels model)
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
                MenuModels m = db.tb_Menu.Find(id);
                db.tb_Menu.Remove(m);
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

        public List<MenuModels> obtenerMenuCondicion(string cod)
        {
            String[] menuTip = new string[4];

            switch (cod)
            {
                case "02"://sub 1
                    menuTip[0] = ConstantesGlobales.tMenu_Plantilla;//busco solo 
                    break;
                case "03"://sub 2
                    menuTip[0] = ConstantesGlobales.tMenu_Menu;//busco solo 
                    break;
                case "04"://vista 
                    menuTip[0] = ConstantesGlobales.tMenu_Plantilla;//Ninguno
                    menuTip[1] = ConstantesGlobales.tMenu_Menu;//Ninguno
                    menuTip[2] = ConstantesGlobales.tMenu_Sub;//Ninguno
                    break;
                case "05"://LINK PARAMETRO
                    menuTip[0] = ConstantesGlobales.tMenu_Plantilla;//Ninguno
                    menuTip[1] = ConstantesGlobales.tMenu_Menu;//Ninguno
                    menuTip[2] = ConstantesGlobales.tMenu_Sub;//Ninguno
                    break;
            }

            using (var db = new ApplicationDbContext())
            {
                var m = (from p in db.tb_Menu
                           where menuTip.Contains(p.idTipMen)
                           select p).Include(x=>x.Parent).Include(x => x.Parent.Parent).Include(x => x.tipMenu).Include(x => x.Parent.tipMenu).Include(x => x.Parent.Parent.tipMenu).ToList();
                return m;
            }
        }

        public List<MenuModels> obtenerMenus()
        {
            var db = new ApplicationDbContext();
            var m =db.tb_Menu.ToList();
            return m;
        }

    }
}