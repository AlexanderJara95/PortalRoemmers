using System;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Security;
using System.Linq;
using System.Web.Mvc;
using PortalRoemmers.Filters;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Global
{
    public class TipoCambioController : Controller
    {// TIPOCAMBIOCONTROLLER 000200
        private TipoCambioRepositorio _tcbio;
        public TipoCambioController()
        {
            _tcbio = new TipoCambioRepositorio();
        }
        // GET: Sistemas/TipoCambio
        [CustomAuthorize(Roles = "000003,000201")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tcbio.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000202")]
        [HttpGet]
        public ActionResult Registrar()
        {
            var actual = DateTime.Today.ToString("dd/MM/yyyy");
            ViewBag.actual = actual;
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(TipoCambioModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchCreTC = DateTime.Now;
                model.userCreTC = SessionPersister.Username;
                TempData["mensaje"] = _tcbio.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000203")]
        public ActionResult Modificar(string id)
        {
            DateTime Date = DateTime.Parse(id);
            var model = _tcbio.obtenerItem(Date);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(TipoCambioModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchModTC = DateTime.Now;
                model.userModTC = SessionPersister.Username;
                TempData["mensaje"] = _tcbio.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000204")]
        public ActionResult Eliminar(string id)
        {
            DateTime Date = DateTime.Parse(id);
            var model = _tcbio.obtenerItem(Date);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(TipoCambioModels model)
        {
            DateTime Date = DateTime.Parse(model.fchTipoCbio.ToString());
            TempData["mensaje"] = _tcbio.eliminar(Date);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

    }
}