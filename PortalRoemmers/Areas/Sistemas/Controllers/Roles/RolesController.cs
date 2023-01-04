using PortalRoemmers.Areas.Sistemas.Models.Roles;
using PortalRoemmers.Areas.Sistemas.Services.Roles;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Roles
{
    public class RolesController : Controller
    {//ROLESCONTROLLER 000103
        private RolesRepositorio _rol;
        private Usu_RolRepositorio _usu;
        private TipoRolRepositorio _trol;
        public RolesController()
        {
            _rol = new RolesRepositorio();
            _usu = new Usu_RolRepositorio();
            _trol= new TipoRolRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000104")]
        public ActionResult Index(string menuArea, string menuVista, string id, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _rol.obtenerTodos(pagina, search, id);
            ViewBag.search = search;
            ViewBag.rol = id;
            return View(model);
        }

        //registrar roles
        [HttpGet]
        [CustomAuthorize(Roles = "000003,000105")]
        public ActionResult Registrar(string rol) {
            ViewBag.rol = rol;
            ViewBag.id = new SelectList(_trol.obtenerTipRol(), "idTipRol", "nomTipRol");
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(RolesModels rols, string rol)
        {
            if (ModelState.IsValid)
            {
                rols.usuCrea = SessionPersister.Username;
                rols.usufchCrea = DateTime.Now;

                TempData["mensaje"] = _rol.crear(rols);
                
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search, id = rol });
            }
            ViewBag.id = new SelectList(_trol.obtenerTipRol(), "idTipRol", "nomTipRol");
            return View();
        }
        //modificar rol
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000106")]
        public ActionResult Modificar(string id, string rol)
        {
            ViewBag.rol = rol;
            var rols = _rol.obtenerItem(id);
            ViewBag.id = new SelectList(_trol.obtenerTipRol(), "idTipRol", "nomTipRol", rols.rolTip);
            ViewBag.rolId = new SelectList(_rol.obtenerRolCondicion(rols.rolTip), "rolId", "roltitu", rols.ParentId);
            
            return View(rols);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(RolesModels rols, string rol)
        {
            if (ModelState.IsValid)
            {
                rols.usuMod = SessionPersister.Username;
                rols.usufchMod = DateTime.Now;

                TempData["mensaje"] = _rol.modificar(rols);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search, id = rol });
            }
            ViewBag.id = new SelectList(_trol.obtenerTipRol(), "idTipRol", "nomTipRol", rols.rolTip);
            ViewBag.rolId = new SelectList(_rol.obtenerRolCondicion(rols.rolTip), "rolId", "roltitu", rols.ParentId);
            return View(rols);
        }
        //Eliminar rol
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000107")]
        public ActionResult Eliminar(string id, string rol)
        {
            ViewBag.rol = rol;
            var rols = _rol.obtenerItem(id);
            ViewBag.id = new SelectList(_trol.obtenerTipRol(), "idTipRol", "nomTipRol", rols.rolTip);
            ViewBag.rolId = new SelectList(_rol.obtenerRolCondicion(rols.rolTip), "rolId", "roltitu", rols.ParentId);
            return View(rols);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(RolesModels rols, string rol)
        {
            TempData["mensaje"] = _rol.eliminar(rols.rolId);
            
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search, id = rol });
        }
        //treeview
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000108")]
        public ActionResult PermisosRoles(string id, string usu)
        {

            var seleccionados = _usu.obtenerPermisos(usu);
            var roles = _rol.obtenerTodoRoles().Where(x => x.ParentId == id);


            RolesModels rol = new RolesModels();
            List<RolesModels> rols = new List<RolesModels>();
            foreach (var r in roles)
            {
                rol.rolId = r.rolId;
                rol.rolTip = r.rolTip;
                rol.rolId = r.rolId;
                rol.roltitu = r.roltitu;
                rol.TipRol = r.TipRol;
                rol.Parent = r.Parent;

                if (seleccionados.Contains(r.rolId))
                {
                    rol.option = true;

                }
                else
                {
                    rol.option = false;
                }

                rols.Add(rol);
                rol = new RolesModels();
            }

            ViewBag.usuario = usu;
            return View(rols);
        }

        //combos
        [HttpPost]
        public JsonResult rolesCondicion(string rolTip) {
            return Json(_rol.obtenerRolCondicion(rolTip).Select(x => new { x.rolId, x.roltitu }), JsonRequestBehavior.AllowGet);
        }
        //registrar bd
        [HttpPost]
        public JsonResult RegistrarPermisosUsuarios(string idRol, Boolean selec, string usu)
        {
            var rolArbol =_rol.obtenerTodoRoles().Where(x=>x.ParentId== idRol).Traverse(x=>x.Childs).Select(x=>new {x.rolId});

            Usu_RolModels ur = new Usu_RolModels();
            List<Usu_RolModels> urs = new List<Usu_RolModels>();

            //lleno el primero
            ur.idAcc = usu;
            ur.rolId = idRol;
            ur.usuCrea = SessionPersister.Username;
            ur.usufchCrea = DateTime.Now;
            urs.Add(ur);
            ur = new Usu_RolModels();
            if (rolArbol.Count()!=0)
            {
                foreach (var r in rolArbol)
                {
                    ur.idAcc = usu;
                    ur.rolId = r.rolId;
                    ur.usuCrea = SessionPersister.Username;
                    ur.usufchCrea = DateTime.Now;
                    urs.Add(ur);
                    ur = new Usu_RolModels();
                }
            }
            if (selec)
            {
                _usu.crearUsuRoles(urs);
            }
            else
            {
                _usu.eliminarUsuRoles(urs);
            }


            return Json(rolArbol, JsonRequestBehavior.AllowGet);
        }
    }
}