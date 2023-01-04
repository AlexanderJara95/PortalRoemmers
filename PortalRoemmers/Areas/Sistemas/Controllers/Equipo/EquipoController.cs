using DocumentFormat.OpenXml.Spreadsheet;
using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Areas.Sistemas.Services.Equipo;
using PortalRoemmers.Areas.Sistemas.Services.Gasto;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Equipo
{
    public class EquipoController : Controller
    {//EQUIPOCONTROLLER 000018
        private EquipoRepositorio _equi;
        private UsuarioRepositorio _usu;
        private ModeloERepositorio _modE;
        private SistemaORepositorio _sisO;
        private TipoERepositorio _tipE;
        private FabricanteRepositorio _fab;
        private ProcesadorRepositorio _proc;
        private EstadoRepositorio _est;
        private AreaRoeRepositorio _area;
        private EmpleadoRepositorio _emp;
        private ByteRepositorio _byt;
        private TipoDiscoRepositorio _tipD;
        private TipoRamRepositorio _tipR;
        public EquipoController()
        {
            _equi = new EquipoRepositorio();
            _usu = new UsuarioRepositorio();
            _modE = new ModeloERepositorio();
            _sisO = new SistemaORepositorio();
            _tipE = new TipoERepositorio();
            _fab = new FabricanteRepositorio();
            _proc = new ProcesadorRepositorio();
            _est = new EstadoRepositorio();
            _area = new AreaRoeRepositorio();
            _emp = new EmpleadoRepositorio();
            _byt = new ByteRepositorio();
            _tipD = new TipoDiscoRepositorio();
            _tipR = new TipoRamRepositorio();
        }
        //EQUIPO_LISTAR 000019
        [CustomAuthorize(Roles = "000003,000019")]
        public ActionResult Index(string menuArea, string menuVista)
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            var model=_equi.obtenerEquipos();
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000020")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.fabricante = new SelectList(_fab.obtenerFabricantes(), "idFabrica", "nomFabri");
            ViewBag.procesador = new SelectList(_proc.obtenerProcesador(), "idProce", "nomProce");
            ViewBag.tipEqui = new SelectList(_tipE.obtenerTiposEquipo(), "idTipEqui", "nomTipEqui");
            ViewBag.sisOp = new SelectList(_sisO.obtenerSO(), "idSo", "nomSo");
            ViewBag.estEqui = new SelectList(_est.obtenerEstadoEquipo(), "idEst", "nomEst", ConstantesGlobales.estadoActivo);
            ViewBag.modelEqui = new SelectList(_modE.obtenerModelosEquipo().Where(x => x.idFabrica == ""), "idMolEq", "nomMolEq");
            ViewBag.area = new SelectList(_area.obtenerArea(), "idAreRoe", "nomAreRoe");
            ViewBag.tamdisco = new SelectList(_byt.obtenerBytes(), "idByt", "nomByt");
            ViewBag.tamMemoria = new SelectList(_byt.obtenerBytes(), "idByt", "nomByt");
            ViewBag.aquitectiura = new SelectList(ListadoArquitectura(), "Value", "Text");
            ViewBag.usuario = new SelectList(_emp.obtenerEmpleados().Where(x => x.idAreRoe == "").Select(x => new { x.idEmp, nombre = x.nomComEmp }), "idEmp", "nombre");
            ViewBag.tipDisco = new SelectList(_tipD.obtenerTipoDisco(), "idTipDis", "nomTipDis");
            ViewBag.tipRam = new SelectList(_tipR.obtenerTipoRam(), "idTipRam", "nomTipRam");
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(EquipoModels model)
        {
            if (ModelState.IsValid)
            {
                model.memEqui = model.memEqui + "-" + model.tamMen;
                model.discEqui = model.discEqui + "-" + model.tamDis;
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                TempData["mensaje"] = _equi.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

            ViewBag.fabricante = new SelectList(_fab.obtenerFabricantes(), "idFabrica", "nomFabri");
            ViewBag.procesador = new SelectList(_proc.obtenerProcesador(), "idProce", "nomProce");
            ViewBag.tipEqui = new SelectList(_tipE.obtenerTiposEquipo(), "idTipEqui", "nomTipEqui");
            ViewBag.sisOp = new SelectList(_sisO.obtenerSO(), "idSo", "nomSo");
            ViewBag.estEqui = new SelectList(_est.obtenerEstadoEquipo(), "idEst", "nomEst");
            ViewBag.modelEqui = new SelectList(_modE.obtenerModelosEquipo().Where(x => x.idFabrica == model.idFabrica), "idMolEq", "nomMolEq");
            ViewBag.area = new SelectList(_area.obtenerArea(), "idAreRoe", "nomAreRoe");
            ViewBag.tamdisco = new SelectList(_byt.obtenerBytes(), "idByt", "nomByt");
            ViewBag.tamMemoria = new SelectList(_byt.obtenerBytes(), "idByt", "nomByt");
            ViewBag.aquitectiura = new SelectList(ListadoArquitectura(), "Value", "Text");
            ViewBag.usuario = new SelectList(_emp.obtenerEmpleados().Where(x => x.idAreRoe == model.idAreRoe || x.idAreRoe == ConstantesGlobales.ninguno).Select(x => new { x.idEmp, nombre = x.nomComEmp }), "idEmp", "nombre");
            ViewBag.tipDisco = new SelectList(_tipD.obtenerTipoDisco(), "idTipDis", "nomTipDis", model.idTipDis);
            ViewBag.tipRam = new SelectList(_tipR.obtenerTipoRam(), "idTipRam", "nomTipRam", model.idTipRam);

            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000021")]
        public ActionResult Modificar(string id)
        {
            var model = _equi.obtenerItem(id);
            var desMen = model.memEqui.Split('-');
            var desDis = model.discEqui.Split('-');
            model.tamMen = desMen[1];
            model.memEqui = desMen[0];
            model.tamDis = desDis[1];
            model.discEqui = desDis[0];

            ViewBag.fabricante = new SelectList(_fab.obtenerFabricantes(), "idFabrica", "nomFabri", model.modelos.idFabrica);
            ViewBag.procesador = new SelectList(_proc.obtenerProcesador(), "idProce", "nomProce", model.idProce);
            ViewBag.tipEqui = new SelectList(_tipE.obtenerTiposEquipo(), "idTipEqui", "nomTipEqui", model.idTipEqui);
            ViewBag.sisOp = new SelectList(_sisO.obtenerSO(), "idSo", "nomSo", model.idSo);
            ViewBag.usuario = new SelectList(_emp.obtenerEmpleados().Where(x => x.idAreRoe == model.idAreRoe || x.idAreRoe == ConstantesGlobales.ninguno).Select(x => new { x.idEmp, nombre = x.nomComEmp }), "idEmp", "nombre", model.idEmp);
            ViewBag.estEqui = new SelectList(_est.obtenerEstadoEquipo(), "idEst", "nomEst", model.idEst);
            ViewBag.modelEqui = new SelectList(_modE.obtenerModelosEquipo().Where(x => x.idFabrica == model.modelos.idFabrica), "idMolEq", "nomMolEq", model.idMolEq);
            ViewBag.aquitectiura = new SelectList(ListadoArquitectura(), "Value", "Text");
            ViewBag.area = new SelectList(_area.obtenerArea(), "idAreRoe", "nomAreRoe", model.idAreRoe);
            ViewBag.tamdisco = new SelectList(_byt.obtenerBytes(), "idByt", "nomByt");
            ViewBag.tamMemoria = new SelectList(_byt.obtenerBytes(), "idByt", "nomByt");
            ViewBag.tipDisco= new SelectList(_tipD.obtenerTipoDisco(), "idTipDis", "nomTipDis", model.idTipDis);
            ViewBag.tipRam = new SelectList(_tipR.obtenerTipoRam(), "idTipRam", "nomTipRam", model.idTipRam);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(EquipoModels model)
        {
            if (ModelState.IsValid)
            {
                model.memEqui = model.memEqui + "-" + model.tamMen;
                model.discEqui = model.discEqui + "-" + model.tamDis;
                model.usuMod = SessionPersister.Username;
                model.usufchMod = DateTime.Now;
                TempData["mensaje"] = _equi.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

            ViewBag.fabricante = new SelectList(_fab.obtenerFabricantes(), "idFabrica", "nomFabri", model.idFabrica);
            ViewBag.procesador = new SelectList(_proc.obtenerProcesador(), "idProce", "nomProce", model.idProce);
            ViewBag.modelEqui = new SelectList(_modE.obtenerModelosEquipo().Where(x => x.idFabrica == model.idFabrica), "idMolEq", "nomMolEq", model.idMolEq);
            ViewBag.tipEqui = new SelectList(_tipE.obtenerTiposEquipo(), "idTipEqui", "nomTipEqui", model.idTipEqui);
            ViewBag.sisOp = new SelectList(_sisO.obtenerSO(), "idSo", "nomSo", model.idSo);
            ViewBag.usuario = new SelectList(_emp.obtenerEmpleados().Select(x => new { x.idEmp, nombre = x.nomComEmp }), "idEmp", "nombre", model.idEmp);
            ViewBag.estEqui = new SelectList(_est.obtenerEstadoEquipo(), "idEst", "nomEst", model.idEst);
            ViewBag.area = new SelectList(_area.obtenerArea(), "idAreRoe", "nomAreRoe", model.idAreRoe);
            ViewBag.estEqui = new SelectList(_est.obtenerEstadoEquipo(), "idEst", "nomEst", model.idEst);
            ViewBag.aquitectiura = new SelectList(ListadoArquitectura(), "Value", "Text");
            ViewBag.tamdisco = new SelectList(_byt.obtenerBytes(), "idByt", "nomByt");
            ViewBag.tamMemoria = new SelectList(_byt.obtenerBytes(), "idByt", "nomByt");
            ViewBag.tipDisco = new SelectList(_tipD.obtenerTipoDisco(), "idTipDis", "nomTipDis", model.idTipDis);
            ViewBag.tipRam = new SelectList(_tipR.obtenerTipoRam(), "idTipRam", "nomTipRam", model.idTipRam);
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000022")]
        public ActionResult Eliminar(string id)
        {
            var equi = _equi.obtenerItem(id);
            return View(equi);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(EquipoModels model)
        {
            TempData["mensaje"] = _equi.eliminar(model.idEquipo);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        [HttpPost]
        public JsonResult CreateProcesador(ProcesadorModels model)
        {
            string mensaje = "";

            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                if (_proc.crear(model))
                {
                    mensaje = "ok";
                }
                else
                {
                    mensaje = "error";
                }
            }
            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CreateModeloE(ModEquiModels model)
        {
            string mensaje = "";

            if (ModelState.IsValid)
            {
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                if (_modE.crear(model))
                {
                    mensaje = "ok";
                }
                else
                {
                    mensaje = "error";
                }
            }
            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InfoEquipo()
        {
            return Json(_equi.consultaEquipo(), JsonRequestBehavior.AllowGet);
        }

        public string ExportEquipo()
        {
            var equipo = _equi.obtenerEquiposDetallado();
            string path = "~/Export/Equipo";
            bool exists = Directory.Exists(Server.MapPath(path));
            if (!exists) Directory.CreateDirectory(Server.MapPath(path));

            using (SLDocument sl = new SLDocument())
            {
                
                path = path + "/INVENTARIO DE EQUIPOS.xls";
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
            rst.AppendText("Reporte Equipos", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            style = sl.CreateStyle();
            style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            style.Alignment.Vertical = VerticalAlignmentValues.Center;
            sl.SetCellStyle(fil, col, style);
            sl.MergeWorksheetCells(1, 1, 1, 27);
            sl.SetRowHeight(1, 1, 40);
            //-----------------------------------------
            fil = fil + 1;

            col = 1;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Fabricante", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 25);


            col = 2;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Modelo", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 35);

            col = 3;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Procesador", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 25);

            col = 4;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Velocidad", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 12);

            col = 5;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Principales", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 5);

            col = 6;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Lógicos", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 5);

            col = 7;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Memoria", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 10);

            col = 8;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Tipo", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 10);

            col = 9;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Slot", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 10);

            col = 10;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Disco", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 10);


            col = 11;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Tipo", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 10);

            col = 12;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Cantidad", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 10);

            col = 13;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Detalle", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 30);

            col = 14;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Tipo", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 10);

            col = 15;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Ubicación", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth( col, 15);

            col = 16;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Sistema Operativo", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth( col, 35);

            col = 17;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Versión", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 15);

            col = 18;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Arquitectura", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth( col, 10);

            col = 19;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Instalación", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 13);

            col = 20;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Equipo", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth(col, 15);

            col = 21;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Usuario", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth( col, 45);

            col = 22;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Estado", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth( col, 12);

            col = 23;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Nro Serie", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth( col, 15);

            col = 24;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Anydesk", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth( col, 15);

            col = 25;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Observación", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth( col, 30);

            col = 26;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Usuario Creador", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth( col, 12);

            col = 27;
            font = new SLFont();
            font.Bold = true;//negrita
            font.SetFont(FontSchemeValues.Minor, 14);
            rst = new SLRstType();
            rst.AppendText("Usuario Modificador", font);
            sl.SetCellValue(fil, col, rst.ToInlineString());
            sl.SetColumnWidth( col, 12);
            // --------------------------------------
             //Rayas
            SLStyle style2 = sl.CreateStyle();
            style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Medium;
            style2.Border.BottomBorder.Color = System.Drawing.Color.Black;
            style2.Border.TopBorder.BorderStyle = BorderStyleValues.Medium;
            style2.Border.TopBorder.Color = System.Drawing.Color.Black;
            sl.SetCellStyle(fil, 1, fil, 27, style2);
            // --------------------------------------

            fil = fil + 1;

            foreach (var e in equipo)
            {
                //Fabricante
                col = 1;
                sl.SetCellValue(fil, col, e.modelos.fabrica.nomFabri);

                //Modelo
                col = 2;
                sl.SetCellValue(fil, col, e.modelos.nomMolEq);

                //Procesador
                col = 3;
                sl.SetCellValue(fil, col, e.procesador.nomProce);

                //Velocidad CPU
                col = 4;
                sl.SetCellValue(fil, col, e.procesador.velCpuProce);

                //Pro. Principales
                col = 5;
                sl.SetCellValue(fil, col, e.procesador.nroNucProce);

                //Pro. Lógicos
                col = 6;
                sl.SetCellValue(fil, col, e.procesador.nroNucLogProce);

                //Memoria
                col = 7;
                sl.SetCellValue(fil, col, e.memEqui);

                col = 8;
                sl.SetCellValue(fil, col, e.tipoRam == null ? "VACIO" : e.tipoRam.nomTipRam);

                col = 9;
                sl.SetCellValue(fil, col, e.canRam);

                //Disco
                col = 10;
                sl.SetCellValue(fil, col, e.discEqui);

                col = 11;
                sl.SetCellValue(fil, col, e.tipoDisco == null ? "VACIO" : e.tipoDisco.nomTipDis);

                col = 12;
                sl.SetCellValue(fil, col, e.canDis);

                col = 13;
                sl.SetCellValue(fil, col, e.detDis);

                //Tipo
                col = 14;
                sl.SetCellValue(fil, col, e.tipEqui.nomTipEqui);

                //Ubicación
                col = 15;
                sl.SetCellValue(fil, col, e.area.nomAreRoe);

                //Sistema Operativo
                col = 16;
                sl.SetCellValue(fil, col, e.sisOp.nomSo);

                //Versión
                col = 17;
                sl.SetCellValue(fil, col, e.comEqui);

                //Versión
                col = 18;
                sl.SetCellValue(fil, col, e.arqSis);

                //Instalación
                col = 19;
                string fecha = "";
                try {
                    fecha = ((DateTime)e.fchInsEqui).ToString("dd-MM-yyyy");
                }
                catch (Exception ex) {
                    ex.Message.ToString();
                }

                sl.SetCellValue(fil, col, fecha);

                //Nombre
                col = 20;
                sl.SetCellValue(fil, col, e.nomPcEqui);
                //Usuario
                col = 21;
                sl.SetCellValue(fil, col, e.empleado == null ? "VACIO" : e.empleado.nomComEmp);
                //Estado
                col = 22;
                sl.SetCellValue(fil, col, e.estado.nomEst);

                //nro serie
                col = 23;
                sl.SetCellValue(fil, col, e.nroSerEqui);

                //Anydesk
                col = 24;
                sl.SetCellValue(fil, col, e.nroAnyEqui);

                //Observación
                col = 25;
                sl.SetCellValue(fil, col, e.detEqui);

                //usuario creador
                col = 26;
                sl.SetCellValue(fil, col, e.usuCrea +" - " + e.usufchCrea.ToString() );

                //usuario modifica
                col = 27;
                sl.SetCellValue(fil, col, e.usuMod + " - " + e.usufchMod.ToString());

                fil = fil + 1;
            }

            sl.SaveAs(Server.MapPath(path));
        }
            return Server.MapPath(path);
        }

        [CustomAuthorize(Roles = "000003,000319")]
        public FileResult ExporEquipos()
        {
            string ruta = ExportEquipo();
            return File(ruta, "application/vnd.ms-excel");
        }

        [HttpPost]
        public JsonResult searchEquipoDisponible()
        {
            var equipo = _equi.obtenerOnlyEquipos("REWK");
            string c = "";
            int cn = 0;
            int n = 1;
            string nombre = "";
            foreach (var e in equipo)
            {
                 c = e.nomPcEqui.Substring(4, 3);
                 cn = int.Parse(c);

                if (n!=cn)
                {
                    nombre="REWK"+ n.ToString("D3") + "LIM";
                    break;
                }
                n++;
            }
            if (nombre=="")
            {
                nombre = "REWK" + n.ToString("D3") + "LIM";
            }


            return Json(nombre, JsonRequestBehavior.AllowGet);
        }
        //combos
        [HttpPost]
        public JsonResult modelxFabri(string idFabrica)
        {
            return Json(_modE.obtenerModelosEquipo().Where(x=>x.idFabrica== idFabrica).Select(y=>new {y.idMolEq,y.nomMolEq }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult areaxPer(string idAreRoe)
        {
            return Json(_emp.obtenerEmpleados().Where(x=>x.idAreRoe== idAreRoe|| x.idAreRoe== ConstantesGlobales.ninguno).Select(x => new { x.idEmp, nombre = x.apePatEmp + " " + x.apeMatEmp + " " + x.nom1Emp + " " + x.nom2Emp }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult cargarProcesador()
        {
            return Json(_proc.obtenerProcesador(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult cargarModeloE(string idFabrica)
        {
            return Json(_modE.obtenerModelosEquipo().Where(x=>x.idFabrica== idFabrica), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult darBajaCPU(string msjBaja,string idEquipo)
        {
            string mensaje = "";

                if (_equi.sqlUpdateBaja(msjBaja, idEquipo))
                {
                    mensaje = "OK";
                }
                else
                {
                    mensaje = "ERROR AL ACTUALIZAR";
                }
       
            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }

        private SelectList ListadoArquitectura()
        {
            //creamos una lista tipo SelectListItem
            List<SelectListItem> lst = new List<SelectListItem>();

            //De la siguiente manera llenamos manualmente,
            //Siendo el campo Text lo que ve el usuario y
            //el campo Value lo que en realidad vale nuestro valor
            lst.Add(new SelectListItem() { Text = "32 BITS", Value = "32 BITS" });
            lst.Add(new SelectListItem() { Text = "64 BITS", Value = "64 BITS" });

            //Agregamos la lista a nuestro SelectList
            SelectList miSL = new SelectList(lst, "Value", "Text");

            return miSL;
        }

    }
}