using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Usuario
{
    public class UbicacionController : Controller
    {//UBICACIONCONTROLLER  000164
        private UbicacionRepositorio _ubi;
        public UbicacionController()
        {
            _ubi = new UbicacionRepositorio();
        }
        [CustomAuthorize(Roles = "000003,000165")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _ubi.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000166")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(UbicacionModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                TempData["mensaje"] = _ubi.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000167")]
        public ActionResult Modificar(string id)
        {
            var model = _ubi.obtenerItem(id);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(UbicacionModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                TempData["mensaje"] = _ubi.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000168")]
        public ActionResult Eliminar(string id)
        {
            var model = _ubi.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(UbicacionModels model)
        {
            TempData["mensaje"] = _ubi.eliminar(model.cCod_Ubi);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        //autocomplete
        [HttpGet]
        public JsonResult busquedaPais(string term)
        {
            return Json(_ubi.busquedaPais(term), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult busquedaDepa(string term,string pais)
        {
            return Json(_ubi.busquedaDepa(term, pais), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult busquedaProv(string term, string pais,string depa)
        {
            return Json(_ubi.busquedaProv(term, pais, depa), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult busquedaDist(string term, string pais, string depa,string prov)
        {
            return Json(_ubi.busquedaDist(term, pais, depa, prov), JsonRequestBehavior.AllowGet);
        }

    }
}