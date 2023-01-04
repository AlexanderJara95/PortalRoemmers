using PortalRoemmers.Areas.RRHH.Services.Boleta;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.Boleta
{
    public class BoletaDetalleController : Controller
    {
        private BoletaDetalleRepositorio _det;
        private BoletaPersonalRepositorio _bol;
        public BoletaDetalleController()
        {
            _det = new BoletaDetalleRepositorio();
            _bol = new BoletaPersonalRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000338")]
        public ActionResult Index(string menuArea, string menuVista,string idBolPer,string Value = "false")
        {
            ViewBag.periodo = new SelectList(_bol.obtenerBoletas().Select(x => new { x.idBolPer, x.titBolPer }), "idBolPer", "titBolPer");
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;

  
            ViewBag.visualizar = new SelectList(ListadoVista(), "Value", "Text", Value);
            var model = _det.obtenerBoletas(idBolPer, Value);
            return View(model);
        }

        private List<SelectListItem> ListadoVista()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "SI CONFIRMO", Value = "true" });
            lst.Add(new SelectListItem() { Text = "NO CONFIRMO", Value = "false"});

            return lst;
        }

     

    }
}