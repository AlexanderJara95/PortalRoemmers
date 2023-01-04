using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Areas.Sistemas.Services;
using PortalRoemmers.Areas.Sistemas.Services.Producto;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Producto
{
    public class AreaTerapeuticaController : Controller
    {//AREA TERAPEUTICA CONTROLLER
        private AreaTerapeuticaRepositorio _areaTer;
        public AreaTerapeuticaController()
        {
            _areaTer = new AreaTerapeuticaRepositorio();
        }
        //INDEX
        [CustomAuthorize(Roles = "000003")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _areaTer.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //REGISTRAR
        [CustomAuthorize(Roles = "000003")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(AreaTerapeuticaModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                TempData["mensaje"] = _areaTer.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

            return View(model);
        }
        //MODIFICAR
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003")]
        public ActionResult Modificar(string id)
        {
            var model = _areaTer.obtenerItem(id);

            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(AreaTerapeuticaModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                TempData["mensaje"] = _areaTer.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

            return View(model);
        }
        //ELIMINAR
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003")]
        public ActionResult Eliminar(string id)
        {
            var model = _areaTer.obtenerItem(id);

            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(AreaTerapeuticaModels model)
        {
            TempData["mensaje"] = _areaTer.eliminar(model.idAreaTerap);

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}