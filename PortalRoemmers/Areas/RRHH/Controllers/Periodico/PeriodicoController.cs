using PortalRoemmers.Areas.RRHH.Models.Periodico;
using PortalRoemmers.Areas.RRHH.Services;
using PortalRoemmers.Areas.RRHH.Services.Periodico;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.Periodico
{
    public class PeriodicoController : Controller
    {
        PeriodicoMuralRepositorio _per;
        EfectoImagenRepositorio _efe;
        private Ennumerador enu;
        public PeriodicoController()
        {
             enu = new Ennumerador();
            _per = new PeriodicoMuralRepositorio();
            _efe = new EfectoImagenRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000274")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _per.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000275")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.efecto= new SelectList(_efe.obtenerEfectosImagen(), "idEfeIma", "titEfeIma");
            ViewBag.caida = new SelectList(ListadoCaida(), "Value", "Text");
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Registrar(PeriodicoSeccionModels model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string tabla = "tb_PeriodicoSec";
                int idc = enu.buscarTabla(tabla);
                model.idPerSec = idc.ToString("D7");
                model.usuCre = SessionPersister.Username;
                model.usufchCre = DateTime.Now;
                if (file != null)
                {
                    //crear carpeta
                    string path = "~/Content/Home/Periodico";
                    bool exists = Directory.Exists(Server.MapPath(path));
                    if (!exists) Directory.CreateDirectory(Server.MapPath(path));
                    string subpath = path + "/Seccion/";
                    bool existssub = Directory.Exists(Server.MapPath(subpath));
                    if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));
                    string absolutePath = subpath + file.FileName;
                    file.SaveAs(Server.MapPath(absolutePath));
                    string relativePath = absolutePath.Replace("~/", "../");
                    model.rutPerSec = relativePath;
                    //////////
                }

                if (_per.crear(model))
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
            ViewBag.efecto = new SelectList(_efe.obtenerEfectosImagen(), "idEfeIma", "titEfeIma",model.idEfeIma);
            ViewBag.caida = new SelectList(ListadoCaida(), "Value", "Text",model.caiPerSec);
            return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000276")]
        public ActionResult Modificar(string id)
        {
            var model = _per.obtenerItem(id);
            ViewBag.efecto = new SelectList(_efe.obtenerEfectosImagen(), "idEfeIma", "titEfeIma", model.idEfeIma);
            ViewBag.caida = new SelectList(ListadoCaida(), "Value", "Text", model.caiPerSec);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Modificar(PeriodicoSeccionModels model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;
                if (file != null)
                {
                    //elimino la imagen antigua
                    model.rutPerSec = model.rutPerSec.Replace("../", "~/");
                    System.IO.File.Delete(Server.MapPath(model.rutPerSec));
                    //crear carpeta
                    string path = "~/Content/Home/Periodico";
                    bool exists = Directory.Exists(Server.MapPath(path));
                    if (!exists) Directory.CreateDirectory(Server.MapPath(path));
                    string subpath = path + "/Seccion/";
                    bool existssub = Directory.Exists(Server.MapPath(subpath));
                    if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));
                    string absolutePath = subpath + file.FileName;
                    file.SaveAs(Server.MapPath(absolutePath));
                    string relativePath = absolutePath.Replace("~/", "../");
                    model.rutPerSec = relativePath;
                    //////////
                }


                Boolean listo = _per.modificar(model);
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
            ViewBag.efecto = new SelectList(_efe.obtenerEfectosImagen(), "idEfeIma", "titEfeIma", model.idEfeIma);
            ViewBag.caida = new SelectList(ListadoCaida(), "Value", "Text", model.caiPerSec);
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000277")]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult Eliminar(string id)
        {
            var model = _per.obtenerItem(id);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Eliminar(PeriodicoSeccionModels model)
        {
            if (_per.eliminar(model.idPerSec))
            {
                //elimino la imagen
                model.rutPerSec = model.rutPerSec.Replace("../", "~/");
                System.IO.File.Delete(Server.MapPath(model.rutPerSec));
                TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se eliminó el registro.</div>";
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'> Error al eliminar el registro.</div>";
            }

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        private SelectList ListadoCaida()
        {
            //creamos una lista tipo SelectListItem
            List<SelectListItem> lst = new List<SelectListItem>();

            //De la siguiente manera llenamos manualmente,
            //Siendo el campo Text lo que ve el usuario y
            //el campo Value lo que en realidad vale nuestro valor
            lst.Add(new SelectListItem() { Text = "Caída de la imagen hacia la derecha", Value = "caidaderecha" });
            lst.Add(new SelectListItem() { Text = "Caída de la imagen hacia la izquierda", Value = "caidaizquierda" });

            //Agregamos la lista a nuestro SelectList
            SelectList miSL = new SelectList(lst, "Value", "Text");

            return miSL;
        }

    }
}