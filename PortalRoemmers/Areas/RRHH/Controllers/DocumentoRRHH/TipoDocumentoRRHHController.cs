using PortalRoemmers.Areas.RRHH.Models.Documento;
using PortalRoemmers.Areas.RRHH.Services.DocumentoRRHH;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.DocumentoRRHH
{
    public class TipoDocumentoRRHHController : Controller
    {
        private TipoDocumentoRRHHRepositorio _tipDoc;
        public TipoDocumentoRRHHController()
        {
            _tipDoc = new TipoDocumentoRRHHRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000396")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tipDoc.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000397")]
        public ActionResult Modificar(string id)
        {

            var model = _tipDoc.obtenerItem(id);

            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(TipoDocumentoRRHHModels model)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                if (_tipDoc.modificar(model))
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