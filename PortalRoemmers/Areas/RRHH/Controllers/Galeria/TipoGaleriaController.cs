using PortalRoemmers.Areas.RRHH.Models.Galeria;
using PortalRoemmers.Areas.RRHH.Services;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.IO;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.Galeria
{
    public class TipoGaleriaController : Controller
    {
        private TipoGaleriaRepositorio _tgal;
        private Ennumerador enu;
        public TipoGaleriaController()
        {
            _tgal = new TipoGaleriaRepositorio();
            enu = new Ennumerador();
        }

        [CustomAuthorize(Roles = "000003,000264")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _tgal.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000265")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Registrar(TipoGaleriaModels model)
        {
            if (ModelState.IsValid)
            {
                string tabla = "tb_TipGal";
                int idc = enu.buscarTabla(tabla);
                model.idTipGal = idc.ToString("D7");
                model.usuCre = SessionPersister.Username;
                model.usufchCre = DateTime.Now;
                if (_tgal.crear(model))
                {
                    //crear carpeta
                    string path = "~/Content/Home/Galeria";
                    bool exists = Directory.Exists(Server.MapPath(path));
                    if (!exists) Directory.CreateDirectory(Server.MapPath(path));
                    string subpath = path + "/" + model.idTipGal + "/";
                    bool existssub = Directory.Exists(Server.MapPath(subpath));
                    if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));

                    enu.actualizarTabla(tabla, idc);
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error al crear registro " + "</div>";
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

                return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000266")]
        public ActionResult Modificar(string id)
        {
            var model = _tgal.obtenerItem(id);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Modificar(TipoGaleriaModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;
                Boolean listo = _tgal.modificar(model);
                if (listo)
                {
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se modificó el registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>Error al modificar.</div>";
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
                return View(model);
        }

        [CustomAuthorize(Roles = "000003,000267")]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult Eliminar(string id)
        {
            var model = _tgal.obtenerItem(id);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Eliminar(TipoGaleriaModels model)
        {

            if (_tgal.eliminar(model.idTipGal))
            {
                string subpath = "~/Content/Home/Galeria/" + model.idTipGal;
                Directory.Delete(Server.MapPath(subpath), true);
                TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se eliminó el registro.</div>";
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'> Error al eliminar el registro.</div>";
            }

                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

    }
}