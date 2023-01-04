using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Areas.Sistemas.Services.Producto;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Producto
{
    public class LaboratorioController : Controller
    {//LABORATORIOCONTROLLER 000093
        private LaboratorioRepositorio _lab;
        public LaboratorioController()
        {
            _lab = new LaboratorioRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000094")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _lab.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000095")]
        [HttpGet]
        public ActionResult Registrar()
        {
            
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(LaboratorioModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                TempData["mensaje"] = _lab.crear(model);
                
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000096")]
        public ActionResult Modificar(string id)
        {
            var model = _lab.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(LaboratorioModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                TempData["mensaje"] = _lab.modificar(model);
                
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000097")]
        public ActionResult Eliminar(string id)
        {
            var model = _lab.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(LaboratorioModels model)
        {
            TempData["mensaje"] = _lab.eliminar(model.idLab);
            
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

    }
}