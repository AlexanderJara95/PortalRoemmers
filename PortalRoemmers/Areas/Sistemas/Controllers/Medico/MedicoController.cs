using PortalRoemmers.Areas.Sistemas.Models.Medico;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Medico;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System.Linq;
using System.Web.Mvc;
using System;
using PortalRoemmers.Areas.Sistemas.Services.Visitador;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Medico
{
    public class MedicoController : Controller
    {//CLIENTECONTROLADOR 000008
        private MedicoRepositorio _cli;
        private TipDocIdeRepositorio _ide;
        private GeneroRepositorio _gen;
        private EstadoRepositorio _est;
        private TipMedRepositorio _tipcli;
        private EspecialidadRepositorio _esp;
        public MedicoController()
        {
            _ide = new TipDocIdeRepositorio();
            _esp = new EspecialidadRepositorio();
            _cli = new MedicoRepositorio();
            _gen = new GeneroRepositorio();
            _est = new EstadoRepositorio();
            _tipcli = new TipMedRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000009")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _cli.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000010")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.tipDoc = new SelectList(_ide.obtenerTipDocs(), "idTipDoc", "nomTipDoc");
            ViewBag.sexoUsu = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen");
            ViewBag.idTipCli = new SelectList(_tipcli.obtenerTipoCliente(), "idTipCli", "nomTipCli");
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", ConstantesGlobales.estadoActivo);
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(MedicoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                TempData["mensaje"] = _cli.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.tipDoc = new SelectList(_ide.obtenerTipDocs(), "idTipDoc", "nomTipDoc");
            ViewBag.sexoUsu = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen");
            ViewBag.idTipCli = new SelectList(_tipcli.obtenerTipoCliente(), "idTipCli", "nomTipCli");
            ViewBag.Est = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idTipCli);
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000011")]
        public ActionResult Modificar(string id)
        {
            var model = _cli.obtenerItem(id);

            ViewBag.tipDoc = new SelectList(_ide.obtenerTipDocs(), "idTipDoc", "nomTipDoc", model.idTipDoc);
            ViewBag.sexoUsu = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen", model.idGen);
            ViewBag.idTipCli = new SelectList(_tipcli.obtenerTipoCliente(), "idTipCli", "nomTipCli", model.idTipCli);
            ViewBag.estado = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst",model.idTipCli);

            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(MedicoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;
                TempData["mensaje"] = _cli.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.tipDoc = new SelectList(_ide.obtenerTipDocs(), "idTipDoc", "nomTipDoc", model.idTipDoc);
            ViewBag.sexoUsu = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen", model.idGen);
            ViewBag.idTipCli = new SelectList(_tipcli.obtenerTipoCliente(), "idTipCli", "nomTipCli", model.idTipCli);
            ViewBag.estado = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idTipCli);

            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000012")]
        public ActionResult Eliminar(string id)
        {
            var model = _cli.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(MedicoModels model)
        {
            TempData["mensaje"] = _cli.eliminar(model.idCli);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        //Json
        public JsonResult SearchCliente(string buscar)
        {
            var s = _cli.obtenerClientes().Where(x => x.nomCli.ToUpper().Contains(buscar.ToUpper())&&x.idEst==ConstantesGlobales.estadoActivo).Select(z => new { codigo = z.idCli, nombre = z.nomCli, descripcion = z.nroMatCli }).Take(1000);
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        //Agregando Especialidad
        public JsonResult SearchClienteUpdate(string buscar)
        {
            var s = _cli.obtenerClientes().Where(x => x.nomCli.ToUpper().Contains(buscar.ToUpper()) && x.idEst == ConstantesGlobales.estadoActivo).Select(z => new { codigo = z.idCli, nombre = z.nomCli, descripcion = z.nroMatCli, especialidad = _esp.obtenerItem(z.idEsp ?? "99").nomEsp}).Take(1000);
            return Json(s, JsonRequestBehavior.AllowGet);
        }
    }
}