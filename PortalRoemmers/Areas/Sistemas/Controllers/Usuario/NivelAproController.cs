using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Usuario
{
    public class NivelAproController : Controller
    {//NIVELAPROCONTROLLER  000149
        private NivelAproRepositorio _niv;
        public NivelAproController()
        {
            _niv = new NivelAproRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000150")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _niv.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000151")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(NivelAproModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                TempData["mensaje"] = _niv.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000152")]
        public ActionResult Modificar(string id)
        {
            var model = _niv.obtenerItem(id);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(NivelAproModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;
                TempData["mensaje"] = _niv.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000153")]
        public ActionResult Eliminar(string id)
        {
            var model = _niv.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(NivelAproModels model)
        {
            TempData["mensaje"] = _niv.eliminar(model.idNapro);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}