using PortalRoemmers.Areas.Sistemas.Models.Menu;
using PortalRoemmers.Areas.Sistemas.Services.Menu;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Menu
{
    public class MenuController : Controller
    {// MENUCONTROLLER 000194
        private MenuRepositorio _men;
        private TipoMenuRepositorio _tmen;
        private UsuarioRepositorio _usus;
        private TipoIconRepositorio _ticon;
        private IconRepositorio _icon;

        public MenuController()
        {
            _men = new MenuRepositorio();
            _tmen = new TipoMenuRepositorio();
            _usus = new UsuarioRepositorio();
            _ticon = new TipoIconRepositorio();
            _icon = new IconRepositorio();
        }

        // GET: Sistemas/Roles

        [CustomAuthorize(Roles = "000003,000195")]
        public ActionResult Index(string menuArea, string menuVista, string id, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _men.obtenerTodos(pagina, search,id);
            ViewBag.search = search;
            ViewBag.rol = id;
            return View(model);
        }

        //registrar roles
        [HttpGet]
        [CustomAuthorize(Roles = "000003,000196")]
        public ActionResult Registrar(string rol)
        {
            ViewBag.rol = rol;
            ViewBag.id = new SelectList(_tmen.obtenerTipoMenu(), "idTipMen", "nomTipMen");
            ViewBag.tipIcon = new SelectList(_ticon.obtenerTipoIcon(), "idTipIco", "nomTipIco");
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(MenuModels men, string rol)
        {
            if (ModelState.IsValid)
            {
                men.usuCrea = SessionPersister.Username;
                men.usufchCrea = DateTime.Now;

                TempData["mensaje"] = _men.crear(men);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search, id = rol });
            }
            ViewBag.tipIcon = new SelectList(_ticon.obtenerTipoIcon(), "idTipIco", "nomTipIco");
            ViewBag.id = new SelectList(_tmen.obtenerTipoMenu(), "idTipMen", "nomTipMen");
            return View();
        }
        //modificar rol
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000197")]
        public ActionResult Modificar(string id, string rol)
        {
            ViewBag.rol = rol;
            var m = _men.obtenerItem(id);
            ViewBag.id = new SelectList(_tmen.obtenerTipoMenu(), "idTipMen", "nomTipMen", m.idTipMen);
            ViewBag.tipIcon = new SelectList(_ticon.obtenerTipoIcon(), "idTipIco", "nomTipIco");
            ViewBag.menu = new SelectList(_men.obtenerMenuCondicion(m.idTipMen).Select(x =>
            new {
                x.idMen,
                tiMen =
                    x.ParentId == null ? x.tipMenu.nomTipMen.ToUpper() + "(" + x.tiMen.ToUpper() + ")" + " - " + " (Vacio Padre)" :
                    x.Parent.ParentId == null ? x.tipMenu.nomTipMen.ToUpper() + "(" +x.tiMen.ToUpper()+")" + " - "+ x.Parent.tipMenu.nomTipMen + " (" + x.Parent.tiMen.ToUpper() + ")" + " - (Vacio Abuelo) " :
                    x.tipMenu.nomTipMen.ToUpper() + "(" + x.tiMen.ToUpper()+")" + " - " + x.Parent.tipMenu.nomTipMen + "(" + x.Parent.tiMen.ToUpper() + ")" +" - "+ x.Parent.Parent.tipMenu.nomTipMen + " (" + x.Parent.Parent.tiMen.ToUpper() + ")"
            })
            , "idMen", "tiMen", m.ParentId);
           
            return View(m);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(MenuModels m, string rol)
        {
            if (ModelState.IsValid)
            {
                m.usuMod = SessionPersister.Username;
                m.usufchMod = DateTime.Now;

                TempData["mensaje"] = _men.modificar(m);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search, id = rol });
            }
            ViewBag.tipIcon = new SelectList(_ticon.obtenerTipoIcon(), "idTipIco", "nomTipIco");
            ViewBag.id = new SelectList(_tmen.obtenerTipoMenu(), "idTipMen", "nomTipMen", m.idTipMen);
            ViewBag.menu = new SelectList(_men.obtenerMenuCondicion(m.idTipMen).Select(x =>
            new {
                x.idMen,
                tiMen =
                    x.ParentId == null ? x.tipMenu.nomTipMen.ToUpper() + "(" + x.tiMen.ToUpper() + ")" + " - " + " (Vacio Padre)" :
                    x.Parent.ParentId == null ? x.tipMenu.nomTipMen.ToUpper() + "(" + x.tiMen.ToUpper() + ")" + " - " + x.Parent.tipMenu.nomTipMen + " (" + x.Parent.tiMen.ToUpper() + ")" + " - (Vacio Abuelo) " :
                    x.tipMenu.nomTipMen.ToUpper() + "(" + x.tiMen.ToUpper() + ")" + " - " + x.Parent.tipMenu.nomTipMen + "(" + x.Parent.tiMen.ToUpper() + ")" + " - " + x.Parent.Parent.tipMenu.nomTipMen + " (" + x.Parent.Parent.tiMen.ToUpper() + ")"
            })
            , "idMen", "tiMen", m.ParentId);
            return View(m);
        }
        //Eliminar rol
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000198")]
        public ActionResult Eliminar(string id, string rol)
        {
            ViewBag.rol = rol;
            var men = _men.obtenerItem(id);
            ViewBag.id = new SelectList(_tmen.obtenerTipoMenu(), "idTipMen", "nomTipMen", men.idTipMen);
            return View(men);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(MenuModels men, string rol)
        {
            TempData["mensaje"] = _men.eliminar(men.idMen);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search, id = rol });
        }
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000206")]
        public ActionResult MenuUsuarios(string id, string nom)
        {
            var users = _usus.obtenerUsuarios();

            ViewBag.usuario = new SelectList(users.Where(x => (x.idMen == null && x.idAcc != id) && x.idAcc != id).Select(x => new { x.idAcc, nombre = x.empleado.nomComEmp+" ("+x.username+")"  }), "idAcc", "nombre");
            ViewBag.usuarioA = new SelectList(users.Where(x => x.idMen == id && x.idAcc != id).Select(x => new { x.idAcc, nombre = x.empleado.nomComEmp + " (" + x.username + ")" }), "idAcc", "nombre");
            ViewBag.id = id;
            ViewBag.menu = nom;

            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult AddUsers(string[] idAcc, string id, string nombre)
        {
            string codigo = id;
            _usus.agregarMenuUsuario(idAcc, codigo);

            return RedirectToAction("MenuUsuarios", new { id = codigo, nom = nombre });
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult DelUsers(string[] idAccA, string id, string nombre)
        {
            string codigo = id;
            _usus.eliminarMenuUsuario(idAccA);

            return RedirectToAction("MenuUsuarios", new { id = codigo, nom = nombre });
        }
        [ChildActionOnly]
        public ActionResult _MenuLayout()
        {
            List<MenuModels> menu = (List<MenuModels>)System.Web.HttpContext.Current.Session[Sessiones.menu];
            return PartialView(menu);
        }
        //combos///////////////////////////////////////////////////////////////////
        [HttpPost]
        public JsonResult menuCondicion(string idTipMen)
        {
            return Json(_men.obtenerMenuCondicion(idTipMen).Select(x =>
            new {
                x.idMen,
                tiMen =
                    x.ParentId == null ? x.tipMenu.nomTipMen.ToUpper() + "(" + x.tiMen.ToUpper() + ")" + " - " + " (Vacio Padre)" :
                    x.Parent.ParentId == null ? x.tipMenu.nomTipMen.ToUpper() + "(" + x.tiMen.ToUpper() + ")" + " - " + x.Parent.tipMenu.nomTipMen + " (" + x.Parent.tiMen.ToUpper() + ")" + " - (Vacio Abuelo) " :
                    x.tipMenu.nomTipMen.ToUpper() + "(" + x.tiMen.ToUpper() + ")" + " - " + x.Parent.tipMenu.nomTipMen + "(" + x.Parent.tiMen.ToUpper() + ")" + " - " + x.Parent.Parent.tipMenu.nomTipMen + " (" + x.Parent.Parent.tiMen.ToUpper() + ")"
            })
            , JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult cargarIcons(int idTipIco)
        {
            return Json(_icon.obtenerIcon().Where(x=>x.idTipIco== idTipIco).Select(x=>new { x.nomIco ,x.idIco}), JsonRequestBehavior.AllowGet);
        }
    }
}