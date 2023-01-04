using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Areas.Sistemas.Services.Equipo;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Equipo
{
    public class TipoEController : Controller
    {//TIPOECONTROLLER  000043
        private TipoERepositorio _tip;
        public TipoEController()
        {
            _tip = new TipoERepositorio();
        }

        [CustomAuthorize(Roles = "000003,000044")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tip.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000045")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(TipoEquipoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                TempData["mensaje"] = _tip.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000046")]
        public ActionResult Modificar(string id)
        {
            var model = _tip.obtenerItem(id);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(TipoEquipoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                TempData["mensaje"] = _tip.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000047")]
        public ActionResult Eliminar(string id)
        {
            var model = _tip.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(TipoEquipoModels model)
        {
            TempData["mensaje"] = _tip.eliminar(model.idTipEqui);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}