using PortalRoemmers.Areas.Sistemas.Models.Enlace;
using PortalRoemmers.Areas.Sistemas.Services.Enlace;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Enlace
{
    public class EnlaceController : Controller
    {

        private EnlaceRepositorio _enl;
        private Ennumerador _enu;
        private TipoEnlaceRepositorio _ten;
        public EnlaceController()
        {
            _enl = new EnlaceRepositorio();
            _enu = new Ennumerador();
            _ten = new TipoEnlaceRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000299")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _enl.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000300")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.tipoEnlace = new SelectList(_ten.obtenerTipoEnlance(), "idTEnl", "nomTEnl");
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(EnlaceModels model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string tabla = "tb_Enlace";
                int idc = _enu.buscarTabla(tabla);
                model.idEnl = idc.ToString("D7");
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                if (file != null)
                {
                    //crear carpeta
                    string path = "~/Content/Home/Enlace";
                    bool exists = Directory.Exists(Server.MapPath(path));
                    if (!exists) Directory.CreateDirectory(Server.MapPath(path));
                    string subpath = path + "/" + model.idTEnl + "/";
                    bool existssub = Directory.Exists(Server.MapPath(subpath));
                    if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));
                    string absolutePath = subpath + file.FileName;
                    file.SaveAs(Server.MapPath(absolutePath));
                    string relativePath = absolutePath.Replace("~/", "../");
                    model.rutaEnl = relativePath;
                    //////////
                }

                if (_enl.crear(model))
                {
                    _enu.actualizarTabla(tabla, idc);
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error al crear registro " + "</div>";
                }
                
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.tipoEnlace = new SelectList(_ten.obtenerTipoEnlance(), "idTEnl", "nomTEnl", model.idTEnl);
            return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000301")]
        public ActionResult Modificar(string id)
        {
            var model = _enl.obtenerItem(id);
            ViewBag.tipoEnlace = new SelectList(_ten.obtenerTipoEnlance(), "idTEnl", "nomTEnl", model.idTEnl);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(EnlaceModels model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                if (file != null)
                {

                    model.rutaEnl = model.rutaEnl.Replace("../", "~/");
                    //elimino archivo
                    System.IO.File.Delete(Server.MapPath(model.rutaEnl));
                    //crear carpeta
                    string subpath = "~/Content/Home/Enlace" + "/" + model.idTEnl + "/";
                    bool existssub = Directory.Exists(Server.MapPath(subpath));
                    if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));
                    string absolutePath = subpath + file.FileName;
                    file.SaveAs(Server.MapPath(absolutePath));
                    string relativePath = absolutePath.Replace("~/", "../");
                    model.rutaEnl = relativePath;
                    //////////
                }

                Boolean listo = _enl.modificar(model);
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
            ViewBag.tipoEnlace = new SelectList(_ten.obtenerTipoEnlance(), "idTEnl", "nomTEnl", model.idTEnl);
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000302")]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult Eliminar(string id)
        {
            var model = _enl.obtenerItem(id);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Eliminar(EnlaceModels model)
        {
            if (_enl.eliminar(model.idEnl))
            {
                //elimino la imagen
                model.rutaEnl = model.rutaEnl.Replace("../", "~/");
                System.IO.File.Delete(Server.MapPath(model.rutaEnl));

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