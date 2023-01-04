
using PortalRoemmers.Areas.RRHH.Models.Bienvenida;
using PortalRoemmers.Areas.RRHH.Services;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Data.SqlTypes;

namespace PortalRoemmers.Areas.RRHH.Controllers.Bienvenida
{
    public class BienvenidaController : Controller
    {
        private Ennumerador enu;
        private BienvenidaRepositorio _bien;

        public BienvenidaController()
        {
            _bien = new BienvenidaRepositorio();
            enu = new Ennumerador();

        }

        [CustomAuthorize(Roles = "000003,000258")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _bien.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000260")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Registrar(BienvenidaModels model, List<HttpPostedFileBase> file)
        {
            model.titbien = model.titbien.Trim();
            model.desbien = model.desbien.Trim();
            model.usuCre= SessionPersister.Username;
            model.usufchCre = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (file[0] != null )
                {
                    string tabla = "tb_Bienvenida";
                    int idc = enu.buscarTabla(tabla);
                    model.idbien = idc.ToString("D7");
                    Boolean listo = _bien.crear(model);
                    if (listo)
                    {
                        enu.actualizarTabla(tabla, idc);
                        string path = "~/Content/Home/Bienvenida";
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists) Directory.CreateDirectory(Server.MapPath(path));

                        string subpath = path + "/" + model.idbien + "/";
                        bool existssub = Directory.Exists(Server.MapPath(subpath));
                        if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));


                        FotosBienvenidaModels fot = new FotosBienvenidaModels();
                        List<FotosBienvenidaModels> fots = new List<FotosBienvenidaModels>();
                        int id = 1;
                        foreach (var f in file)
                        {
                            string absolutePath = subpath + f.FileName;
                            f.SaveAs(Server.MapPath(absolutePath));
                            string relativePath = absolutePath.Replace("~/", "../");

                            fot.idbien = model.idbien;
                            fot.idFotBie = id.ToString("D7");
                            fot.rutaFotBie = relativePath;
                            if (id == 1) { fot.actFotBie = true; }
                            else { fot.actFotBie = false; }
                            fots.Add(fot);
                            fot = new FotosBienvenidaModels();
                            id++;
                        }
                        if (_bien.crearDetalle(fots))
                        {
                            TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                        }
                        else
                        {
                            TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error al crear detalle " + "</div>";
                        }
                        return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
                    }
                }
                else
                {
                    ViewBag.titulo = model.titbien;
                    ViewBag.descripcion = model.desbien;
                    TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Se debe cargar imagenes" + "</div>";
                    return View();
                }
            }

            ViewBag.titulo = model.titbien;
            ViewBag.descripcion = model.desbien;
            return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000261")]
        public ActionResult Modificar(string id)
        {
            var model=_bien.obtenerItem(id);
            ViewBag.titulo = model.titbien;
            ViewBag.descripcion = model.desbien;
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Modificar(BienvenidaModels model, List<HttpPostedFileBase> file)
        {
            model.titbien = model.titbien.Trim();
            model.desbien = model.desbien.Trim();
            model.usuMod = SessionPersister.Username;
            model.usufchMod = DateTime.Now;
            if (ModelState.IsValid)
            {
                Boolean listo = _bien.modificar(model);
                if (listo)
                {
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se modificó el registro.</div>";
                    if (file[0] != null)
                    {
                        string path = "~/Content/Home/Bienvenida";
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists) Directory.CreateDirectory(Server.MapPath(path));

                        string subpath = path + "/" + model.idbien + "/";
                        bool existssub = Directory.Exists(Server.MapPath(subpath));
                        if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));

                        //elimino todo el detalle
                        _bien.eliminarDetalle(model.idbien);
                        FotosBienvenidaModels fot = new FotosBienvenidaModels();
                        List<FotosBienvenidaModels> fots = new List<FotosBienvenidaModels>();
                        int id = 1;
                        foreach (var f in file)
                        {
                            string absolutePath = subpath + f.FileName;
                            f.SaveAs(Server.MapPath(absolutePath));
                            string relativePath = absolutePath.Replace("~/", "../");

                            fot.idbien = model.idbien;
                            fot.idFotBie = id.ToString("D7");
                            fot.rutaFotBie = relativePath;
                            if (id == 1) { fot.actFotBie = true; }
                            else { fot.actFotBie = false; }
                            fots.Add(fot);
                            fot = new FotosBienvenidaModels();
                            id++;
                        }
                        if (_bien.crearDetalle(fots))
                        {
                            TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se modificó el registro y detalle.</div>";
                        }
                        else
                        {
                            TempData["mensaje"] = "<div id='warning' class='alert alert-warning'> Error al crear detalle.</div>";
                        }
                    }
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>Error al modificar.</div>";
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.titulo = model.titbien;
            ViewBag.descripcion = model.desbien;
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000262")]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult Eliminar(string id)
        {
            var model = _bien.obtenerItem(id);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Eliminar(BienvenidaModels model)
        {
            Boolean ok = _bien.eliminar(model.idbien);
            if (ok)
            {
                string subpath = "~/Content/Home/Bienvenida/"+ model.idbien;
                Directory.Delete(Server.MapPath(subpath), true);
                TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se eliminó el registro.</div>";
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'> Error al eliminar el registro.</div>";
            }

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000344")]
        public JsonResult activarBienvenida(string idbien)
        {
            return Json(_bien.actualizarBienvenida(idbien), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000355")]
        public JsonResult obtenerFilesLinks(string id)
        {
            var rutas = _bien.obtenerFotosBien(id).Select(x => new { id = x.idFotBie, path = x.rutaFotBie, link = x.linkFotBie });
            return Json(rutas, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000356")]
        public JsonResult guardarLink(string idBien, string idFotoB, string link)
        {
            var resultado = _bien.actualizarFotoBienvenida(idBien, idFotoB, link);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
    }
}