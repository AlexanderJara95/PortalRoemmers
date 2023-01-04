using PortalRoemmers.Areas.Sistemas.Models.Medico;
using PortalRoemmers.Areas.Sistemas.Services.Medico;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Medico
{
    public class TipMedController : Controller
    {//TIPCLICONTROLLER 000013
        private TipMedRepositorio _tcli;
        public TipMedController()
        {
            _tcli = new TipMedRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000014")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tcli.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000015")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(TipoMedicoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                TempData["mensaje"] = _tcli.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000016")]
        public ActionResult Modificar(string id)
        {
            var model = _tcli.obtenerItem(id);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(TipoMedicoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.UtcNow;
                TempData["mensaje"] = _tcli.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000017")]
        public ActionResult Eliminar(string id)
        {
            var model = _tcli.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(TipoMedicoModels model)
        {
            TempData["mensaje"] = _tcli.eliminar(model.idTipCli);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}