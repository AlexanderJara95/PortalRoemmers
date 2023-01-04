using System;
using System.Web.Mvc;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Sistemas.Services.Gasto;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Gasto
{
    public class TipGastDeActivController : Controller
    {//TIPOGASTOACTIVIDADCONTROLLER 00000
        private TipGastDeActivRepositorio _tgasact;
        public TipGastDeActivController()
        {
            _tgasact = new TipGastDeActivRepositorio();
        }
        // Listar y Busqueda
        [CustomAuthorize(Roles = "000003,000285")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tgasact.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000286")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(TipGastDeActivModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchCreTipGastActiv = DateTime.Now;
                model.userCreTipGastActiv = SessionPersister.Username;
                TempData["mensaje"] = _tgasact.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000287")]
        public ActionResult Modificar(string id)
        {
            var model = _tgasact.obtenerItem(id);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(TipGastDeActivModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchModTipGastActiv = DateTime.Now;
                model.userModTipGastActiv = SessionPersister.Username;
                TempData["mensaje"] = _tgasact.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000288")]
        public ActionResult Eliminar(string id)
        {
            var model = _tgasact.obtenerItem(id);

            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult Eliminar(TipGastDeActivModels model)
        {
            TempData["mensaje"] = _tgasact.eliminar(model.idTipGasAct);

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}