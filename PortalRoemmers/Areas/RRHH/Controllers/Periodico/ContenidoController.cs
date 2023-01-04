using PortalRoemmers.Areas.RRHH.Models.Periodico;
using PortalRoemmers.Areas.RRHH.Services;
using PortalRoemmers.Areas.RRHH.Services.Periodico;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.Periodico
{
    public class ContenidoController : Controller
    {
        private Ennumerador enu;
        private ContenidoRepositorio _con;
        private PeriodicoMuralRepositorio _per;
        public ContenidoController()
        {
            enu = new Ennumerador();
            _con = new ContenidoRepositorio();
            _per = new PeriodicoMuralRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000279")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _con.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000280")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.contenido = new SelectList(_per.obtenerPeriodicoVacio(), "idPerSec", "titPerSec");
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Registrar(ContenidoSeccionModels model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                model.usuCre = SessionPersister.Username;
                model.usufchCre = DateTime.Now;

                if (file != null)
                {
                    //crear carpeta
                    string path = "~/Content/Home/Periodico";
                    bool exists = Directory.Exists(Server.MapPath(path));
                    if (!exists) Directory.CreateDirectory(Server.MapPath(path));
                    string subpath = path + "/Contenido/";
                    bool existssub = Directory.Exists(Server.MapPath(subpath));
                    if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));
                    string absolutePath = subpath + file.FileName;
                    file.SaveAs(Server.MapPath(absolutePath));
                    string relativePath = absolutePath.Replace("~/", "../");
                    model.rutConSec = relativePath;
                    //////////
                }

                if (_con.crear(model))
                {
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error al crear registro " + "</div>";
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.descripcion = model.desConSec;
            ViewBag.contenido = new SelectList(_per.obtenerPeriodicoVacio(), "idPerSec", "titPerSec",model.idConSec);
            return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000281")]
        public ActionResult Modificar(string id)
        {
            var model = _con.obtenerItem(id);
            ViewBag.descripcion = model.desConSec;
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Modificar(ContenidoSeccionModels model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                if (file != null)
                {
                    //elimino la imagen antigua
                    if (model.rutConSec!=null)
                    {
                        model.rutConSec = model.rutConSec.Replace("../", "~/");
                        System.IO.File.Delete(Server.MapPath(model.rutConSec));
                    }
                    
                    //crear carpeta
                    string path = "~/Content/Home/Periodico";
                    bool exists = Directory.Exists(Server.MapPath(path));
                    if (!exists) Directory.CreateDirectory(Server.MapPath(path));
                    string subpath = path + "/Contenido/";
                    bool existssub = Directory.Exists(Server.MapPath(subpath));
                    if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));
                    string absolutePath = subpath + file.FileName;
                    file.SaveAs(Server.MapPath(absolutePath));
                    string relativePath = absolutePath.Replace("~/", "../");
                    model.rutConSec = relativePath;
                    //////////
                }

                Boolean listo = _con.modificar(model);
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
            ViewBag.contenido = new SelectList(_per.obtenerPeriodicoVacio(), "idPerSec", "titPerSec", model.idConSec);
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000282")]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult Eliminar(string id)
        {
            var model = _con.obtenerItem(id);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Eliminar(ContenidoSeccionModels model)
        {
            if (_con.eliminar(model.idConSec))
            {
                //elimino la imagen
                model.rutConSec = model.rutConSec.Replace("../", "~/");
                System.IO.File.Delete(Server.MapPath(model.rutConSec));
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