using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Sistemas.Services.Gasto;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Gasto
{
    public class ConceptoGastoController : Controller
    {//CONCEPTOGASTOCONTROLLER 000048
        private ConceptoGastoRepositorio _cgas;
        private TipoGastoRepositorio _tgas;
        private EstadoRepositorio _est;

        public ConceptoGastoController()
        {
            _cgas = new ConceptoGastoRepositorio();
            _tgas = new TipoGastoRepositorio();
            _est = new EstadoRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000049")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _cgas.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000050")]
        [HttpGet]
        public ActionResult Registrar()
        {

            ViewBag.tipogasto = new SelectList(_tgas.obtenerTipGasto(), "idTipGas", "nomTipGas");
            
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(ConceptoGastoModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchCreConGas = DateTime.Now;
                model.userCreConGas = SessionPersister.Username;
                model.idEst = ConstantesGlobales.estadoActivo;
                TempData["mensaje"] = _cgas.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.tipogasto = new SelectList(_tgas.obtenerTipGasto(), "idTipGas", "nomTipGas");
            
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000051")]
        public ActionResult Modificar(string id)
        {
            var model = _cgas.obtenerItem(id);
            ViewBag.tipogasto = new SelectList(_tgas.obtenerTipGasto(), "idTipGas", "nomTipGas", model.idTipGas);
            ViewBag.EstU = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(ConceptoGastoModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchModConGas = DateTime.Now;
                model.userModConGas = SessionPersister.Username;
                TempData["mensaje"] = _cgas.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.tipogasto = new SelectList(_tgas.obtenerTipGasto(), "idTipGas", "nomTipGas", model.idTipGas);
            ViewBag.EstU = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000052")]
        public ActionResult Eliminar(string id)
        {
            var model = _cgas.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(ConceptoGastoModels model)
        {
            TempData["mensaje"] = _cgas.eliminar(model.idConGas);
            
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}