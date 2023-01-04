using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Visitador;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Visitador
{
    public class ZonaController : Controller
    {//ZONACONTROLLER   000186
        private ZonaRepositorio _zon;
        private EstadoRepositorio _est;
        public ZonaController()
        {
            _zon = new ZonaRepositorio();
            _est = new EstadoRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000187")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _zon.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000188")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.estadoZ = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst",ConstantesGlobales.estadoActivo);
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(ZonaModels model)
        {
            if (ModelState.IsValid)
            {
                //agrego auditoria
                model.fchCreZon = DateTime.Now;
                model.userCreZon = SessionPersister.Username;
                TempData["mensaje"] = _zon.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.estadoZ = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst",model.idEst);
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000189")]
        public ActionResult Modificar(string id)
        {
            var model = _zon.obtenerItem(id);
            ViewBag.estadoZ = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(ZonaModels model)
        {
            if (ModelState.IsValid)
            {
                //agrego auditoria
                model.fchModZon = DateTime.Now;
                model.userModZon = SessionPersister.Username;
                TempData["mensaje"] = _zon.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.estadoZ = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000190")]
        public ActionResult Eliminar(string id)
        {
            var model = _zon.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(ZonaModels model)
        {
            TempData["mensaje"] = _zon.eliminar(model.idZon);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}