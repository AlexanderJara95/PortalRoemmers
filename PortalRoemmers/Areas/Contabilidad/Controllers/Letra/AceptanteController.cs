using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Contabilidad.Services.Letra;
using PortalRoemmers.Areas.Contabilidad.Models.Letra;

namespace PortalRoemmers.Areas.Contabilidad.Controllers.Letra
{
    //AREA ROL 
    public class AceptanteController : Controller
    {//ACEPTANTE_CONTROLLER 
        private AceptanteService _acept;
        private EstadoRepositorio _est;
        public AceptanteController()
        {
            _acept = new AceptanteService();
            _est = new EstadoRepositorio();
        }
        //ACEPTANTE_LISTAR 
        [CustomAuthorize(Roles = "000003,000366")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            //-----------------------------
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            //-----------------------------
            var model = _acept.obtenerTodos(pagina,search);
            //-----------------------------
            ViewBag.search = search;
            //-----------------------------
            return View(model);
        }
        //ACEPTANTE_REGISTRAR 
        [CustomAuthorize(Roles = "000003,000367")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(AceptanteModels model)
        {
            model.idEst = ConstantesGlobales.estadoActivo;
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                TempData["mensaje"] = _acept.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            //ViewBag.estado = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //ACEPTANTE_MODIFICAR
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000368")]
        public ActionResult Modificar(string id)
        {
            var model = _acept.obtenerItem(id);
            ViewBag.estado = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(AceptanteModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;
                TempData["mensaje"] = _acept.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.estado = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //ACEPTANTE_ELIMINAR
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000369")]
        public ActionResult Eliminar(string id)
        {
            var model = _acept.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(AceptanteModels model)
        {
            TempData["mensaje"] = _acept.eliminar(model.idAcep);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
    }
}