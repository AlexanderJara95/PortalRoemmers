using PortalRoemmers.Areas.Almacen.Models.Inventario;
using PortalRoemmers.Areas.Almacen.Services.Inventario;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using SpreadsheetLight;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Almacen.Controllers.Inventario
{
    public class InventarioAxController : Controller
    {

        private InventarioAxService _inv;

        // GET: Almacen/Inventario
        [CustomAuthorize(Roles = "000003,000314")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
            {
                _inv = new InventarioAxService();

                SessionPersister.ActiveVista = menuVista;
                SessionPersister.ActiveMenu = menuArea;
                SessionPersister.Search = search;
                SessionPersister.Pagina = pagina.ToString();
                var model = _inv.obtenerTodos(pagina, search);
                ViewBag.search = search;

                return View(model);
            }

        [HttpGet]
        [CustomAuthorize(Roles = "000003,000315")]
        public ActionResult Registrar()
        {
           return View();
        }
            
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(InventarioAxModels model)
            {

                if(ModelState.IsValid)
                {
                    _inv = new InventarioAxService();
                    
                    model.usuCrea = SessionPersister.Username;
                    model.usufchCrea = DateTime.Now;

                    if(_inv.crear(model))
                    {
                        TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                    }
                    else
                    {
                        TempData["mensaje"] = "< div id = 'warning' class='alert alert-warning'>Error al crear el registro.</div>";
                    }
                   
                    return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
                }
                return View();
            }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000316")]
        public ActionResult Modificar(string idPro,string nrolot,string ubiPro)
            {
                _inv = new InventarioAxService();
                var model = _inv.obtenerModel(idPro, nrolot, ubiPro);
                return View(model);
            }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(InventarioAxModels model)
            {


                if(ModelState.IsValid)
                {
                    model.usuMod = SessionPersister.Username;
                    model.usufchMod = DateTime.Now;

                _inv = new InventarioAxService();
                if(_inv.modificar(model))
                {
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se modificó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "< div id = 'warning' class='alert alert-warning'>Error al modificar el registro.</div>";
                }


                    return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
                }

                 return View();
            }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000317")]
        public ActionResult Eliminar(string idPro, string nrolot, string ubiPro)
            {
                 _inv = new InventarioAxService();
                 var model = _inv.obtenerModel(idPro, nrolot, ubiPro);
                 return View(model);
            }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Eliminar(InventarioAxModels model)
        {
            _inv = new InventarioAxService();
            if(_inv.eliminar(model))
            {
                TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se eliminó el registro.</div>";
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'> Error al eliminar el registro.</div>";
            }

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        [CustomAuthorize(Roles = "000003,000346")]
        public FileResult PlantillaInvAx()
        {
            return File(Server.MapPath("~/Plantillas/PLANTILLA CARGA INVENTARIO AX.xlsx"), "application/vnd.ms-excel");
        }

        [CustomAuthorizeJson(Roles = "000003,000347")]
        public JsonResult importData()
        {
            FileStream fs = new FileStream(Server.MapPath("~/Import/InventarioAx/tempCarga.xls"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            SLDocument sl = new SLDocument(fs, "PRODUCTO");
            int rowIndex = 2;

            string codigo = "";
            string nroLote = "";
            string ubicacion = "";
            string grupo = "";
            string descripcion = "";
            DateTime fchVen = new DateTime();
            string cantidad = "";
            string cbarra = "";
            string fabricante = "";
            string almacen = "";

            string mensaje = "|";
            int c = 0;

            InventarioAxModels model = new InventarioAxModels();

            if(sl.SelectWorksheet("PRODUCTO"))
            { //valido que tenga la pestaña
                _inv = new InventarioAxService();
                _inv.eliminarRegistros();

                while(!string.IsNullOrEmpty(sl.GetCellValueAsString(rowIndex, 1)))
                {
                    //tomamos los valores de las celdas y lo pasamos a las respectivas columnas
                    codigo = sl.GetCellValueAsString(rowIndex, 1).Trim();
                    nroLote = sl.GetCellValueAsString(rowIndex, 2).Trim();
                    ubicacion = sl.GetCellValueAsString(rowIndex, 3).Trim();
                    grupo = sl.GetCellValueAsString(rowIndex, 4).Trim();
                    descripcion = sl.GetCellValueAsString(rowIndex, 5).Trim();
                    fchVen = sl.GetCellValueAsDateTime(rowIndex, 6);
                    cantidad = sl.GetCellValueAsString(rowIndex, 7).Trim();
                    cbarra = sl.GetCellValueAsString(rowIndex, 8).Trim();
                    fabricante = sl.GetCellValueAsString(rowIndex, 9).Trim();
                    almacen = sl.GetCellValueAsString(rowIndex, 10).Trim();

                    if(!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(nroLote) && !string.IsNullOrEmpty(ubicacion) &&
                        !string.IsNullOrEmpty(grupo) && !string.IsNullOrEmpty(descripcion) && !string.IsNullOrEmpty(sl.GetCellValueAsString(rowIndex, 6).Trim()) &&
                        !string.IsNullOrEmpty(cantidad) && !string.IsNullOrEmpty(cbarra) && !string.IsNullOrEmpty(fabricante) && !string.IsNullOrEmpty(almacen))
                    {//siempre y cuando se diferente a vacio o nulo

                        if(!int.TryParse(cantidad, out c))
                        {
                            mensaje += "La cantidad ingresada no es valido|";
                        }
                      

                        model.idProInv = codigo;
                        model.nroLotInv = nroLote;
                        model.ubiProInv = ubicacion;
                        model.idgruInv = grupo;
                        model.desProInv = descripcion;
                        model.fchVenInv = fchVen;
                        model.canProInv = c;
                        model.codBarInv = cbarra;
                        model.usuCrea = SessionPersister.Username;  
                        model.usufchCrea = DateTime.Today;
                        model.fabProInv = fabricante;
                        model.almProInv = almacen;

                        if(_inv.crear(model))
                        {
                            mensaje += "Se creo el registro|";
                        }
                        else
                        {
                            mensaje += "No se creo el registro|";
                        }
                       

                        sl.SetCellValue(rowIndex, 11, mensaje);
                        model = new InventarioAxModels();
                        mensaje = "|";
                    }
                    else
                    {
                        sl.SetCellValue(rowIndex, 11, "no puede existir campos vacios");
                    }
                    //incrementeamos una unidad al indice de la fila para continuar con el recorrido
                    rowIndex += 1;
                }

                sl.SaveAs(Server.MapPath("~/Import/InventarioAx/tempCarga.xls"));
                fs.Close();
            }
            return Json(Server.MapPath("~/Import/InventarioAx/tempCarga.xls"), JsonRequestBehavior.AllowGet);
       
        }

        [HttpPost]
        public JsonResult saveFile(HttpPostedFileBase file)
        {
            Boolean ok = false;
            if (file != null && file.ContentLength > 0)
            {//cargo el archivo al servidor
                //crear carpeta
                string path = "~/Import";
                bool exists = Directory.Exists(Server.MapPath(path));
                if (!exists) Directory.CreateDirectory(Server.MapPath(path));

                string subpath = path + "/" + "InventarioAx" + "/";
                bool existssub = Directory.Exists(Server.MapPath(subpath));
                if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));

                string absolutePath = subpath + "tempCarga.xls";
                file.SaveAs(Server.MapPath(absolutePath));
                ok = true;
            }
            return Json(ok, JsonRequestBehavior.AllowGet);
        }

        public FileResult Resultado(string ruta)
        {
            return File(ruta, "application/vnd.ms-excel");
        }
        
    }
}