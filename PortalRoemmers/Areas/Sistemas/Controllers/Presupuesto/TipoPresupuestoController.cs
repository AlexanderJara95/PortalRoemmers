using System;
using System.Linq;
using System.Web.Mvc;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Sistemas.Services.Presupuesto;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using PortalRoemmers.Filters;
using PortalRoemmers.Areas.Sistemas.Services.Global;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Presupuesto
{
    public class TipoPresupuestoController : Controller
    {//TIPOPRESUPUESTOCONTROLLER 000217
        private TipoPresupuestoRepositorio _tipPres;
        private EstadoRepositorio _est;
        public TipoPresupuestoController()
        {
            _est = new EstadoRepositorio();
            _tipPres = new TipoPresupuestoRepositorio();
        }
        // GET: Sistemas/TipoPresupuesto
        [CustomAuthorize(Roles = "000003,000218")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tipPres.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000219")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", ConstantesGlobales.estadoActivo);
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(TipoPresupuestoModels model)
        {
            if (ModelState.IsValid)
            {
                model.idEst = ConstantesGlobales.estadoActivo;
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                TempData["mensaje"] = _tipPres.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000220")]
        public ActionResult Modificar(string id)
        {
            var model = _tipPres.obtenerItem(id);
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(TipoPresupuestoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                TempData["mensaje"] = _tipPres.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000221")]
        public ActionResult Eliminar(string id)
        {
            var model = _tipPres.obtenerItem(id);

            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(TipoPresupuestoModels model)
        {
            TempData["mensaje"] = _tipPres.eliminar(model.idTipoPres);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}