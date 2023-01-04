using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Sistemas.Services.Gasto;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Gasto
{
    public class TipoGastoController : Controller
    {//TIPOGASTOCONTROLLER 000053
        private TipoGastoRepositorio _tgas;
        private EstadoRepositorio _est;
        private UsuarioRepositorio _usu;
        private TipoGasto_UsuRepositorio _tipGastoU;
        public TipoGastoController()
        {
            _tgas = new TipoGastoRepositorio();
            _est = new EstadoRepositorio();
            _usu = new UsuarioRepositorio();
            _tipGastoU = new TipoGasto_UsuRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000054")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tgas.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000055")]
        [HttpGet]
        public ActionResult Registrar()
        {
            
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(TipoGastoModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchCreTipGas = DateTime.Now;
                model.userCreTipGas = SessionPersister.Username;
                model.idEst = ConstantesGlobales.estadoActivo;
                TempData["mensaje"] = _tgas.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000056")]
        public ActionResult Modificar(string id)
        {
            var model = _tgas.obtenerItem(id);
            ViewBag.EstU = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(TipoGastoModels model)
        {
            if (ModelState.IsValid)
            {
                model.fchModTipGas = DateTime.Now;
                model.userModTipGas = SessionPersister.Username;
                TempData["mensaje"] = _tgas.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.EstU = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000057")]
        public ActionResult Eliminar(string id)
        {
            var model = _tgas.obtenerItem(id);
            
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(TipoGastoModels model)
        {
            TempData["mensaje"] = _tgas.eliminar(model.idTipGas);
            
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000236")]
        public ActionResult TipoGastoUsuario(string id, string tip)
        {
            var usuarios = _usu.obtenerUsuarios();
            var seleccionados = _tipGastoU.obtenerUsuarioxTipoDeGasto(id).Select(x => new { x.idAcc, nomComEmp = x.cuentaDeUsuario.empleado.nomComEmp + " (" + x.cuentaDeUsuario.username + ")" });

            string[] selec = seleccionados.Select(x => x.idAcc.ToString()).ToArray();

            var Nseleccionados = (from u in usuarios
                                  where !selec.Contains(u.idAcc.ToString())
                                  select new { u.idAcc, nomComEmp = u.empleado.nomComEmp + " (" + u.username + ")" }).ToList();

            ViewBag.idTipGas = id;
            ViewBag.tipoDeGasto = tip;
            ViewBag.disponible = new SelectList(Nseleccionados, "idAcc", "nomComEmp");
            ViewBag.elegido = new SelectList(seleccionados, "idAcc", "nomComEmp");

            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult AgregarUsuarios(string[] idAcc, string id, string nombre)
        {
            string codigo = id;
            List<TipGas_Usu_Models> lista = new List<TipGas_Usu_Models>();
            TipGas_Usu_Models item = new TipGas_Usu_Models();

            if (idAcc != null)
            {
                foreach (string c in idAcc)
                {
                    item.idTipGas = id;
                    item.idAcc = c;
                    item.usuCrea = SessionPersister.Username;
                    item.usufchCrea = DateTime.Now;
                    lista.Add(item);
                    item = new TipGas_Usu_Models();
                }
                _tipGastoU.crear(lista);
            }
            return RedirectToAction("TipoGastoUsuario", new { id = codigo, tip = nombre });
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult EliminarUsuarios(string[] idAccN, string id, string nombre)
        {
            string codigo = id;
            if (idAccN != null)
            {
                _tipGastoU.eliminar(idAccN, id);
            }
            return RedirectToAction("TipoGastoUsuario", new { id = codigo, tip = nombre });
        }
    }
}