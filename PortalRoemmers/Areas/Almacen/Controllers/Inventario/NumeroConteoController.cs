using DocumentFormat.OpenXml.Spreadsheet;
using PortalRoemmers.Areas.Almacen.Models.Inventario;
using PortalRoemmers.Areas.Almacen.Services.Inventario;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static PortalRoemmers.Areas.Almacen.Services.Inventario.InventarioAxService;

namespace PortalRoemmers.Areas.Almacen.Controllers
{
    public class NumeroConteoController : Controller
    {
        private NumeroConteoService _conR;
        private InventarioProductoService _con;
        private InventarioAxService _inv;
        private EstadoRepositorio _est;

        // GET: Almacen/ConteoReporte
        [CustomAuthorize(Roles = "000003,000325")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            _conR = new NumeroConteoService();            

            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _conR.obtenerTodos(pagina, search);
            ViewBag.search = search;

            return View(model);
        }

        [HttpGet]
        [CustomAuthorize(Roles = "000003,000326")]
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(NumeroConteoModels model)
        {

            if(ModelState.IsValid)
            {                
                _con= new InventarioProductoService();
                _conR = new NumeroConteoService();

                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                if(_conR.crear(model))
                {

                    if (_con.ObtenerProductosOk(model.codCon-1).Count != 0)
                    {
                        var resultado = _con.ObtenerProductosOk(model.codCon - 1);
                        foreach (var producto in resultado)
                        {
                            producto.nroInvCon = model.codCon;
                            _con.crear(producto);
                        }
                    }
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
        [CustomAuthorize(Roles = "000003,000327")]
        public ActionResult Modificar(string codCon)
        {
            var q = int.Parse(codCon);
            _est = new EstadoRepositorio();
            _conR = new NumeroConteoService();
            var model = _conR.obtenerModel(q);
            ViewBag.estGlobal = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);

            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(NumeroConteoModels model)
        {

            if(ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;

                _conR = new NumeroConteoService();
                if(_conR.modificar(model))
                {              
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se modificó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "< div id = 'warning' class='alert alert-warning'>Error al modificar el registro.</div>";
                }


                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            _est = new EstadoRepositorio();
            ViewBag.estGlobal = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", model.idEst);
            return View();
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000328")]
        public ActionResult Eliminar(string codCon)
        {
            _conR = new NumeroConteoService();
            var q = int.Parse(codCon);
            var model = _conR.obtenerModel(q);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Eliminar(NumeroConteoModels model)
        {
            _conR = new NumeroConteoService();
            if(_conR.eliminar(model))
            {
                TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se eliminó el registro.</div>";
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'> Error al eliminar el registro.</div>";
            }

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        [HttpGet]
        [CustomAuthorize(Roles = "000003,000318")]
        public FileResult ReporteConteo()
        {
            string ruta = ExportConteoAx();
            return File(ruta, "application/vnd.ms-excel");
        }

        [HttpGet]
        [CustomAuthorize(Roles = "000003,000383")]
        public FileResult ReporteConteoDetalle()
        {
            string ruta = ExporteDetalleConteo();
            return File(ruta, "application/vnd.ms-excel");
        }


        [HttpGet]
        [CustomAuthorize(Roles = "000003,000331")]
        public ActionResult Activar(int con, string est)
        {
            _conR = new NumeroConteoService();

            _conR.sqlUpdateCont(con, est);

            return RedirectToAction("Index");
        }


        public string ExportConteoAx()
        {
            _con = new InventarioProductoService();
            _inv = new InventarioAxService();
            _conR = new NumeroConteoService();

            var conTitulo = _con.obtenerConteoAgrupadoAx();
            var conteos = _conR.obtenerConteos();

            string path = "~/Export/Almacen";
            bool exists = Directory.Exists(Server.MapPath(path));
            if (!exists) Directory.CreateDirectory(Server.MapPath(path));
            using (SLDocument sl = new SLDocument())
            {
                sl.RenameWorksheet("Sheet1", "REPORTE");
                path = path + "/Listado de Inventario.xls";
                //EXPORTANDO DATA
                int fil = 1;
                int col = 0;
                SLFont font;
                SLRstType rst;
                SLStyle style;

                col = 1;

                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Major, 20);
                font.Underline = UnderlineValues.Double;
                rst = new SLRstType();
                rst.AppendText("MEGA LABS LATAM SA - Inventario Productos General", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                style.Alignment.Vertical = VerticalAlignmentValues.Center;
                sl.SetCellStyle(fil, col, style);
                sl.MergeWorksheetCells(1, 1, 1, 18);
                sl.SetRowHeight(1, 1, 40);
                //-----------------------------------------
                fil = fil + 1;

                col = 1;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Código", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 14);

                col = 2;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Descripción", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 49);

                col = 3;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Lote No.", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 10);

                col = 4;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Grupo", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 8);


                col = 5;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Cantidad", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 11);

                col = 6;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Cant. Ubi. AX", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 11);

                foreach (var c in conteos)
                {
                    col = col + 1;
                    font = new SLFont();
                    font.Bold = true;//negrita
                    font.SetFont(FontSchemeValues.Minor, 14);
                    rst = new SLRstType();
                    rst.AppendText(c.desCon, font);
                    sl.SetCellValue(fil, col, rst.ToInlineString());
                    sl.SetColumnWidth(col, 18);

                    col = col + 1;
                    font = new SLFont();
                    font.Bold = true;//negrita
                    font.SetFont(FontSchemeValues.Minor, 14);
                    rst = new SLRstType();
                    rst.AppendText("Cant. Ubi. "+ c.codCon, font);
                    sl.SetCellValue(fil, col, rst.ToInlineString());
                    sl.SetColumnWidth(col, 18);
                }

                col = col + 1;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Ultimo Conteo", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 12);

                col = col + 1;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Resultado", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 12);

                col = col + 1;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Almacen", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 12);

                // --------------------------------------
                //Rayas
                SLStyle style2 = sl.CreateStyle();
                style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.BottomBorder.Color = System.Drawing.Color.Black;
                style2.Border.TopBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.TopBorder.Color = System.Drawing.Color.Black;
                sl.SetCellStyle(fil, 1, fil, col, style2);
                // --------------------------------------

                fil = fil + 1;
                foreach (var c in conTitulo)
                {
                    //Código
                    col = 1;
                    sl.SetCellValue(fil, col, c.idProInv);
                    //Descripción
                    col = 2;
                    sl.SetCellValue(fil, col, c.desProInv);

                    //Lote No.
                    col = 3;
                    sl.SetCellValue(fil, col, c.nroLotInv);
                    
                    //grupo
                    col = 4;
                    sl.SetCellValue(fil, col, c.idgruInv);

                    //cantidad
                    col = 5;
                    sl.SetCellValue(fil, col, c.canProInv);

                    //cantidad ubicacion AX

                    col = 6;
                    sl.SetCellValue(fil, col, c.ubiProInv);

                    int ultimo = 0;
                    foreach (var co in conteos)
                    {
                        //Conteos
                        var m = _con.obtenerConteoEspecifico(c.idProInv, c.nroLotInv, co.codCon, c.almProInv);
                        int con = m == null ? 0 : m.Select(x => x.canInvCon).Sum();
                        col = col + 1;
                        sl.SetCellValue(fil, col, con);


                        col = col + 1;
                        sl.SetCellValue(fil, col, m.Count());

                        if (m.Count() != 0)
                        {
                            ultimo = con;
                        }
                    }

                    //ultimo conteo
                    col = col + 1;
                    sl.SetCellValue(fil, col, ultimo);
                    //diferencia
                    int resultado = ultimo -c.canProInv ;
                    col = col + 1;
                    sl.SetCellValue(fil, col, resultado);
                 

                    //Almacen
                    col = col + 1;
                    sl.SetCellValue(fil, col, c.almProInv);

                    fil = fil + 1;
                }
                sl.SaveAs(Server.MapPath(path));
            }

                return Server.MapPath(path);
        }

        public string ExporteDetalleConteo()
        {
            string path = "~/Export/Almacen";
            bool exists = Directory.Exists(Server.MapPath(path));
            if (!exists) Directory.CreateDirectory(Server.MapPath(path));

            _con = new InventarioProductoService();
            _inv = new InventarioAxService();

            var conTitulo = _con.obtenerConteoDetalle();
            var conAgrupadoAx = _con.obtenerConteoAgrupadoAx();
            using (SLDocument sl = new SLDocument())
            {
                sl.RenameWorksheet("Sheet1", "REPORTE");

                path = path + "/Listado de Inventario Detalle.xls";
                //EXPORTANDO DATA
                int fil = 1;
                int col = 0;
                SLFont font;
                SLRstType rst;
                SLStyle style;

                col = 1;

                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Major, 20);
                font.Underline = UnderlineValues.Double;
                rst = new SLRstType();
                rst.AppendText("MEGA LABS LATAM SA - Inventario Productos Detalle", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                style.Alignment.Vertical = VerticalAlignmentValues.Center;
                sl.SetCellStyle(fil, col, style);
                sl.MergeWorksheetCells(1, 1, 1, 18);
                sl.SetRowHeight(1, 1, 40);
                //-----------------------------------------
                fil = fil + 1;

                col = 1;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Código", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 14);

                col = 2;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Descripción", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 49);

                col = 3;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Lote No.", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);


                col = 4;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Almacen", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 12);

                col = 5;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Ubicación", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 8);

                col = 6;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Usuario", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                col = 7;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Nro. Conteo", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 12);

                col = 8;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Can. Conteo", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 12);

                col = 9; // Asegúrate de que esta es la siguiente columna disponible
                font = new SLFont();
                font.Bold = true; //negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Suma Can. Conteo", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 20);

                col = 10;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Can. Ax", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                col = 11;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Suma Can. Ax", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 20);

                col = 12;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Resultado", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);


                // --------------------------------------
                //Rayas
                SLStyle style2 = sl.CreateStyle();
                style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.BottomBorder.Color = System.Drawing.Color.Black;
                style2.Border.TopBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.TopBorder.Color = System.Drawing.Color.Black;
                sl.SetCellStyle(fil, 1, fil, col, style2);
                // --------------------------------------

                fil = fil + 1;

                // Escribiendo datos y combinando celdas
                int inicioFilaSuma = 0;
                string loteActual = string.Empty;
                int nroConteo = 0;

                var sumaPorLote = new Dictionary<string, int>();

                foreach (var c in conTitulo)
                {
                    if (!sumaPorLote.ContainsKey(c.nroLotCon + "|" + c.nroInvCon))
                    {
                        sumaPorLote[c.nroLotCon + "|" + c.nroInvCon] = 0;
                    }
                    sumaPorLote[c.nroLotCon + "|" + c.nroInvCon] += c.canInvCon;

                    //Código
                    col = 1;
                    sl.SetCellValue(fil, col, c.codProCon);

                    //Descripción
                    col = 2;
                    sl.SetCellValue(fil, col, c.desProCon);

                    //Lote No.
                    col = 3;
                    sl.SetCellValue(fil, col, c.nroLotCon);

                    //Almacen.
                    col = 4;
                    sl.SetCellValue(fil, col, c.almInvCon);

                    //Ubicacion.
                    col = 5;
                    sl.SetCellValue(fil, col, c.ubiProCon);

                    //usuario.
                    col = 6;
                    sl.SetCellValue(fil, col, c.usuCrea);

                    //Nro Conteo.
                    col = 7;
                    sl.SetCellValue(fil, col, c.nroInvCon);

                    //cantidad Conteo.
                    col = 8;
                    sl.SetCellValue(fil, col, c.canInvCon);

                    col = 10;
                    var invAxFil = _inv.obtenerModel(c.codProCon, c.nroLotCon, c.ubiProCon);
                    sl.SetCellValue(fil, col, invAxFil != null ? invAxFil.canProInv : 0 );

                    string identificadorCompuesto = c.nroLotCon + "|" + c.nroInvCon;

                    if ((loteActual+"|"+nroConteo) != identificadorCompuesto)
                    {
                        if (inicioFilaSuma != 0)
                        {
                            // Combina las celdas del lote anterior
                            sl.MergeWorksheetCells(inicioFilaSuma, 9, fil - 1, 9);
                            sl.SetCellValue(inicioFilaSuma, 9, sumaPorLote[loteActual + "|" + nroConteo]);

                            // Combina las celdas del lote anterior
                            var canTotalProvinv = conAgrupadoAx.Find(lote => lote.nroLotInv == loteActual).canProInv;
                            sl.MergeWorksheetCells(inicioFilaSuma, 11, fil - 1, 11);
                            sl.SetCellValue(inicioFilaSuma, 11, canTotalProvinv);

                            //Resultado.
                            col = 11;
                            sl.MergeWorksheetCells(inicioFilaSuma, 12, fil - 1, 12);
                            sl.SetCellValue(inicioFilaSuma, 12, sumaPorLote[loteActual + "|" + nroConteo] - canTotalProvinv);
                        }

                        inicioFilaSuma = fil;
                        loteActual = c.nroLotCon;
                        nroConteo = c.nroInvCon;

                    }

                    fil = fil + 1;
                }
                if (inicioFilaSuma != 0 && !string.IsNullOrEmpty(loteActual + "|" + nroConteo))
                {
                    // Combina y establece valor para la columna 9 (Suma Can. Conteo)
                    sl.MergeWorksheetCells(inicioFilaSuma, 9, fil - 1, 9);
                    sl.SetCellValue(inicioFilaSuma, 9, sumaPorLote.ContainsKey(loteActual + "|" + nroConteo) ? sumaPorLote[loteActual + "|" + nroConteo] : 0);

                    // Combina y establece valor para la columna 10 (Tot Cantidad)
                    var canTotal = conAgrupadoAx.Find(lote => lote.nroLotInv == loteActual);
                    sl.MergeWorksheetCells(inicioFilaSuma, 11, fil - 1, 11);
                    sl.SetCellValue(inicioFilaSuma, 11, canTotal != null ? canTotal.canProInv : 0);

                    //Resultado.
                    col = 12;
                    sl.MergeWorksheetCells(inicioFilaSuma, 12, fil - 1, 12);
                    sl.SetCellValue(inicioFilaSuma, 12, (sumaPorLote.ContainsKey(loteActual + "|" + nroConteo) && canTotal != null) ? (sumaPorLote[loteActual + "|" + nroConteo] - canTotal.canProInv):0);
                }

                sl.SaveAs(Server.MapPath(path));
            }
                return Server.MapPath(path);
        }


    }
}