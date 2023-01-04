using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Marketing.Models.Actividad;
using PortalRoemmers.Areas.Marketing.Services.Actividad;
using PortalRoemmers.Areas.Sistemas.Services.Visitador;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Medico;
using PortalRoemmers.Helpers;
using System.Globalization;
using System.IO;
using SpreadsheetLight;
using PortalRoemmers.Areas.Sistemas.Models.Medico;

namespace PortalRoemmers.Areas.Marketing.Controllers.Actividad
{
    //AREA ROL 000229
    public class ActividadController : Controller
    {//ACTIVIDAD_CONTROLLER 000230
        Ennumerador enu ;
        private ActividadRepositorio _act;
        private EspecialidadRepositorio _esp;
        private EstadoRepositorio _est;
        private UsuarioRepositorio _usu;
        private Parametros p ;
        private Esp_Usu_Repositorio _esp_usu;
        private MedicoRepositorio _med;
        public ActividadController()
        {
            _usu = new UsuarioRepositorio();
            _act = new ActividadRepositorio();
            _esp = new EspecialidadRepositorio();
            _est = new EstadoRepositorio();
            p = new Parametros();
            _esp_usu = new Esp_Usu_Repositorio();
            enu = new Ennumerador();
            _med = new MedicoRepositorio();

        }
        //ACTIVIDAD_LISTAR 000231
        [CustomAuthorize(Roles = "000003,000231")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "", string fchEveSolGasI = "", string fchEveSolGasF = "")
        {
            //-----------------------------
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();
            try
            {
                inicio = DateTime.Parse(fchEveSolGasI);
                fin = DateTime.Parse(fchEveSolGasF);
            }
            catch (Exception e)
            {
                DateTime date = DateTime.Now;
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, 1, 1);
                DateTime oUltimoDiaDelMes = new DateTime(date.Year, 12, 31);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = oUltimoDiaDelMes.ToString("dd/MM/yyyy");
                inicio = DateTime.Parse(primero);
                fin = DateTime.Parse(actual);
            }
            //-----------------------------
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            SessionPersister.FchEveSolGasI = fchEveSolGasI;
            SessionPersister.FchEveSolGasF = fchEveSolGasF;
            //-----------------------------
            var model = _act.obtenerTodos(pagina, search, inicio.ToString(), fin.ToString());
            //-----------------------------
            ViewBag.search = search;
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //-----------------------------
            return View(model);
        }
        //ACTIVIDAD_REGISTRAR 000232
        [CustomAuthorize(Roles = "000003,000232")]
        [HttpGet]
        public ActionResult Registrar()
        {
            DateTime thisDay = DateTime.Today;
            int year = thisDay.Year;
            //--
            string mydate1 = "01/01/"+year.ToString();
            string mydate2 = "31/12/" + year.ToString();
            DateTime myConvertedDate1 = DateTime.ParseExact(mydate1, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime myConvertedDate2 = DateTime.ParseExact(mydate2, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //--
            ViewBag.actual1 = mydate1;
            ViewBag.actual2 = mydate2;
            //------
            var parametro = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).ToList();
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp });
            //------
            ViewBag.responsable = new SelectList(result.Select(x => new { idAccRes = x.value, nombre = x.nomComEmp }), "idAccRes", "nombre");
            ViewBag.especialidad = new SelectList(_esp.obteneEspecialidades(), "idEsp", "nomEsp");
            ViewBag.estado = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", ConstantesGlobales.estadoActivo);
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(ActividadModels model)
        {
            if (ModelState.IsValid)
            {
                string tabla = "tb_Activ";
                int idc = enu.buscarTabla(tabla);
                int anioActual = DateTime.Now.Year;
                model.idActiv = anioActual + idc.ToString("D7");
                model.userCreActiv = SessionPersister.Username;
                model.fchCreActiv = DateTime.Now;
                model.idEst = ConstantesGlobales.estadoActivo;

                if (_act.crear(model))
                {
                    enu.actualizarTabla(tabla, idc);
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                }
                else
                {
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>Error.</div>";
                }

                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            //------
            //--
            DateTime thisDay = DateTime.Today;
            int year = thisDay.Year;
            //--
            string mydate1 = "01/01/" + year.ToString();
            string mydate2 = "31/12/" + year.ToString();
            DateTime myConvertedDate1 = DateTime.ParseExact(mydate1, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime myConvertedDate2 = DateTime.ParseExact(mydate2, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //--
            ViewBag.actual1 = mydate1;
            ViewBag.actual2 = mydate2;
            //------
            //------
            var parametro = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).ToList();
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp });
            //------
            ViewBag.responsable = new SelectList(result.Select(x => new { idAccRes = x.value, nombre = x.nomComEmp }), "idAccRes", "nombre", model.idAccRes);
            ViewBag.especialidad = new SelectList(_esp.obteneEspecialidades(), "idEsp", "nomEsp");
            ViewBag.estado = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", ConstantesGlobales.estadoActivo);
            return View(model);
        }
        //ACTIVIDAD_MODIFICAR 000233
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000233")]
        public ActionResult Modificar(string id)
        {
            var model = _act.obtenerItem(id);
            //------
            var parametro = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).ToList();
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp });
            //------
            ViewBag.responsable = new SelectList(result.Select(x => new { idAccRes = x.value, nombre = x.nomComEmp }), "idAccRes", "nombre", model.idAccRes);
            ViewBag.especialidad = new SelectList(_esp.obteneEspecialidades(), "idEsp", "nomEsp");
            ViewBag.estado = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", ConstantesGlobales.estadoActivo);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(ActividadModels model)
        {
            if (ModelState.IsValid)
            {
                model.userModActiv = SessionPersister.Username;
                model.fchModActiv = DateTime.Now;
                TempData["mensaje"] = _act.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.especialidad = new SelectList(_esp.obteneEspecialidades(), "idEsp", "nomEsp");
            ViewBag.estado = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", ConstantesGlobales.estadoActivo);
            return View(model);
        }
        //ACTIVIDAD_ANULAR 000234
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000234")]
        public ActionResult Eliminar(string id)
        {
            var model = _act.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult Eliminar(ActividadModels model)
        {
            TempData["mensaje"] = _act.eliminar(model.idActiv);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
        
        //Reutilizable
        public List<SelectedModels> selectEspecialidad(string idAccRes)
        {
            var especialidad = _esp_usu.obtenerEspecialidadesXusuario(idAccRes).Select(x => new { x.idEsp, x.especialidad.nomEsp }).Distinct();
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            foreach (var v in especialidad)
            {
                cbo.value = v.idEsp;
                cbo.text = v.nomEsp;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            return cboList;
        }
        [HttpPost]
        public JsonResult buscarEspecialidad(string idAccRes)
        {
            return Json(selectEspecialidad(idAccRes), JsonRequestBehavior.AllowGet);
        }
        //DETALLE DE MEDICOS DE UNA ACTIVIDAD
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000321")]
        public ActionResult AgregarMedicos(string id)
        {
            var model = _act.obtenerItemDetalle(id);
            string medicoD = "";
            //**************
            //Logica Hidden
            //**************
            //var mdetalle = model.dMed.ToList();
            //**---***---***----
            var medicos = _med.obtenerClientes().ToList();
            var lista = model.dMed.ToList();
            var mdetalle = lista.Join(medicos, e => e.idCli, d => d.idCli, (e, d) => new { e.idCli, d.nomCli,d.nroMatCli}).OrderBy(x=>x.nomCli).ToList();
            //**---***---***----
            if (mdetalle.Count() != 0)
            {
                foreach (var d in mdetalle)
                {
                    if (medicoD != "")
                    {
                        medicoD += "|";
                    }
                    medicoD += d.idCli + ";" + d.nomCli + ";" + d.nroMatCli;
                }
            }
            //**************
            ViewBag.tb_medico = creaTabla(medicoD);
            //**************
            //----
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult AgregarMedicos(ActividadModels model, string medicoD)
        {
            var m = listaDetalle(medicoD);
            //medico
            List<DetActiv_MedModels> listMed = new List<DetActiv_MedModels>();
            DetActiv_MedModels med = new DetActiv_MedModels();
            if (m != null)
            {
                foreach (var i in m)
                {
                    string[] item = i.Split(';');
                    med.idActiv = model.idActiv;
                    med.idCli = item[2];
                    med.usuCreaDetActMed = SessionPersister.Username;
                    med.usufchCreaDetActMed = DateTime.Now;
                    listMed.Add(med);
                    med = new DetActiv_MedModels();
                }
            }
            string mensaje = "";
            if(_act.guardarDetalleActMedicos(listMed , model.idActiv, out mensaje))
            {
                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            //**************
            ViewBag.tb_medico = creaTabla(medicoD);
            TempData["mensaje"] = mensaje;
            //**************
            return View(model);
        }
        //--------------------------------------------------
        public string creaTabla(string detalle)
        {
            string rows = "";
            //armo tabla tb_Medico
            //**************
            //armo tabla tb_Medico
            if (detalle != "")
            {
                var m = listaDetalle(detalle);
                foreach (var i in m)
                {
                    string[] item = i.Split(';');
                    rows += "<tr>";
                    rows += "<td class='text-center'>" + item[2] + "</td>";
                    rows += "<td class='text-center'>" + item[1] + "</td>";
                    rows += "<td class='hidden'>" + item[0] + "</td>";
                    rows += "<td class='elimfila'><span class='glyphicon glyphicon-remove'></span></td>";
                    rows += "</tr>";
                }
            }
            //*************

            return rows;
        }
        [HttpPost]
        public JsonResult UploadExcel(HttpPostedFileBase FileUpload)
        {
            List<string> data = new List<string>();
            List<ImportacionMedicos> list = new List<ImportacionMedicos>();
            if (FileUpload != null)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName).ToLower();
                string[] validFileTypes = { ".xls", ".xlsx" };

                if (validFileTypes.Contains(extension))
                {
                    if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        string filename = FileUpload.FileName;
                        string targetpath = Server.MapPath("~/Import/Actividad/");
                        FileUpload.SaveAs(targetpath + filename);
                    }

                    string path = string.Format("{0}/{1}", Server.MapPath("~/Import/Actividad/"), Request.Files["FileUpload"].FileName);
                    if (!Directory.Exists(path))
                    {
                        SLDocument sl = new SLDocument(path);
                        int iRow = 5;
                        
                        while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                        {
                            //************************************************
                            string actividad = sl.GetCellValueAsString(iRow, 1);
                            string cmp = sl.GetCellValueAsString(iRow, 2);
                            string nomEspe = sl.GetCellValueAsString(iRow, 3);
                            string nomMed = sl.GetCellValueAsString(iRow, 4);
                            //************************************************
                            ImportacionMedicos item = new ImportacionMedicos();
                            //-------Datos a importar-------
                            item.idActiv = actividad;
                            item.nomEsp = nomEspe;
                            item.nomMed = nomMed;
                            //-------****************-------
                            if (nomEspe!= ConstantesGlobales.Especialidad1 || nomEspe != ConstantesGlobales.Especialidad2)
                            {
                                item.cmp = cmp;
                            }
                            else
                            {
                                if(nomEspe == ConstantesGlobales.Especialidad1)
                                {
                                    item.cmp = "OD-"+cmp;
                                }
                                if(nomEspe == ConstantesGlobales.Especialidad2)
                                {
                                    item.cmp = "OB-" + cmp;
                                }
                            }
                            MedicoModels model = _med.obtenerItemxCMP(cmp);
                            //-------****************-------
                            if (model != null)
                            {
                                string prueba = "";
                                string nombre = item.nomMed;
                                string[] palabras;
                                palabras = nombre.Split(' ');
                                bool verificar = false;
                                int contador = 0;
                                foreach (var c in palabras)
                                {
                                    if (model.nomCli.Contains(c))
                                    {
                                        verificar = true;
                                        contador++;
                                    }
                                }
                                if (!verificar || contador < 3)
                                {
                                    prueba = "- Verificar Nombre con :" + item.nomMed;
                                    item.ver = "2";
                                }
                                else
                                {
                                    item.ver = "3";
                                }
                                item.nomMedBD = model.nomCli;
                                item.obs = "OK " + prueba;
                                item.idCli = model.idCli;
                            }
                            else
                            {
                                item.nomMedBD = "";
                                item.obs = "ERROR! No se encuentra : " + item.cmp;
                                item.idCli = "";
                                item.ver = "1";
                            }
                             
                            list.Add(item);
                            //************************************************
                            iRow++;
                        }
                    }
                    //-----------------------------------------------
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(path)))
                    {
                        System.IO.File.Delete(path);
                    }
                    //-----------------------------------------------
                    return Json(list, JsonRequestBehavior.AllowGet);
                    //-----------------------------------------------
                }
                else
                {
                    //alert message for invalid file format  
                    //data.Add("<ul>");
                    //data.Add("<li>Only Excel file format is allowed</li>");
                    //data.Add("</ul>");
                    //data.ToArray();
                    return Json("EXT", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                //data.Add("<ul>");
                //if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                //data.Add("</ul>");
                //data.ToArray();
                return Json("VACIO", JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult DownloadExcel()
        {
            string path = "/Plantillas/ArchivoDeImportacionDeMedicos.xlsx";
            return File(path, "application/vnd.ms-excel", "ArchivoDeImportacionDeMedicos.xlsx");
        }
        //--------------------------------------------------
        //Reutilizables
        public string[] listaDetalle(string info)
        {
            string[] detalle = null;
            if (info != "")
            {
                detalle = info.Split('|');
            }
            return detalle;
        }
        //--------------------------------------------------
    }
}