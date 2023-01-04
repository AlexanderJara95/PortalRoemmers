using PortalRoemmers.Areas.RRHH.Models.Boleta;
using PortalRoemmers.Areas.RRHH.Services.Boleta;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.Boleta
{
    public class BoletaPersonalController : Controller
    {
        private BoletaPersonalRepositorio _bol;
        private BoletaDetalleRepositorio _dbol;
        public BoletaPersonalController()
        {
            _bol = new BoletaPersonalRepositorio();
            _dbol = new BoletaDetalleRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000334")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _bol.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000335")]
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public ActionResult Registrar(BoletaPersonalModels model)
        {
            //Cargo los documentos 
            Ennumerador enu = new Ennumerador();
            //creo su ID
            string tabla = "tb_BolPer";
            int idc = enu.buscarTabla(tabla);
            model.idBolPer = idc.ToString("D7");
            model.usuCre = SessionPersister.Username;
            model.usufchCre = DateTime.UtcNow;

            List<BoletaDetalleModels> boletas = new List<BoletaDetalleModels>();
            //creo la carpeta
            bool exists = Directory.Exists(Server.MapPath(model.rutBolPer));
            if (!exists) Directory.CreateDirectory(Server.MapPath(model.rutBolPer));
            
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string absolutePath = model.rutBolPer + "/" + fileName;

                    BoletaDetalleModels boleta = new BoletaDetalleModels()
                    {
                        idBolPer = model.idBolPer,
                        nroDocBolDet = fileName.Split('-')[0],
                        rutBolDet= absolutePath.Replace("~/", "../"),
                        nomBolDet= fileName,
                        usuCre=SessionPersister.Username,
                        usufchCre=DateTime.UtcNow
                    };
                    boletas.Add(boleta);
                    file.SaveAs(Server.MapPath(absolutePath));
                }
            }

            model.detalle = boletas;
            model.rutBolPer = model.rutBolPer.Replace("~/", "../");
            if (ModelState.IsValid)
            {
                if (_bol.crear(model))
                {
                    enu.actualizarTabla(tabla, idc);
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error al crear detalle " + "</div>";
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000336")]
        public ActionResult Modificar(string id)
        {
            var model = _bol.obtenerItem(id);

            model.rutBolPerMod = model.rutBolPer;
            ViewBag.cantidad = model.detalle.Count;

            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(BoletaPersonalModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;
                model.rutBolPer = model.rutBolPer.Replace("~/", "../");

                if (_bol.modificar(model))
                {
                    if (model.rutBolPer != model.rutBolPerMod) //valido si el nombre cambio
                    {
                        if (Directory.Exists(Server.MapPath(model.rutBolPerMod.Replace("../", "~/"))))
                        {
                            Directory.Move(Server.MapPath(model.rutBolPerMod.Replace("../", "~/")), Server.MapPath(model.rutBolPer.Replace("../", "~/")));
                            _dbol.actualizarRuta(model.idBolPer, model.rutBolPer + "/");
                        }
                    }
                    if (Request.Files[0].ContentLength != 0)
                        { //verifico si hay archivos nuevos
                            for (int i = 0; i < Request.Files.Count; i++)
                            {
                                var file = Request.Files[i];
                                if (file != null && file.ContentLength > 0)
                                {
                                    var fileName = Path.GetFileName(file.FileName);
                                    string absolutePath = model.rutBolPer.Replace("../", "~/") + "/" + fileName;

                                    BoletaDetalleModels boleta = new BoletaDetalleModels()
                                    {
                                        idBolPer = model.idBolPer,
                                        nroDocBolDet = fileName.Split('-')[0],
                                        rutBolDet = absolutePath.Replace("~/", "../"),
                                        nomBolDet = fileName,
                                    };
                                    _dbol.mergeDetalle(boleta);
                                    file.SaveAs(Server.MapPath(absolutePath));
                                }
                            }
                        }
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se modificó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "< div id = 'warning' class='alert alert-warning'>Error al modificar el registro.</div>";
                }



                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

            return View(model);
        }

        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000337")]
        public ActionResult Eliminar(string id)
        {
            var model = _bol.obtenerItem(id);
            ViewBag.cantidad = model.detalle.Count;
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Eliminar(BoletaPersonalModels model)
        {

            if (_bol.eliminar(model))
            {
                Directory.Delete(Server.MapPath(model.rutBolPer.Replace("../", "~/")), true);
                TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se eliminó el registro.</div>";
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'> Error al eliminar el registro.</div>";
            }

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000345")]
        public JsonResult Enviar(string id)
        {
            var model = _bol.obtenerItem(id);
            EmailHelper m = new EmailHelper();
            EmpleadoRepositorio emp = new EmpleadoRepositorio();

            string titulo = model.titBolPer;


            string respuesta = "";
          
            List<BoletaDetalleModels> modificado = new List<BoletaDetalleModels>();
            foreach (var d in model.detalle.Where(x => x.estBolDet == false))
            {
                var empDat = emp.obtenerxDniEmpleado(d.nroDocBolDet);
                if (empDat != null)
                {

                    string mensaje = string.Format("<section> Estimado (a) <span style=\"font - weight: bold;\">{0}</span><BR/> <p>El documento {1} ya se encuentra en el portal  <BR/> Para visualizar tus boletas ingresa a  <a href=\"{2}\" title=\"Megalabs-latam\">Megalas-latam.com.pe</a> </p>  </section>", empDat.nomComEmp, model.titBolPer, Url.Action("Index", "Account", new { Area = "" }, Request.Url.Scheme));
                    bool ok = false;
                    try
                    {
                        var correo = empDat.usuarios.Where(x => x.email.ToUpper().Contains("MEGALABS.COM.PE")).Select(x => x.email).FirstOrDefault();
                        ok = m.SendEmail(correo, mensaje, titulo, ConstCorreo.CORREOBOLETA, ConstCorreo.CLAVE_BOLETA);

                    }
                    catch (Exception e)
                    {
                        respuesta += "Error al enviar correo: " + empDat.nomComEmp + "|";
                    }

                    if (ok)
                    {
                        d.estBolDet = true;
                        _dbol.modificar(d);
                    }
                }
                else
                {
                    respuesta += "No existe el usuario: " + d.nroDocBolDet + "|";
                }
            }
          
            
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }


    }
}