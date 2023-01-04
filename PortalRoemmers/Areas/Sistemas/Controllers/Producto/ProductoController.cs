
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Areas.Sistemas.Services.Producto;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System.Linq;
using System.Web.Mvc;
using System;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Producto
{
    public class ProductoController : Controller
    {//PRODUCTOCONTROLLER  000098
        private ProductoRepositorio _pro;
        private UsuarioRepositorio _usu;
        private FamiliaAXRepositorio _famAx;
        private FamiliaRoeRepositorio _famRoe;
        private LaboratorioRepositorio _lab;
        private EstadoRepositorio _est;
        private EmpleadoRepositorio _emp;
        private AreaTerapeuticaRepositorio _areaTer;
        public ProductoController() {
            _pro = new ProductoRepositorio();
            _usu = new UsuarioRepositorio();
            _famAx = new FamiliaAXRepositorio();
            _famRoe = new FamiliaRoeRepositorio();
            _lab = new LaboratorioRepositorio();
            _est = new EstadoRepositorio();
            _emp = new EmpleadoRepositorio();
            _areaTer = new AreaTerapeuticaRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000099")]
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
        [CustomAuthorize(Roles = "000003,000100")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.familiaAx = new SelectList(_famAx.obtenerFamiliaAX(), "idFam", "nomFam");
            ViewBag.familiaRoe = new SelectList(_famRoe.obtenerFamiliaRoe(), "idFamRoe", "nomFamRoe");
            ViewBag.usuario = new SelectList(_emp.obtenerEmpleados().Where(x => x.idCarg == ConstantesGlobales.GP || x.idCarg == ConstantesGlobales.GPJ || x.idCarg == ConstantesGlobales.NI || x.idCarg == ConstantesGlobales.GM).Select(x => new { x.idEmp, nombre = x.nomComEmp}), "idEmp", "nombre");
            ViewBag.laborato = new SelectList(_lab.obtenerLaboratorio(), "idLab", "nomLab");
            ViewBag.areaTer = new SelectList(_areaTer.obtenerAreaTerap(), "idAreaTerap", "desAreaTerap");
            
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(ProductoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                model.idEst = ConstantesGlobales.estadoActivo;
                TempData["mensaje"] = _pro.crear(model);
                
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.familiaAx = new SelectList(_famAx.obtenerFamiliaAX(), "idFam", "nomFam", model.idFam);
            ViewBag.familiaRoe = new SelectList(_famRoe.obtenerFamiliaRoe(), "idFamRoe", "nomFamRoe", model.idFamRoe);
            ViewBag.usuario = new SelectList(_emp.obtenerEmpleados().Where(x => x.idCarg == ConstantesGlobales.GP || x.idCarg == ConstantesGlobales.GPJ || x.idCarg == ConstantesGlobales.NI || x.idCarg == ConstantesGlobales.GM).Select(x => new { x.idEmp, nombre = x.nomComEmp }), "idEmp", "nombre", model.idEmp);
            ViewBag.laborato = new SelectList(_lab.obtenerLaboratorio(), "idLab", "nomLab", model.idLab);
            ViewBag.areaTer = new SelectList(_areaTer.obtenerAreaTerap(), "idAreaTerap", "desAreaTerap",model.idAreaTerap);
            return View(model);
        }

        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000101")]
        public ActionResult Modificar(string id)
        {
            var model = _pro.obtenerItem(id);

            ViewBag.estado = new SelectList(_est.obteneEstadoProducto(), "idEst", "nomEst", model.idEst);
            ViewBag.familiaAx = new SelectList(_famAx.obtenerFamiliaAX(), "idFam", "nomFam", model.idFam);
            ViewBag.familiaRoe = new SelectList(_famRoe.obtenerFamiliaRoe(), "idFamRoe", "nomFamRoe", model.idFamRoe);
            ViewBag.usuario = new SelectList(_emp.obtenerEmpleados().Where(x => x.idCarg == ConstantesGlobales.GP || x.idCarg == ConstantesGlobales.GPJ || x.idCarg == ConstantesGlobales.NI || x.idCarg == ConstantesGlobales.GM).Select(x => new { x.idEmp, nombre = x.nomComEmp }), "idEmp", "nombre", model.idEmp);
            ViewBag.laborato = new SelectList(_lab.obtenerLaboratorio(), "idLab", "nomLab", model.idLab);
            ViewBag.areaTer = new SelectList(_areaTer.obtenerAreaTerap(), "idAreaTerap", "desAreaTerap", model.idAreaTerap);

            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(ProductoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                TempData["mensaje"] = _pro.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.familiaAx = new SelectList(_famAx.obtenerFamiliaAX(), "idFam", "nomFam", model.idFam);
            ViewBag.familiaRoe = new SelectList(_famRoe.obtenerFamiliaRoe(), "idFamRoe", "nomFamRoe", model.idFamRoe);
            ViewBag.usuario = new SelectList(_emp.obtenerEmpleados().Where(x => x.idCarg == ConstantesGlobales.GP || x.idCarg == ConstantesGlobales.GPJ || x.idCarg == ConstantesGlobales.NI || x.idCarg == ConstantesGlobales.GM).Select(x => new { x.idEmp, nombre = x.nomComEmp }), "idEmp", "nombre", model.idEmp);
            ViewBag.laborato = new SelectList(_lab.obtenerLaboratorio(), "idLab", "nomLab", model.idLab);
            ViewBag.estado = new SelectList(_est.obteneEstadoProducto(), "idEst", "nomEst", model.idEst);
            ViewBag.areaTer = new SelectList(_areaTer.obtenerAreaTerap(), "idAreaTerap", "desAreaTerap", model.idAreaTerap);

            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000102")]
        public ActionResult Eliminar(string id)
        {
            var model = _pro.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(ProductoModels model)
        {
            TempData["mensaje"] = _pro.eliminar(model.idProAX);
            
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        //json
        public JsonResult SearchProducto(string buscar)
        {
            var s = _pro.obtenerProductos().Where(x => x.nomPro.ToUpper().Contains(buscar.ToUpper())).Select(z => new { cod1 = z.idFamRoe+"+"+z.idAreaTerap, nombre1 = z.nomPro , nombre2 = z.familiaRoe.nomFamRoe , nombre3 = z.areaTerap.desAreaTerap , cod2 = z.idFamRoe , cod3 = z.idAreaTerap });
            return Json(s, JsonRequestBehavior.AllowGet);
        }
    }
}