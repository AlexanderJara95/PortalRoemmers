using DocumentFormat.OpenXml.Spreadsheet;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Equipo;
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

namespace PortalRoemmers.Areas.Sistemas.Controllers.Usuario
{
    public class EmpleadoController : Controller
    {// EMPLEADOCONTROLLER  000133
        private UsuarioRepositorio _usu;
        private EquipoRepositorio _equi;
        private EstadoRepositorio _est;
        private TipDocIdeRepositorio _ide;
        private CargoRepositorio _car;
        private EstCivilRepositorio _civ;
        private AfpRepositorio _afp;
        private GeneroRepositorio _gen;
        private AreaRoeRepositorio _are;
        private AsigAproRepositorio _asiapr;
        private UbicacionRepositorio _ubic;
        private EmpleadoRepositorio _emp;
        private BancoRepositorio _ban;
        public EmpleadoController()
        {
            _equi = new EquipoRepositorio();
            _usu = new UsuarioRepositorio();
            _est = new EstadoRepositorio();
            _ide = new TipDocIdeRepositorio();
            _car = new CargoRepositorio();
            _civ = new EstCivilRepositorio();
            _afp = new AfpRepositorio();
            _gen = new GeneroRepositorio();
            _are = new AreaRoeRepositorio();
            _asiapr = new AsigAproRepositorio();
            _ubic = new UbicacionRepositorio();
            _emp = new EmpleadoRepositorio();
            _ban = new BancoRepositorio();
        }


        [CustomAuthorize(Roles = "000003,000134")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _emp.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        //registrar
        [CustomAuthorize(Roles = "000003,000135")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.tipDoc = new SelectList(_ide.obtenerTipDocs(), "idTipDoc", "nomTipDoc");
            ViewBag.cargo = new SelectList(_car.obtenerCargo(), "idCarg", "desCarg");
            ViewBag.estado = new SelectList(_civ.obtenerEstCi(), "idEstCiv", "nomEstCiv");
            ViewBag.pais = new SelectList(_ubic.ubicacionPersonal().Select(x => new { x.cCod_Pais, x.cPais }).Distinct(), "cCod_Pais", "cPais");
            ViewBag.Afp = new SelectList(_afp.obteneAfpUsu(), "idAfp", "nomAfp");
            ViewBag.sexoUsu = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen");
            ViewBag.area = new SelectList(_are.obtenerArea(), "idAreRoe", "nomAreRoe");
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(EmpleadoModels model)
        {
            if (ModelState.IsValid)
            {
                model.nomComEmp = model.apePatEmp + " " + model.apeMatEmp + " " + model.nom1Emp + " " + model.nom2Emp;
                model.fchCreUsu = DateTime.Now;
                model.userCreUsu = SessionPersister.Username;
                TempData["mensaje"] = _emp.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.tipDoc = new SelectList(_ide.obtenerTipDocs(), "idTipDoc", "nomTipDoc");
            ViewBag.cargo = new SelectList(_car.obtenerCargo(), "idCarg", "desCarg");
            ViewBag.estado = new SelectList(_civ.obtenerEstCi(), "idEstCiv", "nomEstCiv");
            ViewBag.pais = new SelectList(_ubic.ubicacionPersonal().Select(x => new { x.cCod_Pais, x.cPais }).Distinct(), "cCod_Pais", "cPais");
            ViewBag.Afp = new SelectList(_afp.obteneAfpUsu(), "idAfp", "nomAfp");
            ViewBag.sexoUsu = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen");
            ViewBag.area = new SelectList(_are.obtenerArea(), "idAreRoe", "nomAreRoe");
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000136")]
        public ActionResult Modificar(string id)
        {

            var emp = _emp.obtenerItem(id.ToString());

            ViewBag.tipDoc = new SelectList(_ide.obtenerTipDocs(), "idTipDoc", "nomTipDoc", emp.idTipDoc);
            ViewBag.cargo = new SelectList(_car.obtenerCargo(), "idCarg", "desCarg", emp.idCarg);
            ViewBag.estado = new SelectList(_civ.obtenerEstCi(), "idEstCiv", "nomEstCiv", emp.idEstCiv);
            try
            {
                ViewBag.pais = new SelectList(_ubic.ubicacionPersonal().Select(x => new { x.cCod_Pais, x.cPais }).Distinct(), "cCod_Pais", "cPais", emp.cCod_Ubi.Substring(0, 2));
                ViewBag.departamento = new SelectList(_ubic.ubicacionPersonal().Where(x => x.cCod_Pais == emp.cCod_Ubi.Substring(0, 2)).Select(y => new { y.cCod_Dpto, y.cDepartamento }).Distinct().OrderBy(x => x.cDepartamento), "cCod_Dpto", "cDepartamento", emp.cCod_Ubi.Substring(2, 2));
                ViewBag.provincia = new SelectList(_ubic.ubicacionPersonal().Where(x => x.cCod_Dpto == emp.cCod_Ubi.Substring(2, 2) && x.cCod_Pais == emp.cCod_Ubi.Substring(0, 2)).Select(y => new { y.cCod_Provincia, y.cProvincia }).Distinct().OrderBy(x => x.cProvincia), "cCod_Provincia", "cProvincia", emp.cCod_Ubi.Substring(4, 2));
                ViewBag.ubicacion = new SelectList(_ubic.ubicacionPersonal().Where(x => x.cCod_Provincia == emp.cCod_Ubi.Substring(4, 2) && x.cCod_Pais == emp.cCod_Ubi.Substring(0, 2) && x.cCod_Dpto == emp.cCod_Ubi.Substring(2, 2)).Select(y => new { y.cCod_Ubi, y.cDistrito }).Distinct().OrderBy(x => x.cDistrito), "cCod_Ubi", "cDistrito", emp.cCod_Ubi.Substring(6, 2));
            }
            catch (Exception e)
            {
                ViewBag.pais = new SelectList(_ubic.ubicacionPersonal().Select(x => new { x.cCod_Pais, x.cPais }).Distinct(), "cCod_Pais", "cPais");
                ViewBag.departamento = new SelectList(_ubic.ubicacionPersonal().Select(y => new { y.cCod_Dpto, y.cDepartamento }).Distinct().OrderBy(x => x.cDepartamento), "cCod_Dpto", "cDepartamento");
                ViewBag.provincia = new SelectList(_ubic.ubicacionPersonal().Select(y => new { y.cCod_Provincia, y.cProvincia }).Distinct().OrderBy(x => x.cProvincia), "cCod_Provincia", "cProvincia");
                ViewBag.ubicacion = new SelectList(_ubic.ubicacionPersonal().Select(y => new { y.cCod_Ubi, y.cDistrito }).Distinct().OrderBy(x => x.cDistrito), "cCod_Ubi", "cDistrito");
            }
            ViewBag.area = new SelectList(_are.obtenerArea(), "idAreRoe", "nomAreRoe", emp.idAreRoe);
            ViewBag.Afp = new SelectList(_afp.obteneAfpUsu(), "idAfp", "nomAfp", emp.idAfp);
            ViewBag.sexoUsu = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen", emp.idGen);
            ViewBag.EstU = new SelectList(_est.obteneEstadoUsuario(), "idEst", "nomEst", emp.idEst);

            
            return View(emp);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(EmpleadoModels model)
        {
            if (ModelState.IsValid)
            {
                model.nomComEmp = model.apePatEmp + " " + model.apeMatEmp + " " + model.nom1Emp + " " + model.nom2Emp;
                model.fchModUsu = DateTime.Now;
                model.userModUsu = SessionPersister.Username;
                TempData["mensaje"] = _emp.modificar(model);
                
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000137")]
        public ActionResult Eliminar(string id)
        {
            var model = _emp.obtenerItem(id.ToString());
            
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(EmpleadoModels model)
        {
            string mensaje = "";

            if (_emp.cesadoEmp(model.idEmp))
            {
                
                if (_usu.cesadoUsu(model.idEmp))
                {
                    mensaje = "<div id='success' class='alert alert-success'>Se cambio estado a Cesado.</div>";
                }
                else
                {
                    mensaje = " < div id = 'warning' class='alert alert-warning'>Se produjo un error al Cesar el usuario</div>";
                }
            }
            else
            {
                mensaje = " < div id = 'warning' class='alert alert-warning'>Se produjo un error al Cesar el empleado</div>";
            }

            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
        //visualizar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000255")]
        public ActionResult Visualizar(string id)
        {
            var emp = _emp.obtenerItemDetalle(id.ToString());
            return View(emp);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000138")]
        public ActionResult AsignarJefe(string id, string nom)
        {
            var users = _emp.obtenerEmpleados();
            ViewBag.usuario = new SelectList(users.Where(x => (x.idEmpJ == null && x.idEmp != id) && x.idEmp != id).Select(x => new { x.idEmp, nombre = x.nomComEmp }), "idEmp", "nombre");
            ViewBag.usuarioA = new SelectList(users.Where(x => x.idEmpJ == id && x.idEmp != id.ToString()).Select(x => new { x.idEmp, nombre = x.nomComEmp }), "idEmp", "nombre");
            ViewBag.id = id;
            ViewBag.jefe = nom;
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult AddBoss(string[] idEmp, string id, string nombre)
        {
            string codigo = id;
            _emp.agregarJefe(idEmp, codigo);
            
            return RedirectToAction("AsignarJefe", new { id = codigo, nom = nombre });
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult DelBoss(string[] idEmpJ, string id, string nombre)
        {
            string codigo = id;
            _emp.eliminarJefe(idEmpJ);
            
            return RedirectToAction("AsignarJefe", new { id = codigo, nom = nombre });
        }

        [CustomAuthorize(Roles = "ALL")]
        public ActionResult Cumpleanio(int mes)
        {
            if (mes==13)
            {
                mes = 1;
            }
            if (mes==0)
            {
                mes = 12;
            }
            List<EmpleadoModels> model = _emp.obtenerEmpleados().OrderBy(x=>x.nacEmp.Day).Where(x=>x.nacEmp.Month== mes).ToList();
            ViewBag.mes = NombreMes(mes);
            SessionPersister.ActiveMenu = string.Empty;
            SessionPersister.ActiveVista = string.Empty;
            ViewBag.actualElegido = mes;
            return View(model);
        }

        //json
        public JsonResult SearchResponsable(string buscar)
        {
            var s = _emp.obtenerEmpleados().Where(x => x.nomComEmp.ToUpper().Contains(buscar.ToUpper())).Select(z => new { codigo = z.idEmp, nombre = z.nomComEmp, descripcion = z.nroDocEmp });
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Roles = "000003,000320")]
        public FileResult ExporEmpleados()
        {
            string ruta = ExporEmpleado();
            return File(ruta, "application/vnd.ms-excel");
        }

        public string NombreMes(int mes)
        {
            string nombre = "";
            switch (mes)
            {
                case 1:
                    nombre = "Enero";
                    break;
                case 2:
                    nombre = "Febrero";
                    break;
                case 3:
                    nombre = "Marzo";
                    break;
                case 4:
                    nombre = "Abril";
                    break;
                case 5:
                    nombre = "Mayo";
                    break;
                case 6:
                    nombre = "Junio";
                    break;
                case 7:
                    nombre = "Julio";
                    break;
                case 8:
                    nombre = "Agosto";
                    break;
                case 9:
                    nombre = "Setiembre";
                    break;
                case 10:
                    nombre = "Octubre";
                    break;
                case 11:
                    nombre = "Noviembre";
                    break;
                case 12:
                    nombre = "Diciembre";
                    break;
            }
            return nombre;
        }

        public string ExporEmpleado()
        {
            var empleado = _emp.obtenerEmpleadosExport();
            string path = "~/Export/Equipo";
            bool exists = Directory.Exists(Server.MapPath(path));
            if (!exists) Directory.CreateDirectory(Server.MapPath(path));
            using (SLDocument sl = new SLDocument())
            {
                path = path + "/INVENTARIO DE EMPLEADO.xls";
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
                rst.AppendText("Reporte Empleados", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                style.Alignment.Vertical = VerticalAlignmentValues.Center;
                sl.SetCellStyle(fil, col, style);
                sl.MergeWorksheetCells(1, 1, 1, 26);
                sl.SetRowHeight(1, 1, 40);
                //-----------------------------------------
                fil = fil + 1;


                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("COLABORADOR", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 43);

                col = 2;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("INGRESO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 12);

                col = 3;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("ZONA", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 37);

                col = 4;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CARGO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 35);

                col = 5;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("LINEA", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 20);

                col = 6;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("AREA", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 18);

                col = 7;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("TIPO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 20);

                col = 8;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("DNI", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 10);

                col = 9;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("NACIMIENTO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 18);

                col = 10;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("ESTADO CIVIL", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 22);

                col = 11;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("SEXO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                col = 12;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("AFP", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 13);

                col = 13;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("DIRECCION", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 50);

                col = 14;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("DISTRITO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 32);

                col = 15;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("DEPARTAMENTO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);

                col = 16;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("TELEFONO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                col = 17;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CELULAR", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 15);

                col = 18;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CORREO RMS", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 35);

                col = 19;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CORREO PERSONAL", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 40);

                col = 20;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CONTACTO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 35);

                col = 21;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CONTACTO CELULAR", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 12);

                col = 22;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CONTACTO CORREO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 35);

                col = 23;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("HIJOS", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 5);

                col = 24;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("ESTADO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 12);

                col = 25;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("JEFE", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 43);

                col = 26;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("CONFIRMO", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 5);

                // --------------------------------------
                //Rayas
                SLStyle style2 = sl.CreateStyle();
                style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.BottomBorder.Color = System.Drawing.Color.Black;
                style2.Border.TopBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.TopBorder.Color = System.Drawing.Color.Black;
                sl.SetCellStyle(fil, 1, fil, 26, style2);
                // --------------------------------------

                fil = fil + 1;

                foreach (var e in empleado)
                {
                    //COLABORADOR
                    col = 1;
                    sl.SetCellValue(fil, col, e.nomComEmp);

                    //INGRESO
                    col = 2;
                    sl.SetCellValue(fil, col, e.ingfchEmp == null?"": DateTime.Parse(e.ingfchEmp.ToString()).ToString("dd/MM/yyyy"));

                    //ZONA
                    col = 3;
                    sl.SetCellValue(fil, col, e.usuarios.Count == 0 ? "SIN USUARIO" : e.usuarios.Where(w => w.idEst!=ConstantesGlobales.estadoCesado).Select(x => x.UseZonLin.Count == 0 ? "SIN ZONA" : x.UseZonLin.Select(u => u.zona.nomZon).FirstOrDefault()).FirstOrDefault());

                    //CARGO
                    col = 4;
                    sl.SetCellValue(fil, col, e.cargo==null ? "SIN CARGO" : e.cargo.desCarg);

                    //LINEA
                    col = 5;
                    sl.SetCellValue(fil, col, e.usuarios.Count == 0 ? "SIN USUARIO" : e.usuarios.Where(w => w.idEst != ConstantesGlobales.estadoCesado).Select(x => x.UseZonLin.Count == 0 ? "SIN LINEA" : x.UseZonLin.Select(u => u.linea.nomLin).FirstOrDefault()).FirstOrDefault());

                    //AREA
                    col = 6;
                    sl.SetCellValue(fil, col, e.area==null? "SIN AREA": e.area.nomAreRoe);

                    //TIPO
                    col = 7;
                    //sl.SetCellValue(fil, col, e.tipTrab == null ? "SIN TIPO" : e.tipTrab.nomTipTra);
                    
                    //DNI
                    col = 8;
                    sl.SetCellValue(fil, col, e.nroDocEmp);

                    //NACIMIENTO
                    col = 9;
                    sl.SetCellValue(fil, col, e.nacEmp.ToString("dd/MM/yyyy"));

                    //ESTADO CIVIL
                    col = 10;
                    sl.SetCellValue(fil, col, e.estCiv == null ? "SIN ESTADO CIVIL" : e.estCiv.nomEstCiv);

                    //SEXO
                    col = 11;
                    sl.SetCellValue(fil, col, e.gene == null ? "SIN GENERO" : e.gene.nomGen);

                    //SEXO
                    col = 12;
                    sl.SetCellValue(fil, col, e.afp == null ? "SIN AFP" : e.afp.nomAfp);

                    //DIRECCION
                    col = 13;
                    sl.SetCellValue(fil, col, e.dirEmp);

                    //DISTRITO
                    col = 14;
                    sl.SetCellValue(fil, col, e.ubicacion == null ? "SIN DISTRITO" : e.ubicacion.cDistrito);

                    //DEPARTAMENTO 
                    col = 15;
                    sl.SetCellValue(fil, col, e.ubicacion == null ? "SIN DEPARTAMENTO " : e.ubicacion.cDepartamento);

                    //TELEFONO
                    col = 16;
                    sl.SetCellValue(fil, col, e.numTeleEmp);

                    //CELULAR RMS
                    col = 17;
                    sl.SetCellValue(fil, col, e.numCelEmp );

                    //CORREO RMS
                    col = 18;
                    sl.SetCellValue(fil, col, e.usuarios.Count == 0 ? "SIN USUARIO" : e.usuarios.Where(w => w.idEst != ConstantesGlobales.estadoCesado).Select(x => x.email).FirstOrDefault());

                    //CORREO PERSONAL
                    col = 19;
                    sl.SetCellValue(fil, col, e.emailPerEmp);

                    //PERSONA DE CONTACTO
                    col = 20;
                    sl.SetCellValue(fil, col, e.perConEmp);

                    //CELULAR CONTACTO
                    col = 21;
                    sl.SetCellValue(fil, col, e.celConEmp);

                    //CORREO CONTACTO
                    col = 22;
                    sl.SetCellValue(fil, col, e.corConEmp);

                    //NRO. DE HIJOS
                    col = 23;
                    //sl.SetCellValue(fil, col, e.nroHijEmp.ToString());

                    //ESTADO
                    col = 24;
                    sl.SetCellValue(fil, col, e.estado == null ? "SIN ESTADO" : e.estado.nomEst);

                    //JEFE
                    col = 25;
                    sl.SetCellValue(fil, col, e.jefe==null?"SIN JEFE": e.jefe.nomComEmp);

                    //CONFIRMO
                    col = 26;
                    sl.SetCellValue(fil, col, e.usuarios.Count == 0 ? "SIN USUARIO" : e.usuarios.Where(w => w.idEst != ConstantesGlobales.estadoCesado).Select(x => x.confirmEmail==true?"SI":"NO").FirstOrDefault());


                    fil = fil + 1;
                }
                sl.SaveAs(Server.MapPath(path));
            }

            return Server.MapPath(path);
        }
    }
}