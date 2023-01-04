using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Security;
using System.Web.Mvc;
using PortalRoemmers.Filters;
using System;
using PortalRoemmers.Areas.Sistemas.Services.Solicitud;
using PortalRoemmers.Areas.Sistemas.Models.Solicitud;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Solicitud
{
    public class TipoSolController : Controller
    {//TIPOSOLCONTROLLER 000078
        private TipoSolRepositorio _tsol;
        private EstadoRepositorio _est;

        public TipoSolController()
        {
            _est = new EstadoRepositorio();
            _tsol = new TipoSolRepositorio();
        }
        //GET
        [CustomAuthorize(Roles = "000003,000079")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tsol.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000080")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", ConstantesGlobales.estadoActivo);
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(TipoSolModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchCreTipSol = DateTime.Now;
                model.userCreTipSol = SessionPersister.Username;
                model.idEst = ConstantesGlobales.estadoActivo;
                TempData["mensaje"] = _tsol.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000081")]
        public ActionResult Modificar(string id)
        {
            var model = _tsol.obtenerItem(id);
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(TipoSolModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchModTipSol = DateTime.Now;
                model.userModTipSol = SessionPersister.Username;
                TempData["mensaje"] = _tsol.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000082")]
        public ActionResult Eliminar(string id)
        {
            var model = _tsol.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(TipoSolModels model)
        {
            TempData["mensaje"] = _tsol.eliminar(model.idTipSol);
            
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}