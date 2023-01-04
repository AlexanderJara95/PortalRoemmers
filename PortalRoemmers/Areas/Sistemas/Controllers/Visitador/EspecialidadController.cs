using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Visitador;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Visitador
{
    public class EspecialidadController : Controller
    {//ESPECIALIDADCONTROLLER   000175
        private EspecialidadRepositorio _esp;
        private EstadoRepositorio _est;
        private LineaRepositorio _lin;
        private Esp_Usu_Repositorio _espU;
        private UsuarioRepositorio _usu;

        public EspecialidadController()
        {
            _esp = new EspecialidadRepositorio();
            _est = new EstadoRepositorio();
            _lin = new LineaRepositorio();
            _espU = new Esp_Usu_Repositorio();
            _usu = new UsuarioRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000176")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _esp.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000177")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(EspecialidadModels model)
        {
            if (ModelState.IsValid)
            {
                model.idEst = ConstantesGlobales.estadoActivo;
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                TempData["mensaje"] = _esp.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000178")]
        public ActionResult Modificar(string id)
        {
            var model = _esp.obtenerItem(id);
            ViewBag.estadoG = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(EspecialidadModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;
                TempData["mensaje"] = _esp.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.estadoG = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000179")]
        public ActionResult Eliminar(string id)
        {
            var model = _esp.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(EspecialidadModels model)
        {
            TempData["mensaje"] = _esp.eliminar(model.idEsp);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000236")]
        public ActionResult EspecialidadUsuario(string id, string esp)
        {

            var usuarios = _usu.obtenerUsuarios();
            var seleccionados = _espU.obtenerUsuarioxEspecialidad(id).Select(x => new { x.idAcc, nomComEmp=x.accounts.empleado.nomComEmp+" ("+x.accounts.username +")" });

            string[] selec = seleccionados.Select(x => x.idAcc.ToString()).ToArray();
           
             var Nseleccionados = (from u in usuarios
                                   where !selec.Contains(u.idAcc.ToString())
                                   select new { u.idAcc, nomComEmp = u.empleado.nomComEmp + " (" + u.username + ")" }).ToList();

            ViewBag.idEsp = id;
            ViewBag.especialidad = esp;
            ViewBag.disponible = new SelectList(Nseleccionados, "idAcc", "nomComEmp");
            ViewBag.elegido = new SelectList(seleccionados, "idAcc", "nomComEmp"); 
            
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult AgregarUsuarios(string[] idAcc, string id,string nombre)
        {
            string codigo = id;
            List<Esp_Usu_Models> lista = new List<Esp_Usu_Models>();
            Esp_Usu_Models item = new Esp_Usu_Models();

            if (idAcc != null)
            {
                foreach (string c in idAcc)
                {
                    item.idEsp = id;
                    item.idAcc = c;
                    item.usuCrea = SessionPersister.Username;
                    item.usufchCrea = DateTime.Now;
                    lista.Add(item);
                    item = new Esp_Usu_Models();
                }
                _espU.crear(lista);
            }
            return RedirectToAction("EspecialidadUsuario", new { id = codigo, esp = nombre });
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult EliminarUsuarios(string[] idAccN, string id, string nombre)
        {
            string codigo = id;
            if (idAccN != null)
            {
                _espU.eliminar(idAccN,id);
            }
            return RedirectToAction("EspecialidadUsuario", new { id = codigo, esp = nombre });
        }
    }
}