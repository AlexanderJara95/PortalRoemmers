using PortalRoemmers.Areas.RRHH.Models.Documento;
using PortalRoemmers.Areas.RRHH.Services.DocumentoRRHH;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.DocumentoRRHH
{
    public class DocumentoRRHHController : Controller
    {
        private DocumentoRRHHRepositorio _doc;
        private Ennumerador _enu;
        private TipoDocumentoRRHHRepositorio _tdoc;
        public DocumentoRRHHController()
        {
            _doc = new DocumentoRRHHRepositorio();
            _enu = new Ennumerador();
            _tdoc = new TipoDocumentoRRHHRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000399")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _doc.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000400")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.tipoDocumento = new SelectList(_tdoc.obtenerTipoEnlance(), "idTipDoc", "nomTipDoc");
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(DocumentoRRHHModels model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string tabla = "tb_DocRRHH";
                int idc = _enu.buscarTabla(tabla);
                model.idDoc = idc.ToString("D7");
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                if (file != null)
                {
                    //crear carpeta
                    string path = "~/Content/Home/Documento";
                    bool exists = Directory.Exists(Server.MapPath(path));
                    if (!exists) Directory.CreateDirectory(Server.MapPath(path));
                    string subpath = path + "/" + model.idTipDoc + "/";
                    bool existssub = Directory.Exists(Server.MapPath(subpath));
                    if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));
                    string absolutePath = subpath + file.FileName;
                    file.SaveAs(Server.MapPath(absolutePath));
                    string relativePath = absolutePath.Replace("~/", "../");
                    model.rutaDoc = relativePath;
                    //////////
                }

                if (_doc.crear(model))
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
            ViewBag.tipoDocumento = new SelectList(_tdoc.obtenerTipoEnlance(), "idTipDoc", "nomTipDoc", model.idTipDoc);
            return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000401")]
        public ActionResult Modificar(string id)
        {
            var model = _doc.obtenerItem(id);
            ViewBag.tipoDocumento = new SelectList(_tdoc.obtenerTipoEnlance(), "idTipDoc", "nomTipDoc", model.idTipDoc);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(DocumentoRRHHModels model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                if (file != null)
                {

                    model.rutaDoc = model.rutaDoc.Replace("../", "~/");
                    //elimino archivo
                    System.IO.File.Delete(Server.MapPath(model.rutaDoc));
                    //crear carpeta
                    string subpath = "~/Content/Home/Documento" + "/" + model.idTipDoc + "/";
                    bool existssub = Directory.Exists(Server.MapPath(subpath));
                    if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));
                    string absolutePath = subpath + file.FileName;
                    file.SaveAs(Server.MapPath(absolutePath));
                    string relativePath = absolutePath.Replace("~/", "../");
                    model.rutaDoc = relativePath;
                    //////////
                }

                Boolean listo = _doc.modificar(model);
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
            ViewBag.tipoEnlace = new SelectList(_tdoc.obtenerTipoEnlance(), "idTipDoc", "nomTipDoc", model.idTipDoc);
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000402")]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult Eliminar(string id)
        {
            var model = _doc.obtenerItem(id);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Eliminar(DocumentoRRHHModels model)
        {
            if (_doc.eliminar(model.idDoc))
            {
                //elimino la imagen
                model.rutaDoc = model.rutaDoc.Replace("../", "~/");
                System.IO.File.Delete(Server.MapPath(model.rutaDoc));

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