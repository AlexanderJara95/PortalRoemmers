using DocumentFormat.OpenXml.Spreadsheet;
using MvcRazorToPdf;
using PortalRoemmers.Areas.Marketing.Models.FarmacoVigilancia;
using PortalRoemmers.Areas.Marketing.Services.FarmacoVigilancia;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using iTextSharp.text;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace PortalRoemmers.Areas.Marketing.Controllers.FarmacoVigilancia
{
    public class EventoAdversoController : Controller
    {//EVENTOADVERSOCONTROLLER  000384
        private GeneroRepositorio _gen;
        private EventoAdversoRepositorio _eve;
        public EventoAdversoController()
        {
            _gen = new GeneroRepositorio();
            _eve = new EventoAdversoRepositorio();
        }

        // GET: Marketing/FichaEventoAdverso
        [CustomAuthorize(Roles = "000003,000385")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();

            var model = _eve.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }


        [HttpGet]
        [CustomAuthorize(Roles = "000003,000386")]
        public ActionResult Registrar()
        {
            ViewBag.genero = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen");

            return View();
        }


        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(EventoAdversoModels model)
        {
            if (ModelState.IsValid)
            {
                EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];


                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                model.nomComEmp = emple.nomComEmp;

                if (_eve.crear(model))
                {
                    bool val=  _eve.CorreoNuevoRegistro();
                    string msj = "";
                    if (!val)
                    {
                        msj = " se genero un error al enviar correo";
                    }
                    else
                    {
                        msj = "Se envio el correo satisfactoriamente";
                    }
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro. - " + msj+"</div>";
                   
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>Error al crear el registro.</div>";
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.genero = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen", model.idGen);

            return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000387")]
        public ActionResult Modificar(string id)
        {

            var model = _eve.obtenerItem(id);
            ViewBag.genero = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen", model.idGen);
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(EventoAdversoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;


                if (_eve.modificar(model))
                {
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se modificó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>Error al modificar el registro.</div>";
                }

               
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.genero = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen", model.idGen);
            return View(model);
        }


        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000389")]
        public ActionResult Visualizar(string id)
        {

            var model = _eve.obtenerItem(id);
            
            return View(model);
        }


        [CustomAuthorize(Roles = "000003,000388")]
        public ActionResult ReporteEventoAdverso(string menuArea, string menuVista, string fchIni, string fchFin)
        {
          
            //Primero obtenemos el día actual
            DateTime date = DateTime.Now;

            //Asi obtenemos el primer dia del mes actual
            DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);

            //Y de la siguiente forma obtenemos el ultimo dia del mes
            //agregamos 1 mes al objeto anterior y restamos 1 día.
            DateTime oUltimoDiaDelMes = oPrimerDiaDelMes.AddMonths(1).AddDays(-1);


            if (String.IsNullOrEmpty(fchIni)|| String.IsNullOrEmpty(fchFin) )
            {
                fchIni = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                fchFin = oUltimoDiaDelMes.ToString("dd/MM/yyyy");
            }
           

            ViewBag.fchIni = fchIni; 
            ViewBag.fchFin = fchFin;

            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;

            var parsedDateIni = DateTime.Parse(fchIni);
            var parsedDateFin = DateTime.Parse(fchFin);

            var model = _eve.obtenerEventosAdversos().Where(x=>(x.usufchCrea>= parsedDateIni && x.usufchCrea <= parsedDateFin.AddHours(23).AddMinutes(59))).ToList();
        
            return View(model);
        }

        [HttpPost]
        public JsonResult autoDesPro(string prefix)
        {
            var auto = _eve.obtenerProdDes().Where(x => x.text.ToUpper().Contains(prefix.ToUpper())).Select(x => new { x.value, x.text });
            return Json(auto, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Roles = "000003,000388")]
        public FileResult ExportRepEvento(string fchIni, string fchFin)
        {
            string ruta = ExportEventoAdverso(fchIni, fchFin);
            return File(ruta, "application/vnd.ms-excel");
        }

        [HttpPost]
        public JsonResult actualizarVisualizar(string idEveAdv)
        {
            bool ver = false;

            if (_eve.actualizarVisualizar(idEveAdv))
            {
                ver = true;
            }

            return Json(ver, JsonRequestBehavior.AllowGet);
        }


        public string ExportEventoAdverso(string fchIni, string fchFin)
        {

            var parsedDateIni = DateTime.Parse(fchIni);
            var parsedDateFin = DateTime.Parse(fchFin);

            var evento = _eve.obtenerEventosAdversos().Where(x => (x.usufchCrea >= parsedDateIni && x.usufchCrea <= parsedDateFin)).ToList();
            string path = "~/Export/Evento";
            bool exists = Directory.Exists(Server.MapPath(path));
            if (!exists) Directory.CreateDirectory(Server.MapPath(path));
            using (SLDocument sl = new SLDocument())
            {
                path = path + "/EVENTOS ADVERSOS.xls";
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
                rst.AppendText("Reporte Eventos Adversos", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                style.Alignment.Vertical = VerticalAlignmentValues.Center;
                sl.SetCellStyle(fil, col, style);
                sl.MergeWorksheetCells(1, 1, 1, 18);
                sl.SetRowHeight(1, 1, 40);
                //-----------------------------------------
                fil = fil + 1;
                //paciente
                col = 1;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CÓDIGO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 10);


                col = 2;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("NOMBRES Y APELLIDOS O INICIALES (PACIENTE)", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);

                col = 3;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("EDAD (PACIENTE)", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 8);

                col = 4;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("SEXO (PACIENTE)", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);


                col = 5;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("TELÉFONO DE CONTACTO O EMAIL (PACIENTE)", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);

                //medicamento

                col = 6;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("NOMBRE DEL MEDICAMENTO PRINCIPAL", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 48);

                col = 7;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("LOTE", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                col = 8;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("FECHA DE VENCIMIENTO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                //Evento
                col = 9;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("DESCRIPCIÓN DEL EVENTO ADVERSO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 50);

                //Reportante
                col = 10;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("NOMBRES Y APELLIDOS O INICIALES (REPORTANTE)", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);

                col = 11;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("TELÉFONO DE CONTACTO O EMAIL (REPORTANTE)", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);

                //Medico

                col = 12;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("NOMBRES DEL MEDICO TRATANTE", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);

                col = 13;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("TELÉFONO DE CONTACTO O EMAIL (MEDICO)", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);

                col = 14;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("NOMBRE DE LA INSTITUCION DE SALUD", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);

                //USUARIO QUE CREO
                col = 15;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("USUARIO REGISTRO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);


                fil = fil + 1;

                foreach (var e in evento)
                {
                    //paciente

                    //CODIGO
                    col = 1;
                    sl.SetCellValue(fil, col, e.idEveAdv);

                    //NOMBRES Y APELLIDOS O INICIALES
                    col = 2;
                    sl.SetCellValue(fil, col, e.nomApePacEveAdv);

                    //EDAD paciente
                    col = 3;
                    sl.SetCellValue(fil, col, e.edaPacEveAdv.ToString());

                    //SEXO paciente
                    col = 4;
                    sl.SetCellValue(fil, col, e.gene==null?"SIN GENERO":e.gene.nomGen);

                    //TELÉFONO DE CONTACTO O EMAIL paciente
                    col = 5;
                    sl.SetCellValue(fil, col, e.nroPacEveAdv);

                    //Medicamento
                    //NOMBRE DEL MEDICAMENTO PRINCIPAL:
                    col = 6;
                    sl.SetCellValue(fil, col, e.desProEveAdv);

                    //LOTE
                    col = 7;
                    sl.SetCellValue(fil, col, e.nroLteEveAdv);

                    //FECHA DE VENCIMIENTO
                    col = 8;
                    sl.SetCellValue(fil, col, e.fchVenEveAdv.ToString());

                    //DESCRIPCIÓN DEL EVENTO ADVERSO
                    col = 9;
                    sl.SetCellValue(fil, col, e.desAdvEveAdv);

                    //Reportante
                    //NOMBRES Y APELLIDOS O INICIALES
                    col = 10;
                    sl.SetCellValue(fil, col, e.nomApeRepEveAdv);
                    //TELÉFONO DE CONTACTO O EMAIL
                    col = 11;
                    sl.SetCellValue(fil, col, e.nroRepEveAdv);

                    //Medico
                    //NOMBRES DEL MEDICO TRATANTE
                    col = 12;
                    sl.SetCellValue(fil, col, e.nomApeMedEveAdv);

                    //TELÉFONO DE CONTACTO O EMAIL
                    col = 13;
                    sl.SetCellValue(fil, col, e.nomApeMedEveAdv);

                    //NOMBRE DE LA INSTITUCION DE SALUD
                    col = 14;
                    sl.SetCellValue(fil, col, e.nomApeMedEveAdv);

                    //USUARIO CREO
                    col = 15;
                    sl.SetCellValue(fil, col, e.nomComEmp);


                    fil = fil + 1;
                }
                sl.SaveAs(Server.MapPath(path));

            }
            return Server.MapPath(path);
        }

    }
}