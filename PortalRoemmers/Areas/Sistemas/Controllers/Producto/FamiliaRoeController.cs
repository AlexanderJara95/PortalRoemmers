using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Areas.Sistemas.Services.Producto;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Producto
{
    public class FamiliaRoeController : Controller
    {//FAMILIAROECONTROLLER 000088
        private FamiliaRoeRepositorio _famRoe;

        public FamiliaRoeController()
        {
            _famRoe = new FamiliaRoeRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000089")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _famRoe.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000090")]
        [HttpGet]
        public ActionResult Registrar()
        {
            
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(FamProdRoeModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                TempData["mensaje"] = _famRoe.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000091")]
        public ActionResult Modificar(string id)
        {
            var model = _famRoe.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(FamProdRoeModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                TempData["mensaje"] = _famRoe.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            
            return View(model);
        }

        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000092")]
        public ActionResult Eliminar(string id)
        {
            var model = _famRoe.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(FamProdRoeModels model)
        {
            TempData["mensaje"] = _famRoe.eliminar(model.idFamRoe);
            
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        //json
        public JsonResult SearchFamilia(string buscar)
        {
            var s= _famRoe.obtenerFamiliaRoe().Where(x => x.nomFamRoe.ToUpper().Contains(buscar.ToUpper())).Select(z=>new { codigo = z.idFamRoe,nombre=z.nomFamRoe,descripcion=z.desFamRoe });
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        //json
        public JsonResult SearchFamiliaAT(string buscar)
        {
            var s = _famRoe.obtenerFamiliaRoeAT().Where(x => x.nom1.ToUpper().Contains(buscar.ToUpper())).ToList();
            return Json(s, JsonRequestBehavior.AllowGet);
        }

    }
}