using System.Linq;
using System.Web.Mvc;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Sistemas.Services.Gasto;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using System;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Gasto
{
    public class ActividadGastoController : Controller
    {//ACTIVIDADGASTOCONTROLLER 00000

        private ActividadGastoRepositorio _actgas;
        private TipGastDeActivRepositorio _tgasact;
        private EstadoRepositorio _est;
        public ActividadGastoController()
        {
            _actgas = new ActividadGastoRepositorio();
            _tgasact = new TipGastDeActivRepositorio();
            _est = new EstadoRepositorio();
        }
        // Listar y Busqueda
        [CustomAuthorize(Roles = "000003,000290")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _actgas.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000291")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.tipogastoAct = new SelectList(_tgasact.obtenerTipGastoAct(), "idTipGasAct", "nomTipGasAct");
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst");
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(ActividadGastoModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchCreGastActiv = DateTime.Now;
                model.userCreGastActiv = SessionPersister.Username;
                model.idEst = ConstantesGlobales.estadoActivo;
                TempData["mensaje"] = _actgas.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.tipogasto = new SelectList(_tgasact.obtenerTipGastoAct(), "idTipGasAct", "nomTipGasAct", model.idTipGasAct);
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);

            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000292")]
        public ActionResult Modificar(string id)
        {
            var model = _actgas.obtenerItem(id);
            ViewBag.tipogastoAct = new SelectList(_tgasact.obtenerTipGastoAct(), "idTipGasAct", "nomTipGasAct", model.idTipGasAct);
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);

            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(ActividadGastoModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchModGastActiv = DateTime.Now;
                model.userModGastActiv = SessionPersister.Username;
                TempData["mensaje"] = _actgas.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.tipogastoAct = new SelectList(_tgasact.obtenerTipGastoAct(), "idTipGasAct", "nomTipGasAct", model.idTipGasAct);
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);

            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000293")]
        public ActionResult Eliminar(string id)
        {
            var model = _actgas.obtenerItem(id);

            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult Eliminar(ActividadGastoModels model)
        {
            TempData["mensaje"] = _actgas.eliminar(model.idTipGasAct);

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}