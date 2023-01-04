using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Sistemas.Services.Proveedor;
using PortalRoemmers.Areas.Sistemas.Models.Proveedor;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Filters;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Proveedor
{
    public class ProveedorController : Controller
    {//EQUIPOCONTROLLER 000349
        private ProveedorRepositorio _pro;
        private TipProvRepositorio _tipPro;
        private EstadoRepositorio _est;
        // GET: Sistemas/Proveedor
        public ProveedorController()
        {
            _pro = new ProveedorRepositorio();
            _tipPro = new TipProvRepositorio();
            _est = new EstadoRepositorio();
        }
        [CustomAuthorize(Roles = "000003,000350")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _pro.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000351")]
        [HttpGet]   
        public ActionResult Registrar()
        {
            ViewBag.TipPro = new SelectList(_tipPro.obtenerTipoProv(), "idTipPro", "nomTipPro");
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", ConstantesGlobales.estadoActivo);
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(ProveedorModels model)
        {
            string mensaje = "";
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                _pro.crear(model, out mensaje);
                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.TipPro = new SelectList(_tipPro.obtenerTipoProv(), "idTipPro", "nomTipPro", model.idTipPro);
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000352")]
        public ActionResult Modificar(string id)
        {
            var model =_pro.obtenerItem(id);
            ViewBag.TipPro = new SelectList(_tipPro.obtenerTipoProv(), "idTipPro", "nomTipPro", model.idTipPro);
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(ProveedorModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;
                TempData["mensaje"] = _pro.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.TipPro = new SelectList(_tipPro.obtenerTipoProv(), "idTipPro", "nomTipPro", model.idTipPro);
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
           
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000353")]
        public ActionResult Eliminar(string id)
        {
            var model = _pro.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(ProveedorModels model)
        {
            TempData["mensaje"] = _pro.eliminar(model.idPro);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
        //Crear Proveedor desde Modal
        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000354")]
        public JsonResult CrearProvModelo(ProveedorModels model)
        {
            string mensJson = "";
            string mensaje = "";
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                model.usuMod = "";
                model.idTipPro = "0000000";
                if (_pro.crear(model, out mensaje))
                {
                    mensJson = "ok";
                }
                else
                {
                    mensJson = "error";
                }
            }
            return Json(mensJson, JsonRequestBehavior.AllowGet);
        }
    }
}