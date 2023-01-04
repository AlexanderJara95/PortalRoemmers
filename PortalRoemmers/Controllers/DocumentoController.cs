using PortalRoemmers.Areas.RRHH.Services.DocumentoRRHH;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Controllers
{
    public class DocumentoController : Controller
    {

        DocumentoRRHHRepositorio _doc;

        public DocumentoController()
        {
            _doc = new DocumentoRRHHRepositorio();

        }
            
        [CustomAuthorize(Roles = "000003,000390")]
        public ActionResult Politica(string menuArea, string menuVista)
        {
            var model = _doc.obtenerEnlances().Where(x => x.idTipDoc == ConstantesGlobales.tipDocPolitica).ToList();
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000392")]
        public ActionResult Reglamento(string menuArea, string menuVista)
        {
            var model = _doc.obtenerEnlances().Where(x => x.idTipDoc == ConstantesGlobales.tipDocReglamento).ToList();
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000393")]
        public ActionResult Seguridad(string menuArea, string menuVista)
        {
            var model = _doc.obtenerEnlances().Where(x => x.idTipDoc == ConstantesGlobales.tipDocSeguridad).ToList();
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000394")]
        public ActionResult Covid(string menuArea, string menuVista)
        {
            var model = _doc.obtenerEnlances().Where(x => x.idTipDoc == ConstantesGlobales.tipDocCovid).ToList();
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }
    }
}