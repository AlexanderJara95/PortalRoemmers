using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Producto;
using PortalRoemmers.Areas.Sistemas.Services.Visitador;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Visitador
{
    public class LineaController : Controller
    {//LINEACONTROLLER  000180
        private LineaRepositorio _lin;
        private EstadoRepositorio _est;
        private ProductoRepositorio _pro;
        private Pro_Lin_Repositorio _proLin;

        public LineaController()
        {
            _lin = new LineaRepositorio();
            _est = new EstadoRepositorio();
            _pro = new ProductoRepositorio();
            _proLin = new Pro_Lin_Repositorio(); 
        }

        [CustomAuthorize(Roles = "000003,000181")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _lin.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000182")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.estadoL = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst",ConstantesGlobales.estadoActivo);
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(LineaModels model)
        {
            if (ModelState.IsValid)
            {
                //agrego auditoria
                model.fchCreLin = DateTime.Now;
                model.userCreLin = SessionPersister.Username;

                TempData["mensaje"] = _lin.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.estadoL = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000183")]
        public ActionResult Modificar(string id)
        {
            var model = _lin.obtenerItem(id);

            ViewBag.estadoL = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(LineaModels model)
        {
            if (ModelState.IsValid)
            {
                //agrego auditoria
                model.fchModLin = DateTime.Now;
                model.userModLin = SessionPersister.Username;
                TempData["mensaje"] = _lin.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.estadoL = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000184")]
        public ActionResult Eliminar(string id)
        {
            var model = _lin.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(LineaModels model)
        {
            TempData["mensaje"] = _lin.eliminar(model.idLin);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
        //Relacion
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000185")]
        public ActionResult CrearLinxPro(string id,string nom)
        {
            var productos = _pro.obtenerProductos();
            var linxpro = _proLin.obtenerProLinID(id);
            //seleccionados
            var seleccionados = (from p in productos
                           join lp in linxpro on
                           p.idProAX equals lp.idProAX
                          select new { p.idProAX, p.nomPro}).ToList();
            //libres
            string[] selec = seleccionados.Select(x => x.idProAX).ToArray();

            var Nseleccionados = (from p in productos
                             where !selec.Contains(p.idProAX)
                             select new { p.idProAX, p.nomPro }).ToList();


            ViewBag.productosS = new SelectList(seleccionados, "idProAX", "nomPro");
            ViewBag.productosNS = new SelectList(Nseleccionados, "idProAX", "nomPro");
            ViewBag.producto = nom;

            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult AddLxP(string[] proIn, string id, string nombre)
        {
            string codigo = id;
            List<Pro_LIn_Models> lista=new List<Pro_LIn_Models>();
            Pro_LIn_Models item = new Pro_LIn_Models();

            foreach (string c in proIn)
            {
                item.idLin = id;
                item.idProAX = c;
                item.usuCrea = SessionPersister.Username;
                item.usufchCrea = DateTime.Now;
                lista.Add(item);
                item = new Pro_LIn_Models();
            }
            _proLin.crear(lista);
            return RedirectToAction("CrearLinxPro", new { id = codigo, nom = nombre });
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult DelLxP(string[] proAc, string id, string nombre)
        {

            string codigo = id;
            _proLin.eliminar(proAc, codigo);
            return RedirectToAction("CrearLinxPro", new { id = codigo, nom = nombre });
        }
    }
}