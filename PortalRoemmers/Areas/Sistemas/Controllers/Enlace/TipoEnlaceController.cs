using PortalRoemmers.Areas.Sistemas.Models.Enlace;
using PortalRoemmers.Areas.Sistemas.Services.Enlace;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Enlace
{
    public class TipoEnlaceController : Controller
    {

        private TipoEnlaceRepositorio _tipE;
        public TipoEnlaceController()
        {
            _tipE = new TipoEnlaceRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000296")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tipE.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000297")]
        public ActionResult Modificar(string id)
        {

            var model = _tipE.obtenerItem(id);

            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(TipoEnlaceModels model)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                if (_tipE.modificar(model))
                {
                     mensaje = "<div id='success' class='alert alert-success'>Se modificó el registro.</div>";
                }
                else
                {
                     mensaje = "<div id='warning' class='alert alert-warning'>" + "Se produjo un error al modificar" + "</div>";
                }
                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }

    }
}