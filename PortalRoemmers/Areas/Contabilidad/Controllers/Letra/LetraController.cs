using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Contabilidad.Services.Letra;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Contabilidad.Models.Letra;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using MvcRazorToPdf;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.Contabilidad.Services.Letra;

namespace PortalRoemmers.Areas.Contabilidad.Controllers.Letra
{
    public class LetraController : Controller
    {//LETRACONTROLLER 000018
        private LetraService _let;
        private AceptanteService _acept;
        private EstadoRepositorio _est;
        private MonedaRepositorio _mon;
        private UsuarioRepositorio _usu;
        private FirLetService _fir;
        public LetraController()
        {
            _let = new LetraService();
            _est = new EstadoRepositorio();
            _acept = new AceptanteService();
            _mon = new MonedaRepositorio();
            _usu = new UsuarioRepositorio();
            _fir = new FirLetService();
        }
        //LETRA_LISTAR 
        [CustomAuthorize(Roles = "000003,000359")]
        public ActionResult Index(string menuArea, string menuVista)
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            var model = _let.obtenerLetras();
            ViewBag.Estados = new SelectList(_est.obteneEstadoLetra(), "idEst", "nomEst");
            return View(model);
        }
        //LETRA_REGISTRAR
        [CustomAuthorize(Roles = "000003,000360")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.aceptantes = new SelectList(_acept.obtenerAceptantes(), "idAcep", "nomAceptante");
            ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "nomMon");
            return View();
        }
        
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(LetraModels model)
        {
            //model.idEst = ConstantesGlobales.estadoActivo;
            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                TempData["mensaje"] = _let.crear(model);
                //Creo la firma
                FirmasLetraModels fs = new FirmasLetraModels();
                fs.idLetra = model.idLetra;
                fs.idAcc = SessionPersister.UserId;
                fs.idEst = model.idEst;
                fs.obsFirLet = "Solicitud Creada";
                fs.usuCrea = SessionPersister.Username;
                fs.usufchCrea = DateTime.Now;
                _fir.mergeFirmas(fs);
                //--------------
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.aceptantes = new SelectList(_acept.obtenerAceptantes(), "idAcep", "nomAceptante");
            ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "nomMon", model.idMon);
            return View(model);
        }
        //LETRA_MODIFICAR
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000361")]
        public ActionResult Modificar(string id)
        {
            var model = _let.obtenerItem(id);
            ViewBag.aceptantes = new SelectList(_acept.obtenerAceptantes(), "idAcep", "nomAceptante",model.idAcep);
            ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "nomMon", model.idMon);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(LetraModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;
                TempData["mensaje"] = _let.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.aceptantes = new SelectList(_acept.obtenerAceptantes(), "idAcep", "nomAceptante",model.idAcep);
            ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "nomMon", model.idMon);
            return View(model);
        }
        //LETRA_APROBAR
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000362")]
        public ActionResult Aprobar(string menuArea, string menuVista)
        {
            string id = SessionPersister.UserId;
            var model = _let.obtenerLetrasRegistradas();
            //---
            Parametros p = new Parametros();
            var parametro = p.selectResultado(ConstantesGlobales.Com_Apr_Let).ToList();
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp }).ToList();
            //--
            ViewBag.Aprobador = new SelectList(result.Select(x => new { idAccApro = x.value, nombre = x.nomComEmp }), "idAccApro", "nombre");
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }
        //LETRA_CAMBIAR_ESTADO
        public JsonResult modificarEstado(string let, string idNewEst, string idOldEst, string obs)
        {
            //valores iniciales
            Boolean correcto = false;
            //---
            //Obtener registros
            switch (idNewEst)
            {
                //ESTADO CANCELADA
                case "22":
                    if (idOldEst == ConstantesGlobales.estadoAceptada)
                    {
                        _let.updateEstadoLetra(let, idNewEst);
                        //Creo la firma
                        FirmasLetraModels fs = new FirmasLetraModels();
                        fs.idLetra = let;
                        fs.idAcc = SessionPersister.UserId;
                        fs.idEst = idNewEst;
                        fs.obsFirLet = obs;
                        fs.usuCrea = SessionPersister.Username;
                        fs.usufchCrea = DateTime.Now;
                        _fir.mergeFirmas(fs);
                        //--------------
                        correcto = true;
                    }
                    else
                    {
                        correcto = false;
                    }
                    break;
                //ESTADO ANULADO
                case "14":
                    if (idOldEst == ConstantesGlobales.estadoRegistrado || idOldEst == ConstantesGlobales.estadoAprobado || idOldEst == ConstantesGlobales.estadoAceptada)
                    {
                        _let.updateEstadoLetra(let, idNewEst);
                        //Creo la firma
                        FirmasLetraModels fs = new FirmasLetraModels();
                        fs.idLetra = let;
                        fs.idAcc = SessionPersister.UserId;
                        fs.idEst = idNewEst;
                        fs.obsFirLet = obs;
                        fs.usuCrea = SessionPersister.Username;
                        fs.usufchCrea = DateTime.Now;
                        _fir.mergeFirmas(fs);
                        //--------------
                        correcto = true;
                    }
                    else
                    {
                        correcto = false;
                    }
                    break;
                //ESTADO ACEPTADA
                case "21":
                    if (idOldEst == ConstantesGlobales.estadoAprobado)
                    {
                        _let.updateEstadoLetra(let, idNewEst);
                        //Creo la firma
                        FirmasLetraModels fs = new FirmasLetraModels();
                        fs.idLetra = let;
                        fs.idAcc = SessionPersister.UserId;
                        fs.idEst = idNewEst;
                        fs.obsFirLet = obs;
                        fs.usuCrea = SessionPersister.Username;
                        fs.usufchCrea = DateTime.Now;
                        _fir.mergeFirmas(fs);
                        //--------------
                        correcto = true;
                    }
                    else
                    {
                        correcto = false;
                    }
                    break;       
            }
            return Json(correcto, JsonRequestBehavior.AllowGet);
        }
        //APROBAR JSON
        [HttpPost]
        public JsonResult aprobar_o_no_Letras(string let, string apr, string glo, string est)
        {
            //valores iniciales
            Boolean correcto = true;
            Boolean act = false;//actualizacion incorrecta
            Boolean firma = false;//firma incorrecta
            //convierto las solicitudes en un arreglo
            string[] codigo = let.Split('|');

            foreach (var c in codigo)
            {
                act = _let.updateEstadoLetra(c, est);

                if (act)
                {
                    FirmasLetraModels f = new FirmasLetraModels();
                    f.idLetra = c;
                    f.idAcc = apr;
                    f.idEst = est;
                    f.usuCrea = SessionPersister.Username;
                    f.usufchCrea = DateTime.Now;
                    f.obsFirLet = glo;
                    firma = _fir.mergeFirmas(f);
                    if (!firma) { break; }//si firma esta incorrecta salgo del for
                    f = new FirmasLetraModels();
                }
                else { break; }
            }
            if (!act || !firma)//ambos deben ser corectos
            {
                correcto = false;
            }
            return Json(correcto, JsonRequestBehavior.AllowGet);
        }
        //IMPRIMIR LETRA - PENDIENTE DE AGREGAR FIRMA DIGITAL
        [CustomAuthorize(Roles = "000003,000363")]
        [HttpGet]
        public ActionResult Imprimir(string idLet,Boolean fir)
        {
            var letra = _let.obtenerItemEspecifico(idLet);
            //ViewBag.pdf = html;
            //Leemos la plantilla
            string plantilla = Server.MapPath("~/Plantillas/PlantillaLetra.pdf");
            string nueva_letra = Server.MapPath("~/Plantillas/NuevaLetra1.pdf");
            string firma = Server.MapPath("~/Plantillas/FirLet2.png");
            //Leemos la plantilla y la copiamos
            PdfReader reader = new PdfReader(plantilla);
            //**
            var s = reader.GetPageSize(1);
            int xSize = (int)s.Width;//595
            int ySize = (int)s.Height;//325
            //**
            PdfStamper stamper = new PdfStamper(reader, new FileStream(nueva_letra,FileMode.Create));
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);

            //***
            PdfContentByte over = stamper.GetOverContent(1);
            //***
            if(fir)
            { 
                Image img = Image.GetInstance(new FileStream(firma, FileMode.Open,FileAccess.Read,FileShare.Read));
                img.SetAbsolutePosition(xSize - 225, ySize - 262);
                img.ScalePercent(28);
                over.AddImage(img);
            }
            //***
            over.BeginText();
            over.SetFontAndSize(bf, 8);
            over.ShowTextAligned(0, letra.codLetra, xSize - 490, ySize - 85, 0);
            over.EndText();
            //***
            over.BeginText();
            over.SetFontAndSize(bf, 7);
            over.ShowTextAligned(0, letra.refLetra, xSize - 445, ySize - 85, 0);
            over.EndText();
            //***
            over.BeginText();
            over.SetFontAndSize(bf, 8);
            over.ShowTextAligned(0, letra.fchGiroLet.ToString("dd/MM/yyyy"), xSize - 320, ySize - 85, 0);
            over.EndText();
            //***
            over.BeginText();
            over.SetFontAndSize(bf, 8);
            over.ShowTextAligned(0, letra.lugGiroLet, xSize - 240, ySize - 85, 0);
            over.EndText();
            //***
            over.BeginText();
            over.SetFontAndSize(bf, 8);
            over.ShowTextAligned(0, letra.fchVencLet.ToString("dd/MM/yyyy"), xSize - 150, ySize - 85, 0);
            over.EndText();
            //***
            over.BeginText();
            over.SetFontAndSize(bf, 8);
            over.ShowTextAligned(0, letra.moneda.simbMon+" "+letra.impLetra.ToString(".00"), xSize - 65, ySize - 85, 0);
            over.EndText();
            //***
            over.BeginText();
            over.SetFontAndSize(bf, 8);
            over.ShowTextAligned(0, letra.cantEnLetras, xSize - 480, ySize - 133, 0);
            over.EndText();
            //***
            over.BeginText();
            over.SetFontAndSize(bf, 8);
            over.ShowTextAligned(0,letra.aceptante.nomAceptante, xSize - 455, ySize - 165, 0);
            over.EndText();
            //***
            //Si el domicilio no es mayor que 50 procede
            var longitud = letra.aceptante.domAceptante.Length;
            if(longitud<51)
            { 
                over.BeginText();
                over.SetFontAndSize(bf, 7);
                over.ShowTextAligned(0, letra.aceptante.domAceptante, xSize - 455, ySize - 188, 0);
                over.EndText();
            }
            else
            {
                string dom1 = letra.aceptante.domAceptante.Substring(1,50);
                string dom2 = letra.aceptante.domAceptante.Substring(50,longitud-50);
                //****
                over.BeginText();
                over.SetFontAndSize(bf, 7);
                over.ShowTextAligned(0, dom1, xSize - 455, ySize - 188, 0);
                over.EndText();
                //--
                over.BeginText();
                over.SetFontAndSize(bf, 7);
                over.ShowTextAligned(0, dom2, xSize - 490, ySize - 200, 0);
                over.EndText();
                //****
            }
            //***
            over.BeginText();
            over.SetFontAndSize(bf, 8);
            over.ShowTextAligned(0, letra.aceptante.locAceptante, xSize - 324, ySize - 199, 0);
            over.EndText();
            //***
            over.BeginText();
            over.SetFontAndSize(bf, 8);
            over.ShowTextAligned(0, letra.aceptante.niffAceptante, xSize - 454, ySize - 212, 0);
            over.EndText();

            //***
            //Se cierra la conexion del copiadosq
            stamper.Close();
            //--------------------------------------------
            return File(nueva_letra, "application/pdf", "Letra_N°_" + letra.codLetra + "-" + letra.estado.nomEst + ".pdf");     
        }
        //EXPORTAR REPORTE
        [CustomAuthorize(Roles = "000003,000364")]
        public FileResult ExporLetras()
        {
            string ruta = ExportarLetras();
            return File(ruta, "application/vnd.ms-excel");
        }
        public string ExportarLetras()
        {
            var letra = _let.obtenerLetras();
            string path = "~/Export/Letra";
            bool exists = Directory.Exists(Server.MapPath(path));
            if (!exists) Directory.CreateDirectory(Server.MapPath(path));

            using (SLDocument sl = new SLDocument())
            {

                path = path + "/REPORTE DE LETRAS.xls";
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
                rst.AppendText("Reporte Letras", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                style.Alignment.Vertical = VerticalAlignmentValues.Center;
                sl.SetCellStyle(fil, col, style);
                sl.MergeWorksheetCells(1, 1, 1, 8);
                sl.SetRowHeight(1, 1, 40);
                //-----------------------------------------
                fil = fil + 1;

                col = 1;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("ID", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 5);


                col = 2;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CODIGO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                col = 3;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("REFERENCIA", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 35);

                col = 4;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CLIENTE", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 45);

                col = 5;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("FECHA GIRO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                col = 6;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("FECHA VENC.", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                col = 7;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("IMPORTE", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                col = 8;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("ESTADO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);

                
                // --------------------------------------
                //Rayas
                SLStyle style2 = sl.CreateStyle();
                style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.BottomBorder.Color = System.Drawing.Color.Black;
                style2.Border.TopBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.TopBorder.Color = System.Drawing.Color.Black;
                sl.SetCellStyle(fil, 1, fil, 8, style2);
                // --------------------------------------

                fil = fil + 1;

                foreach (var l in letra)
                {
                    //2
                    col = 1;
                    sl.SetCellValue(fil, col, l.idLetra);

                    //2
                    col = 2;
                    sl.SetCellValue(fil, col, l.codLetra);

                    //3
                    col = 3;
                    sl.SetCellValue(fil, col, l.refLetra);

                    //4
                    col = 4;
                    sl.SetCellValue(fil, col, l.aceptante.nomAceptante);

                    //5
                    col = 5;
                    sl.SetCellValue(fil, col, l.fchGiroLet.ToString("dd/MM/yyyy"));

                    //6
                    col = 6;
                    sl.SetCellValue(fil, col, l.fchVencLet.ToString("dd/MM/yyyy"));

                    //7
                    col = 7;
                    sl.SetCellValue(fil, col, l.moneda.simbMon +" "+  l.impLetra);

                    //8
                    col = 8;
                    sl.SetCellValue(fil, col, l.estado.nomEst);


                    fil = fil + 1;
                }

                sl.SaveAs(Server.MapPath(path));
            }
            return Server.MapPath(path);
        }

    }
}