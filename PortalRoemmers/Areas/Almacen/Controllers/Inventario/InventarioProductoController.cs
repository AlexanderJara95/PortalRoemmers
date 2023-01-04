using DocumentFormat.OpenXml.Spreadsheet;
using PortalRoemmers.Areas.Almacen.Models.Inventario;
using PortalRoemmers.Areas.Almacen.Services.Inventario;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Almacen.Controllers.Inventario
{
    public class InventarioProductoController : Controller
    {

        private InventarioProductoService _con;
        private InventarioAxService _inv;
        private NumeroConteoService _conR;

        // GET: Almacen/ConteoProducto
        [CustomAuthorize(Roles = "000003,000310")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            _con = new InventarioProductoService();

            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _con.obtenerTodos(pagina, search);
            ViewBag.search = search;

            return View(model);
        }


        [HttpGet]
        [CustomAuthorize(Roles = "000003,000323")]
        public ActionResult RegistrarDes()
        {
            _con = new InventarioProductoService();
            _inv = new InventarioAxService();
            _conR = new NumeroConteoService();

            ViewBag.nroInvCon = new SelectList(_conR.obtenerConteoActivo().Select(x => new { x.codCon, x.desCon }), "codCon", "desCon");
            ViewBag.nroLotCon = new SelectList(_inv.obtenerLotexCBar("", "").Select(x => new { value = x, text = "Nro.:" + x }), "Value", "Text");
            ViewBag.ubiProCon = new SelectList(_inv.obtenerUbixLotyCBar("", "", "").Select(x => new { value = x, text = "Ubi.:" + x }), "Value", "Text");
            ViewBag.almInvCon = new SelectList(_inv.obtenerAlmxCBar("").Select(x => new { value = x, text = "Nom.:" + x }), "Value", "Text");
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult RegistrarDes(InventarioProductoModels model)
        {
            _con = new InventarioProductoService();
            _inv = new InventarioAxService();
            _conR = new NumeroConteoService();


            if(ModelState.IsValid)
            {
                
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                if(_con.crear(model))
                {
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>Error al crear el registro.</div>";
                }

                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            
            ViewBag.nroInvCon = new SelectList(_conR.obtenerConteoActivo().Select(x => new { x.codCon, x.desCon }), "codCon", "desCon", model.nroInvCon);
            ViewBag.nroLotCon = new SelectList(_inv.obtenerLotexCBar(model.codProCon, model.almInvCon).Select(x => new { value = x, text = "Nro.:" + x }), "Value", "Text", model.nroLotCon);
            ViewBag.ubiProCon = new SelectList(_inv.obtenerUbixLotyCBar(model.codProCon, model.nroLotCon, model.almInvCon).Select(x => new { value = x, text = "Ubi.:" + x }), "Value", "Text", model.ubiProCon);
            ViewBag.almInvCon = new SelectList(_inv.obtenerAlmxCBar(model.codProCon).Select(x => new { value = x, text = "Nom.:" + x }), "Value", "Text");

            return View();
        }

        [HttpGet]
        [CustomAuthorize(Roles = "000003,000311")]
        public ActionResult Registrar()
        {
            _con = new InventarioProductoService();
            _conR = new NumeroConteoService();
            _inv = new InventarioAxService();
            ViewBag.nroInvCon = new SelectList(_conR.obtenerConteoActivo().Select(x=>new { x.codCon,x.desCon }), "codCon", "desCon");
            ViewBag.nroLotCon = new SelectList(_inv.obtenerLotexCBar("","").Select(x => new { value = x, text = "Nro.:" + x }), "Value", "Text");
            ViewBag.ubiProCon = new SelectList(_inv.obtenerUbixLotyCBar("", "","").Select(x => new { value = x, text = "Ubi.:" + x }), "Value", "Text");
            ViewBag.almInvCon = new SelectList(_inv.obtenerAlmxCBar("").Select(x => new { value = x, text = "Nom.:" + x }), "Value", "Text");
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(InventarioProductoModels model)
        {
            _con = new InventarioProductoService();
            _inv = new InventarioAxService();
            _conR = new NumeroConteoService();
            if(ModelState.IsValid)
            {
               
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                if(_con.crear(model))
                {
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>Error al crear el registro.</div>";
                }

                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            _con = new InventarioProductoService();
            _inv = new InventarioAxService();
            _conR = new NumeroConteoService();
            ViewBag.nroInvCon = new SelectList(_conR.obtenerConteoActivo().Select(x => new { x.codCon, x.desCon }), "codCon", "desCon", model.nroInvCon);
            ViewBag.nroLotCon = new SelectList(_inv.obtenerLotexCBar(model.codProCon, model.almInvCon).Select(x => new { value = x, text = "Nro.:" + x }), "Value", "Text", model.nroLotCon);
            ViewBag.ubiProCon = new SelectList(_inv.obtenerUbixLotyCBar(model.codProCon, model.nroLotCon,model.almInvCon).Select(x => new { value = x, text = "Ubi.:" + x }), "Value", "Text", model.ubiProCon);
            ViewBag.almInvCon = new SelectList(_inv.obtenerAlmxCBar(model.codProCon).Select(x => new { value = x, text = "Nom.:" + x }), "Value", "Text", model.almInvCon);

            return View();
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000312")]
        public ActionResult Modificar(string codBar, string nrolot, string ubiPro, string nroCon)
        {
            int num = 0;
            _con = new InventarioProductoService();
            _inv = new InventarioAxService();
            int.TryParse(nroCon, out num);

            var model = _con.obtenerModel(codBar, nrolot, ubiPro, num);

            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(InventarioProductoModels model)
        {
            if(ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                _con = new InventarioProductoService();
                if(_con.modificar(model))
                {
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se modificó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "< div id = 'warning' class='alert alert-warning'>Error al modificar el registro.</div>";
                }


                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

            return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000313")]
        public ActionResult Eliminar(string codBar, string nrolot, string ubiPro, string nroCon)
        {
            int num = 0;
            int.TryParse(nroCon, out num);
            _con = new InventarioProductoService();
            var model = _con.obtenerModel(codBar, nrolot, ubiPro, num);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Eliminar(InventarioProductoModels model)
        {
            _con = new InventarioProductoService();
            if(_con.eliminar(model))
            {
                TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se eliminó el registro.</div>";
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'> Error al eliminar el registro.</div>";
            }

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        public FileResult Reporte(string ruta)
        {
            return File(ruta, "application/vnd.ms-excel");
        }

        [HttpPost]
        public JsonResult autoDesPro(string prefix)
        {
            _inv = new InventarioAxService();
            var auto = _inv.obtenerDesUnica().Where(x => x.text.ToUpper().Contains(prefix.ToUpper())).Select(x => new { x.value, x.text });
            return Json(auto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult autoUbi(string prefix, string codProCon, string nroLotCon)
        {
            _inv = new InventarioAxService();
            var auto = _inv.obtenerUbiUnica(codProCon, nroLotCon).Where(x => x.text.ToUpper().StartsWith(prefix.ToUpper())).Select(x => new { x.value, x.text });
            return Json(auto, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult buscarAlmacen(string codProCon)
        {
            _inv = new InventarioAxService();
            return Json(_inv.obtenerAlmxCBar(codProCon).Select(x => new { value = x, text = "Alm.:" + x }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult buscarNroLote(string codProCon, string almInvCon)
        {
            _inv = new InventarioAxService();
            return Json(_inv.obtenerLotexCBar(codProCon, almInvCon).Select(x => new { value = x, text = "Nro.:" + x }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult buscarDes(string codBarPro)
        {
            _inv = new InventarioAxService();
            return Json(_inv.obtenerDescripcion(codBarPro), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult buscarConteo(string codProCon,string nroLotCon,string ubiProCon,int nroInvCon)
        {
            string mensaje = "";

            _con = new InventarioProductoService();

            var conteo=_con.obtenerModel(codProCon, nroLotCon, ubiProCon, nroInvCon);

            if (conteo!=null)
            {
                mensaje = conteo.usuCrea;
            }

            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }

    }
}