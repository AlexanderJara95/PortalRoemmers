using DocumentFormat.OpenXml.Spreadsheet;
using PortalRoemmers.Areas.Almacen.Models.Inventario;
using PortalRoemmers.Areas.Almacen.Services.Inventario;
using PortalRoemmers.Security;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Almacen.Controllers.Inventario
{
    public class HistoriaInventarioController : Controller
    {

        private HistoriaInventarioService _hisInv;
        private InventarioProductoService _invPro;
        private InventarioAxService _invAx;

        [CustomAuthorize(Roles = "000003,000342")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            _hisInv = new HistoriaInventarioService();

            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();

            var model = _hisInv.obtenerTodos(pagina, search);
            ViewBag.search = search;

            return View(model);
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

                string subpath = path + "/" + "HistoriaInv" + "/";
                bool existssub = Directory.Exists(Server.MapPath(subpath));
                if (!existssub) Directory.CreateDirectory(Server.MapPath(subpath));

                string absolutePath = subpath + "tempHisInv.xls";
                file.SaveAs(Server.MapPath(absolutePath));
                ok = true;
            }
            return Json(ok, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000330")]
        public JsonResult finalizarInventario()
        {
            bool mensaje = false;
            _invPro = new InventarioProductoService();
            _invAx = new InventarioAxService();

            if (_invPro.eliminarRegistros() && _invAx.eliminarRegistros()) { mensaje = true; }
            

            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000329")]
        public JsonResult importData()
        {

            _hisInv = new HistoriaInventarioService();
            FileStream fs = new FileStream(Server.MapPath("~/Import/HistoriaInv/tempHisInv.xls"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            SLDocument sl = new SLDocument(fs, "REPORTE");
            int rowIndex = 2;

            string codigo = "";
            string descripcion = "";
            string nroLote = "";
            string grupo = "";
            string cantidad = "";
            string ultimo = "";
            string resultado = "";
            string observacion = "";

            string mensaje = "|";
            int c = 0;
            int u = 0;
            int r = 0;
            HistoriaInventarioModels model = new HistoriaInventarioModels();

            if (sl.SelectWorksheet("REPORTE"))
            { //valido que tenga la pestaña
                _hisInv = new HistoriaInventarioService();
                _hisInv.eliminarCmd();

                SLFont font;
                SLRstType rst;


                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Observación", font);
                sl.SetCellValue(1, 8, rst.ToInlineString());
                sl.SetColumnWidth(8, 15);


                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Mensaje", font);
                sl.SetCellValue(1, 9, rst.ToInlineString());
                sl.SetColumnWidth(9, 11);


                //Rayas
                SLStyle style2 = sl.CreateStyle();
                style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.BottomBorder.Color = System.Drawing.Color.Black;
                style2.Border.TopBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.TopBorder.Color = System.Drawing.Color.Black;
                sl.SetCellStyle(1, 1, 1, 9, style2);
                // --------------------------------------


                while (!string.IsNullOrEmpty(sl.GetCellValueAsString(rowIndex, 1)))
                {
                    //tomamos los valores de las celdas y lo pasamos a las respectivas columnas
                    codigo = sl.GetCellValueAsString(rowIndex, 1).Trim();
                    descripcion = sl.GetCellValueAsString(rowIndex, 2).Trim();
                    nroLote = sl.GetCellValueAsString(rowIndex, 3).Trim();
                    grupo = sl.GetCellValueAsString(rowIndex, 4).Trim();
                    cantidad = sl.GetCellValueAsString(rowIndex, 5).Trim();
                    ultimo = sl.GetCellValueAsString(rowIndex, 6).Trim();
                    resultado = sl.GetCellValueAsString(rowIndex, 7).Trim();
                    observacion = sl.GetCellValueAsString(rowIndex, 8).Trim();

                    if (!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(descripcion) && !string.IsNullOrEmpty(nroLote) &&
                        !string.IsNullOrEmpty(grupo) && !string.IsNullOrEmpty(cantidad) && !string.IsNullOrEmpty(ultimo) &&
                        !string.IsNullOrEmpty(resultado) && !string.IsNullOrEmpty(observacion))
                    {//siempre y cuando se diferente a vacio o nulo

                        if (!int.TryParse(cantidad, out c))
                        {
                            mensaje += "La cantidad ingresada no es valido|";
                        }
                        if (!int.TryParse(ultimo, out u))
                        {
                            mensaje += "El valor ultimo ingresado no es valido|";
                        }
                        if (!int.TryParse(resultado, out r))
                        {
                            mensaje += "El resultado ingresada no es valido|";
                        }


                        model.fchConHis = DateTime.Today;
                        model.codProConHis = codigo;
                        model.desProConHis = descripcion;
                        model.nroLotConHis = nroLote;
                        model.idGruConHis = grupo;
                        model.obsInvConHis = observacion;

                        model.canInvConHis = c;
                        model.ultCanConHis = u;
                        model.resCanConHis = r;


                        model.usuCrea = SessionPersister.Username;
                        model.usufchCrea = DateTime.Today;
                      

                        if (_hisInv.crear(model))
                        {
                            mensaje += "Se creo el registro|";
                        }
                        else
                        {
                            mensaje += "No se creo el registro|";
                        }


                        sl.SetCellValue(rowIndex, 9, mensaje);
                        model = new HistoriaInventarioModels();
                        mensaje = "|";
                    }
                    else
                    {
                        sl.SetCellValue(rowIndex, 9, "no puede existir campos vacios");
                    }
                    //incrementeamos una unidad al indice de la fila para continuar con el recorrido
                    rowIndex += 1;
                }

                sl.SaveAs(Server.MapPath("~/Import/HistoriaInv/tempHisInv.xls"));
                fs.Close();
            }
            return Json(Server.MapPath("~/Import/HistoriaInv/tempHisInv.xls"), JsonRequestBehavior.AllowGet);

        }

        public FileResult Resultado(string ruta)
        {
            return File(ruta, "application/vnd.ms-excel");
        }


        [CustomAuthorize(Roles = "000003,000348")]
        public FileResult PlantillaHistorial()
        {
            return File(Server.MapPath("~/Plantillas/PLANTILLA CARGA HISTORIAL.xlsx"), "application/vnd.ms-excel");
        }


    }
}