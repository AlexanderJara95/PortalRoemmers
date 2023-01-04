using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Global
{
    public class MonedaController : Controller
    {//MONEDACONTROLLER 000068
        private MonedaRepositorio _mon;

        public MonedaController()
        {
            _mon = new MonedaRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000069")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _mon.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000070")]
        [HttpGet]
        public ActionResult Registrar()
        {
            
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(MonedaModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                TempData["mensaje"] = _mon.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

            
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000071")]
        public ActionResult Modificar(string id)
        {
            var model = _mon.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(MonedaModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                TempData["mensaje"] = _mon.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000072")]
        public ActionResult Eliminar(string id)
        {
            var model = _mon.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(MonedaModels model)
        {
            TempData["mensaje"] = _mon.eliminar(model.idMon);
            
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}