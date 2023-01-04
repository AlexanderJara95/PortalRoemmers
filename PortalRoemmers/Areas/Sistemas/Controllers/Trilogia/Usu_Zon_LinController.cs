using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Visitador;
using PortalRoemmers.Areas.Sistemas.Models.Trilogia;
using PortalRoemmers.Areas.Sistemas.Services.Trilogia;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System.Linq;
using System.Web.Mvc;
using PortalRoemmers.Helpers;
using System;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Trilogia
{
    public class Usu_Zon_LinController : Controller
    {//USU_ZON_LINCONTROLLER 000114
        private Usu_Zon_LinRepositorio _uzl;
        private UsuarioRepositorio _usu;
        private LineaRepositorio _lin;
        private ZonaRepositorio _zon;
        public Usu_Zon_LinController()
        {
            _uzl = new Usu_Zon_LinRepositorio();
            _usu = new UsuarioRepositorio();
            _lin = new LineaRepositorio();
            _zon = new ZonaRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000115")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _uzl.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000116")]
        [HttpGet]
        public ActionResult Registrar()
        {
            Parametros p = new Parametros();
            var para = p.Resultado(ConstantesGlobales.Com_Lis_Ven);
            ViewBag.usuario = new SelectList(_usu.obtenerUsuarios().Where(x => (para.Contains(x.empleado.idCarg)) && (x.idEst != ConstantesGlobales.estadoCesado)).Select(x => new { x.idAcc, nombre = x.empleado.nomComEmp }), "idAcc", "nombre");
            ViewBag.linea = new SelectList(_lin.obtenerLineas(), "idLin", "nomLin");
            ViewBag.zona = new SelectList(_zon.obtenerZonas(), "idZon", "nomZon");
            ViewBag.activarV = "active";
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(Usu_Zon_Lin_Models model)
        {
            if (ModelState.IsValid)
            {
                if (_uzl.verificar(model.idAcc, model.idLin, model.idZon))
                {
                    Parametros p = new Parametros();
                    var para = p.Resultado(ConstantesGlobales.Com_Lis_Ven);
                    TempData["mensaje"] = "<div id='warning' class='alert alert-success'>La combinación elegida ya existe, seleccione otra</div>";
                    ViewBag.usuario = new SelectList(_usu.obtenerUsuarios().Where(x => (para.Contains(x.empleado.idCarg)) && (x.idEst != ConstantesGlobales.estadoCesado)).Select(x => new { x.idAcc, nombre = x.empleado.nomComEmp }), "idAcc", "nombre", model.idAcc);
                    //ViewBag.usuario = new SelectList(_usu.obtenerUsuarios().Where(x => x.empleado.idCarg == ConstantesGlobales.VEN || x.empleado.idCarg == ConstantesGlobales.REP || x.empleado.idCarg == ConstantesGlobales.GP).Select(x => new { x.idAcc, nombre = x.empleado.nomComEmp }), "idAcc", "nombre", model.idAcc);
                    ViewBag.linea = new SelectList(_lin.obtenerLineas(), "idLin", "nomLin", model.idLin);
                    ViewBag.zona = new SelectList(_zon.obtenerZonas(), "idZon", "nomZon", model.idZon);
                    return View(model);
                }
                else {
                    model.usufchCrea = DateTime.Now;
                    model.usuCrea = SessionPersister.Username;
                    model.idEst = ConstantesGlobales.estadoActivo;

                    TempData["mensaje"] = _uzl.crear(model);
                    ViewBag.activarV = "active";
                    return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
                }
            }
            ViewBag.usuario = new SelectList(_usu.obtenerUsuarios().Where(x => x.empleado.idCarg == ConstantesGlobales.VEN || x.empleado.idCarg == ConstantesGlobales.REP || x.empleado.idCarg == ConstantesGlobales.GP).Select(x => new { x.idAcc, nombre = x.empleado.nomComEmp }), "idAcc", "nombre", model.idAcc);
            ViewBag.linea = new SelectList(_lin.obtenerLineas(), "idLin", "nomLin", model.idLin);
            ViewBag.zona = new SelectList(_zon.obtenerZonas(), "idZon", "nomZon", model.idZon);
            ViewBag.activarV = "active";
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000117")]
        public ActionResult Eliminar(string usu, string lin, string zon)
        {
            var model = _uzl.obtenerItem(usu, lin, zon);
            ViewBag.activarV = "active";
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(Usu_Zon_Lin_Models model)
        {
            TempData["mensaje"] = _uzl.updateEstUsu(model.idAcc, model.idLin, model.idZon,ConstantesGlobales.estadoInactivo);
            ViewBag.activarV = "active";
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000304")]
        public ActionResult Activar(string usu, string lin, string zon)
        {
            var model = _uzl.obtenerItem(usu, lin, zon);
            ViewBag.activarV = "active";
            return View(model);
        }

        [HttpPost, ActionName("Activar")]
        [SessionAuthorize]
        public ActionResult ActivarD(Usu_Zon_Lin_Models model)
        {
            TempData["mensaje"] = _uzl.updateEstUsu(model.idAcc, model.idLin, model.idZon, ConstantesGlobales.estadoActivo);
            ViewBag.activarV = "active";
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }


    }
}