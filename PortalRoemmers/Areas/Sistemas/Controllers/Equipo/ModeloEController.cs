using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Areas.Sistemas.Services.Equipo;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Equipo
{
    public class ModeloEController : Controller
    {// MODELOECONTROLLER   000028
        private ModeloERepositorio _modE;
        private FabricanteRepositorio _fab;
        public ModeloEController()
        {
            _modE = new ModeloERepositorio();
            _fab = new FabricanteRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000029")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _modE.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000030")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.fabricante = new SelectList(_fab.obtenerFabricantes(), "idFabrica", "nomFabri");
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(ModEquiModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                if (_modE.crear(model))
                {
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>Se genero un error al crear el registro. </div>";
                }

               
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.fabricante = new SelectList(_fab.obtenerFabricantes(), "idFabrica", "nomFabri");
            return View(model);
        }

        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000031")]
        public ActionResult Modificar(string id)
        {
            var modelo = _modE.obtenerItem(id);
            ViewBag.fabricante = new SelectList(_fab.obtenerFabricantes(), "idFabrica", "nomFabri", modelo.idFabrica);
            return View(modelo);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(ModEquiModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                TempData["mensaje"] = _modE.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }

        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000032")]
        public ActionResult Eliminar(string id)
        {
            var model = _modE.obtenerItem(id);
            return View(model);
        }

        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(ModEquiModels model)
        {
            TempData["mensaje"] = _modE.eliminar(model.idMolEq);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }


    }
}