using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Security;
using System.Web.Mvc;
using PortalRoemmers.Filters;
using System;
using PortalRoemmers.Areas.Sistemas.Services.Solicitud;
using PortalRoemmers.Areas.Sistemas.Models.Solicitud;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Solicitud
{
    public class TipoPagoController : Controller
    {//TIPOPAGOCONTROLLER 000073
        private TipoPagoRepositorio _tpag;
        private EstadoRepositorio _est;
        public TipoPagoController()
        {
            _tpag = new TipoPagoRepositorio();
            _est = new EstadoRepositorio();
        }
        //GET
        [CustomAuthorize(Roles = "000003,000074")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tpag.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
  
        //registrar
        [CustomAuthorize(Roles = "000003,000075")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", ConstantesGlobales.estadoActivo);
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(TipoPagoModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchCreTipPag = DateTime.Now;
                model.userCreTipPag = SessionPersister.Username;
                model.idEst = ConstantesGlobales.estadoActivo;
                TempData["mensaje"] = _tpag.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000076")]
        public ActionResult Modificar(string id)
        {
            var model = _tpag.obtenerItem(id);
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(TipoPagoModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchModTipPag = DateTime.Now;
                model.userModTipPag = SessionPersister.Username;
                TempData["mensaje"] = _tpag.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000077")]
        public ActionResult Eliminar(string id)
        {
            var model = _tpag.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(TipoPagoModels model)
        {
            TempData["mensaje"] = _tpag.eliminar(model.idTipPag);
            
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

    }
}