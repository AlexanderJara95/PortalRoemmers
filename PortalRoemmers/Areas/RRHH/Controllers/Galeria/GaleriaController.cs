using PortalRoemmers.Areas.RRHH.Models.Galeria;
using PortalRoemmers.Areas.RRHH.Services;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.Galeria
{
    public class GaleriaController : Controller
    {

        private GaleriaRepositorio _gal;
        private Ennumerador enu;
        private TipoGaleriaRepositorio _tgal;
        public GaleriaController()
        {
            _gal = new GaleriaRepositorio();
            enu = new Ennumerador();
            _tgal = new TipoGaleriaRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000269")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _gal.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000270")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.tipgaleria = new SelectList(_tgal.obtenerTipoGalerias(), "idTipGal", "titTipGal");
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Registrar(GaleriaModels model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string tabla = "tb_Galeria";
                int idc = enu.buscarTabla(tabla);
                model.idGaleria = idc.ToString("D7");
                model.usuCre = SessionPersister.Username;
                model.usufchCre = DateTime.Now;

                if (file!=null) {
                //crear carpeta
                string path = "~/Content/Home/Galeria";
                bool exists = Directory.Exists(Server.MapPath(path));
                if (!exists) Directory.CreateDirectory(Server.MapPath(path));
                string subpath = path + "/" + model.idTipGal + "/";
                bool existssub = Directory.Exists(Server.MapPath(subpath));
                if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));
                string absolutePath = subpath + file.FileName;
                file.SaveAs(Server.MapPath(absolutePath));
                string relativePath = absolutePath.Replace("~/", "../");
                model.rutaGaleria = relativePath;
                    //////////
                }

                if (_gal.crear(model))
                {
                    enu.actualizarTabla(tabla, idc);
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error al crear registro " + "</div>";
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.tipgaleria = new SelectList(_tgal.obtenerTipoGalerias(), "idTipGal", "titTipGal",model.idTipGal);
            return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000271")]
        public ActionResult Modificar(string id)
        {
            var model = _gal.obtenerItem(id);
            ViewBag.tipgaleria = new SelectList(_tgal.obtenerTipoGalerias(), "idTipGal", "titTipGal", model.idTipGal);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Modificar(GaleriaModels model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                if (file != null)
                {

                    model.rutaGaleria = model.rutaGaleria.Replace("../","~/");
                    //elimino la imagen antigua
                    System.IO.File.Delete(Server.MapPath(model.rutaGaleria));
                    //crear carpeta
                    string path = "~/Content/Home/Galeria";
                    bool exists = Directory.Exists(Server.MapPath(path));
                    if (!exists) Directory.CreateDirectory(Server.MapPath(path));
                    string subpath = path + "/" + model.idTipGal + "/";
                    bool existssub = Directory.Exists(Server.MapPath(subpath));
                    if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));
                    string absolutePath = subpath + file.FileName;
                    file.SaveAs(Server.MapPath(absolutePath));
                    string relativePath = absolutePath.Replace("~/", "../");
                    model.rutaGaleria = relativePath;
                    //////////
                }

                Boolean listo = _gal.modificar(model);
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
            ViewBag.tipgaleria = new SelectList(_tgal.obtenerTipoGalerias(), "idTipGal", "titTipGal", model.idTipGal);
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000272")]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult Eliminar(string id)
        {
            var model = _gal.obtenerItem(id);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Eliminar(GaleriaModels model)
        {
            if (_gal.eliminar(model.idGaleria))
            {
                //elimino la imagen
                model.rutaGaleria = model.rutaGaleria.Replace("../", "~/");
                System.IO.File.Delete(Server.MapPath(model.rutaGaleria));
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