using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Trilogia;
using PortalRoemmers.Security;
using System.Web.Mvc;
using System.Linq;
using PortalRoemmers.Areas.Sistemas.Services.Gasto;
using PortalRoemmers.Areas.Sistemas.Services.Visitador;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Ventas.Models.SolicitudGasto;
using System;
using System.Collections.Generic;
using PortalRoemmers.Helpers;
using PortalRoemmers.Filters;
using PortalRoemmers.Areas.Ventas.Services.SolicitudGasto;
using MvcRazorToPdf;
using PortalRoemmers.Areas.Sistemas.Services.Presupuesto;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using PortalRoemmers.Areas.Marketing.Services.Actividad;
using PortalRoemmers.Areas.Marketing.Services.Estimacion;
using PortalRoemmers.Areas.Marketing.Models.SolicitudGastoMkt;
using PortalRoemmers.Areas.Sistemas.Services.Proveedor;
using PortalRoemmers.Areas.Sistemas.Models.Proveedor;
using PortalRoemmers.Areas.Sistemas.Services.Producto;
using System.Web;
using System.IO;
using System.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using PortalRoemmers.Areas.Sistemas.Services.Solicitud;
using System.Data.SqlTypes;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;

namespace PortalRoemmers.Areas.Marketing.Controllers
{
    public class SolicitudGastoMktController : Controller
    {//SOLGASMKT_CONTROLLER 000237
        private SolicitudGastoRepositorio _sis;
        private AsigAproRepositorio _asiapr;
        private UsuarioRepositorio _usu;
        private TipoGastoRepositorio _tgas;
        private ConceptoGastoRepositorio _cgas;
        private Usu_Zon_LinRepositorio _uzl;
        private Pro_Lin_Repositorio _pl;
        private LineaRepositorio _lin;
        private ZonaRepositorio _zon;
        private MonedaRepositorio _mon;
        private EspecialidadRepositorio _esp;
        private TipoCambioRepositorio _tcam;
        private PresupuestoRepositorio _pre;
        private MovimientoPresRepositorio _mov;
        private TipoPagoRepositorio _tipPag;
        private TipoSolRepositorio _tipSol;
        private Esp_Usu_Repositorio _eu;
        private ActividadRepositorio _act;
        private EstimacionRepositorio _esti;
        private ProveedorRepositorio _prov;
        private FamiliaRoeRepositorio _prod;
        private TipoComprobanteRepositorio _tcom;
        private Ennumerador enu;
        private Parametros p;
        private EstadoRepositorio _est;
        private ProductoRepositorio _product;
        private TipoGasto_UsuRepositorio _tipU;
        public SolicitudGastoMktController()
        {
            _tcom = new TipoComprobanteRepositorio();
            _prov = new ProveedorRepositorio();
            _esti = new EstimacionRepositorio();
            _act = new ActividadRepositorio();
            _sis = new SolicitudGastoRepositorio();
            _asiapr = new AsigAproRepositorio();
            _usu = new UsuarioRepositorio();
            _tgas = new TipoGastoRepositorio();
            _cgas = new ConceptoGastoRepositorio();
            _uzl = new Usu_Zon_LinRepositorio();
            _pl = new Pro_Lin_Repositorio();
            _lin = new LineaRepositorio();
            _zon = new ZonaRepositorio();
            _mon = new MonedaRepositorio();
            _esp = new EspecialidadRepositorio();
            _tcam = new TipoCambioRepositorio();
            _pre = new PresupuestoRepositorio();
            _mov = new MovimientoPresRepositorio();
            _tipPag = new TipoPagoRepositorio();
            _tipSol = new TipoSolRepositorio();
            _eu = new Esp_Usu_Repositorio();
            _prod = new FamiliaRoeRepositorio();
            _product = new ProductoRepositorio();
            _est = new EstadoRepositorio();
            enu = new Ennumerador();
            p = new Parametros();
            _tipU = new TipoGasto_UsuRepositorio();
        }
        //SOLGASMKT_LISTAR 000238
        [CustomAuthorize(Roles = "000003,000238")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "",string idAcc = "", string fchEveSolGasI = "", string fchEveSolGasF = "")
        {
            //-----------------------------
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();
            //-----------------------------
            try
            {
                inicio = DateTime.Parse(fchEveSolGasI);
                fin = DateTime.Parse(fchEveSolGasF);
            }
            catch (Exception e)
            {
                DateTime date = DateTime.Now;
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, 1, 1);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = DateTime.Today.ToString("dd/MM/yyyy");
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
            ViewBag.search = search;
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //-----------------------------
            var parametro = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).ToList();
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp });
            //-----------------------------
            ViewBag.gerentesProd = new SelectList(result.Select(x => new { idAccResGP = x.value, nombre = x.nomComEmp }), "idAccResGP", "nombre");
            ViewBag.actividades = new SelectList(_act.obtenerActividades().Where(x => x.idAccRes == idAcc && x.estimacion != null && ((DateTime.Today >= x.fchIniVig) && (DateTime.Today <= x.fchFinVig))).Select(x => new { idActividades= x.idActiv, nomActiv=  x.nomActiv }), "idActividades", "nomActiv");
            //-----------------------------
            var model = _sis.obtenerTodos(pagina, search, ConstantesGlobales.mod_marketing, inicio.ToString(), fin.ToString());
            //-----------------------------
            return View(model);
        }
        //SOLGASMKT_REGISTRAR 000239
        [CustomAuthorize(Roles = "000003,000239")]
        [HttpGet]
        public ActionResult Registrar(string idActividades)
        {
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];

            ViewBag.procedeA = true;
            string familiaD = "";
            string medicoD = "";
            string responsableD = emple.idEmp + ";" + emple.apePatEmp + " " + emple.apeMatEmp + " " + emple.nom1Emp + " " + emple.nom2Emp + ";" + "";
            string actividadD = "";
            string documentoD = "";
            SolicitudGastoModels model = new SolicitudGastoModels();
            model.idAccSol = SessionPersister.UserId;
            model.idAccRes = SessionPersister.UserId;
            model.idMon = ConstantesGlobales.monedaSol;
            ///-----------------------
            ///----------------------- 
            var actual = DateTime.Today.ToString("dd/MM/yyyy");
            if (idActividades != "")
            { 
                var estim = _esti.obtenerItem(idActividades) ;
                var activ = _act.obtenerItem(idActividades);
                ///-----------------
                model.idAccRes = estim.actividad.idAccRes+"-"+_asiapr.obtenerNivAprob(estim.actividad.idAccRes);
                model.idAccApro = _asiapr.obtenerAprobadoresAsignados(estim.actividad.idAccRes).Select(x => x.idAccApro).FirstOrDefault();
                model.idLin = estim.idLin;
                model.idEsp = estim.actividad.idEsp;
                model.idActiv = idActividades;
                model.idActivEst = idActividades;
                model.idTipGas = estim.idTipGas;
                model.idConGas = estim.idConGas;
                model.titSolGas = activ.nomActiv;
                model.idZon = ConstantesGlobales.zona_ninguno;
                actual = estim.actividad.fchIniActiv.ToString("dd/MM/yyyy");
                DateTime date = DateTime.Now;
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, 1, 1);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                if (actual == primero)
                {
                    actual = DateTime.Today.ToString("dd/MM/yyyy");
                }
                List<string> listPres = new List<string>();
                listPres.Add(estim.idPres);
                model.idPres = listPres;
                model.procedeA = true;
                //---------------------------------------------
                var fdetalle = _esti.obtenerDetalleFamilia(idActividades).Select(x => new { x.idFamRoe, x.familia.nomFamRoe, x.areaTerap.numAreaTerap , x.areaTerap.idAreaTerap});
                //---------------------------------------------
                //Apuntar detalle de area terapeutica :si no tiene porque es una familia pasada crear  y si tiene jalar los datos
                //---------------------------------------------
                if (fdetalle.Count() != 0)
                {
                    foreach (var d in fdetalle)
                    {
                        if (familiaD != "")
                        {
                            familiaD += "|";
                        }
                        familiaD += d.idFamRoe + ";" + d.nomFamRoe + ";" + d.numAreaTerap + ";" +d.idAreaTerap;
                    }
                }
                //***--****
                var mdetalle = _esti.obtenerDetalleMedico(idActividades).Select(x => new { x.idCli, x.cliente.nomCli, x.cliente.nroMatCli, x.cliente.idEsp });
                //var mdetalle = model.dMed.Select(x => x.cliente);
                if (mdetalle.Count() != 0)
                {
                    foreach (var d in mdetalle)
                    {
                        if (medicoD != "")
                        {
                            medicoD += "|";
                        }
                        medicoD += d.idCli + ";" + d.nomCli + ";" + d.nroMatCli + ";" + d.idEsp;
                    }
                }
                //***--****
                //---------------------------------------------
            }
            //////--------------------
            //////--------------------
            try
            {
                model.valtipCam = double.Parse(_tcam.obtenerItem(DateTime.Today).monTCVenta.ToString());
            }
            catch (Exception e) {
                TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe ingresar el tipo de cambio ..." + "</div>";
            }
            //////------------------
            cargarCombos(model, idActividades);
            //////------------------
            creaTablas(familiaD, medicoD, responsableD, actividadD,documentoD);
            
            ViewBag.fecha = actual;
            ViewBag.Titulo = model.titSolGas.Trim()+"/";

            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(SolicitudGastoModels model, string familiaD, string medicoD, string responsableD, string actividadD,string documentoD)
        {
            model.modSolGas = ConstantesGlobales.mod_marketing;
            //--
            double monto = 0;
            string areaterap = "";
            string familia = "";
            try
            {
                monto = (double)model.monNeto;
                areaterap = crearDetalleAT(familiaD, monto);
                familia = crearDetalleFamiliaD(familiaD);
            }
            catch (Exception e) { e.Message.ToString(); }

            //--
            ViewBag.procedeA = model.procedeA;
            try
            {
                model.valtipCam = double.Parse(_tcam.obtenerItem(DateTime.Today).monTCVenta.ToString());
            }
            catch (Exception e) { e.Message.ToString(); }
            //******
            //Detalle de Area Terapeutica 

            //******
            if (ModelState.IsValid)
            {
                string tabla = "tb_SolGastos";
                int idc = enu.buscarTabla(tabla);
                model.idSolGas = idc.ToString("D7");

                if (model.procedeA)
                {
                    if (model.monNeto != model.monDetalle)
                    {
                        cargarCombos(model,"");
                        creaTablas(familiaD, medicoD, responsableD, actividadD,documentoD);
                        ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "La suma de gastos no coincide con el subtotal" + "</div>";
                        return View(model);
                    }
                }

                if (!model.procede)//tiene presupuesto
                {
                    if (familiaD != "") {
                        if (model.idPres != null)
                        {
                            var pre = _pre.obtenerPresupuestos(model.idPres);
                            double tp = 0;//total presupuesto
                                          //sumo los presupuesto ,en caso sea diferente lo pongo a la moneda de la solicitud
                            foreach (var p in pre)
                            {
                                if (model.idMon == p.idMon)
                                {
                                    tp = tp + p.Saldo;
                                }
                                else
                                {
                                    //presupuesto en soles con moneda en dolares
                                    if (p.idMon == ConstantesGlobales.monedaDol)
                                    {
                                        tp = tp + Math.Round(p.Saldo * model.valtipCam, 2);
                                    }

                                    if (p.idMon == ConstantesGlobales.monedaSol)
                                    {
                                        tp = tp + Math.Round(p.Saldo / model.valtipCam, 2);
                                    }
                                }
                            }

                            //valido el presupuesto y la solicitud
                            if (tp >= model.monNeto)
                            {//realizo toda la logica para descontar

                                if (guardarSolicitudDetalles(model, familia, medicoD, responsableD, actividadD,documentoD,areaterap))
                                {
                                    Boolean okM = true;
                                    //valido que la suma presupuesto sea mayor al de la solicitud
                                    double ress = (double)model.monNeto;
                                    double des = 0;
                                    double mov = 0;
                                    var j = 0;
                                    do
                                    {
                                        if (model.idMon == pre[j].idMon)
                                        {
                                            ress = ress - pre[j].Saldo;

                                            if (ress > 0)
                                            {
                                                des = pre[j].Saldo;
                                                mov = pre[j].Saldo;
                                            }
                                            else
                                            {
                                                des = ress + pre[j].Saldo;
                                                mov = ress + pre[j].Saldo;
                                            }
                                        }
                                        else
                                        {
                                            if (pre[j].idMon == ConstantesGlobales.monedaSol)
                                            {
                                                ress = ress - Math.Round(pre[j].Saldo / model.valtipCam, 2);

                                                if (ress > 0)
                                                {
                                                    des = pre[j].Saldo;
                                                    mov = Math.Round(pre[j].Saldo / model.valtipCam, 2);
                                                }
                                                else
                                                {
                                                    des = Math.Round((ress + pre[j].Saldo / model.valtipCam) * model.valtipCam, 2);
                                                    mov = Math.Round(ress + pre[j].Saldo / model.valtipCam, 2);
                                                }
                                            }
                                            if (pre[j].idMon == ConstantesGlobales.monedaDol)
                                            {
                                                ress = ress - Math.Round(pre[j].Saldo * model.valtipCam, 2);

                                                if (ress > 0)
                                                {
                                                    des = pre[j].Saldo;
                                                    mov = Math.Round(pre[j].Saldo * model.valtipCam, 2);
                                                }
                                                else
                                                {
                                                    des = Math.Round((ress + pre[j].Saldo * model.valtipCam) / model.valtipCam, 2);
                                                    mov = Math.Round(ress + pre[j].Saldo * model.valtipCam, 2);
                                                }
                                            }
                                        }

                                        //realizo la actualizacion
                                        MovimientoPresModels mp = new MovimientoPresModels();
                                        mp.idPres = pre[j].idPres;
                                        mp.idSolGas = model.idSolGas;
                                        mp.monSolGas = mov;
                                        mp.idMon = model.idMon;
                                        mp.valtipCam = model.valtipCam;
                                        mp.idEst = model.idEst;
                                        mp.usuCrea = SessionPersister.Username;
                                        mp.usufchCrea = DateTime.Now;
                                        Boolean exito = _mov.crear(mp);
                                        if (exito)//si todo sale correcto actualizo el saldo
                                        {
                                            double actualizado = pre[j].Saldo - des;

                                            if (_mov.updateSaldoPres(pre[j].idPres, actualizado))
                                            {
                                                //no se pone nada se creo bien los detalles
                                            }
                                            else
                                            {
                                                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error al modificar saldo presupuesto" + "</div>";
                                                okM = false; break;
                                            }
                                        }
                                        else
                                        {
                                            TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error al guardar el movimiento" + "</div>";
                                            okM = false; break;
                                        }
                                        j++;
                                    } while (ress > 0);

                                    if (okM)//todo salio bien al guardar detalle y firmas,mensaje final
                                    {
                                        if (model.procedeA)
                                        {
                                            calculoGasto(model.idSolGas, model.idActiv);
                                        }
                                        TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                                    }
                                }
                                else
                                {
                                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error al guardar detalle" + "</div>";
                                }

                            }//fin de la logica para descontar
                            else
                            {
                                cargarCombos(model,"");
                                creaTablas(familiaD, medicoD, responsableD, actividadD, documentoD);
                                ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                                TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "No cuenta con suficiente saldo" + "</div>";
                                return View(model);
                            }

                        }
                        else
                        {
                            cargarCombos(model,"");
                            creaTablas(familiaD, medicoD, responsableD, actividadD, documentoD);
                            ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                            TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe elegir un presupuesto" + "</div>";
                            return View(model);
                        }
                    }
                    else
                    {
                        cargarCombos(model,"");
                        creaTablas(familiaD, medicoD, responsableD, actividadD, documentoD);
                        ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe ingresar una familia de producto" + "</div>";
                        return View(model);
                    }
                }
                else
                {
                    if (familiaD != "")
                    {
                      
                        if (guardarSolicitudDetalles(model, familia, medicoD, responsableD, actividadD, documentoD,areaterap))
                        {
                            //DETALLES////////////////////////////////////////////////////////////////////////////////////////////
                            TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                        }
                        else
                        {
                            TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error en guardar detalle" + "</div>";
                        }
                    }
                    else
                    {
                        cargarCombos(model,"");
                        creaTablas(familiaD, medicoD, responsableD, actividadD, documentoD);
                        ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe ingresar una familia de producto" + "</div>";
                        return View(model);
                    }
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
            cargarCombos(model,"");
            creaTablas(familiaD, medicoD, responsableD, actividadD, documentoD);
            return View(model);
        }
        //SOLGASMKT_REGISTRAR 000240
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000240")]
        public ActionResult Modificar(string id)
        {
            string familiaD = "";
            string medicoD = "";
            string responsableD = "";
            string actividadD = "";
            string documentoD = "";
            string areaD = "";
            var model = _sis.obtenerItem(id);

            //Logica Hidden
            var gdetalle = model.dGas.ToList();
            if (gdetalle.Count() != 0)
            {
                foreach (var d in gdetalle)
                {
                    if (actividadD != "")
                    {
                        actividadD += "|";
                    }
                    actividadD += d.gasto.idTipGasAct + ";" + d.gasto.tipoGastoActividad.nomTipGasAct + ";" + d.idActGas + ";" + d.gasto.nomActGas + ";" + d.monProm + ";" + Math.Round(d.monTotal / d.monProm, 0) + ";" + d.monTotal + ";" + d.idActiv;
                }
            }

            var fdetalle = model.dFam.Select(x => x.familia);
            if (fdetalle.Count() != 0)
            {
                foreach (var d in fdetalle)
                {
                    if (familiaD != "")
                    {
                        familiaD += "|";
                    }
                    familiaD +=  "0;"+d.idFamRoe + ";" + d.nomFamRoe + ";0;";
                }
            }

            var adetalle = model.dAre.Select(x => x.areaTerap);
            if (adetalle.Count() != 0)
            {
                foreach (var d in adetalle)
                {
                    if (areaD != "")
                    {
                        areaD += "|";
                    }
                    areaD += d.idAreaTerap + ";" + d.numAreaTerap + ";" + d.desAreaTerap;
                }
            }

            var mdetalle = model.dMed.Select(x => x.cliente);
            if (mdetalle.Count() != 0)
            {
                foreach (var d in mdetalle)
                {
                    if (medicoD != "")
                    {
                        medicoD += "|";
                    }
                    medicoD += d.idCli + ";" + d.nomCli + ";" + d.nroCloUPCli;
                }
            }

            var rdetalle = model.dResp.Select(x => x.responsable);
            if (rdetalle.Count() != 0)
            {
                foreach (var d in rdetalle)
                {
                    if (responsableD != "")
                    {
                        responsableD += "|";
                    }
                    responsableD += d.idEmp + ";" + d.nomComEmp + ";" + d.nroDocEmp;
                }
            }

            model.idTipGas = model.concepto.idTipGas;
            model.usufchMod = DateTime.Now;
            model.usuMod = SessionPersister.Username;
            creaTablas(familiaD, medicoD, responsableD, actividadD, documentoD);
            crearTablaInterna(areaD);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(SolicitudGastoModels model)
        {
            if (ModelState.IsValid)
            {
                Boolean resultado = _sis.modificar(model);
                string mensaje = "";
                //firma
                if (resultado)
                {
                    FirmasSoliGastoModels fs = new FirmasSoliGastoModels();
                    fs.idSolGas = model.idSolGas;
                    fs.idAcc = model.idAccRes;
                    fs.idEst = model.idEst;
                    fs.idNapro = model.idNapro;
                    fs.obsFirSol = "Solicitud Modificada";
                    fs.usuCrea = SessionPersister.Username;
                    fs.usufchCrea = DateTime.Now;

                    if (_sis.mergeFirmas(fs))
                    {
                        if (_mov.updateEstMovPres(model.idSolGas, model.idEst))
                        {
                            mensaje = "<div id='success' class='alert alert-success'>Se modificó el registro.</div>";
                        }
                        else
                        {
                            mensaje = "<div id='warning' class='alert alert-warning'>" + "Se produjo un error en el movimiento" + "</div>";
                        }
                    }
                    else
                    {
                        mensaje = "<div id='warning' class='alert alert-warning'>" + "Se produjo un error en las firmas" + "</div>";
                    }
                }
                else
                {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + "Se produjo un error en modificar" + "</div>";
                }

                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            //en caso de no estar bien buelve a cargar todo
            string familiaD = "";
            string medicoD = "";
            string responsableD = "";
            string actividadD = "";
            string documentoD = "";
            string areaD = "";
            model = _sis.obtenerItem(model.idSolGas);
            //Logica Hidden
            var fdetalle = model.dFam.Select(x => x.familia);
            if (fdetalle.Count() != 0)
            {
                foreach (var d in fdetalle)
                {
                    if (familiaD != "")
                    {
                        familiaD += "|";
                    }
                    familiaD += d.idFamRoe + ";" + d.nomFamRoe + ";" + d.desFamRoe;
                }
            }

            var adetalle = model.dAre.Select(x => x.areaTerap);
            if (adetalle.Count() != 0)
            {
                foreach (var d in adetalle)
                {
                    if (areaD != "")
                    {
                        areaD += "|";
                    }
                    areaD += d.idAreaTerap + ";" + d.numAreaTerap + ";" + d.desAreaTerap;
                }
            }

            var mdetalle = model.dMed.Select(x => x.cliente);
            if (mdetalle.Count() != 0)
            {
                foreach (var d in mdetalle)
                {
                    if (medicoD != "")
                    {
                        medicoD += "|";
                    }
                    medicoD += d.idCli + ";" + d.nomCli + ";" + d.nroCloUPCli;
                }
            }

            var rdetalle = model.dResp.Select(x => x.responsable);
            if (rdetalle.Count() != 0)
            {
                foreach (var d in rdetalle)
                {
                    if (responsableD != "")
                    {
                        responsableD += "|";
                    }
                    responsableD += d.idEmp + ";" + d.nomComEmp + ";" + d.nroDocEmp;
                }
            }

            model.idTipGas = model.concepto.idTipGas;
            creaTablas(familiaD, medicoD, responsableD, actividadD, documentoD);
            crearTablaInterna(areaD);
            return View(model);
        }
        //SOLGASMKT_MODIFICARPRESUPUESTO
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003")]
        public ActionResult ModMovPres(string id)
        {
            string movD = "";
            ViewBag.procedeMov = false;
            var model = _sis.obtenerItem(id);
            if (model.movimiento.Count() != 0)
            {
                var fmov = model.movimiento;
                string concept = "";
                if (fmov.Count() != 0)
                {
                    foreach (var d in fmov)
                    {
                        if (movD != "")
                        {
                            movD += "|";
                        }
                        concept = (d.presupuesto.especialidad == null ? "" : d.presupuesto.especialidad.nomEsp) + "-" + (d.presupuesto.tipogasto == null ? "" : d.presupuesto.tipogasto.nomTipGas) + "-" + (d.presupuesto.concepto == null ? "" : d.presupuesto.concepto.nomConGas);
                        movD += d.idPres + ";" + concept.ToString() + ";" + d.monSolGas;
                    }
                }
            }
            ViewBag.presupuestos = new SelectList(_pre.obtenerPresupuesto().Where(y=>y.idTipoPres==ConstantesGlobales.plan_Mark && (y.fchIniVigencia <= DateTime.Today && y.fchFinVigencia >= DateTime.Today) && (y.idEst != ConstantesGlobales.estadoInactivo)).Select(x => new { idPres = x.idPres, nomPres = (x.especialidad == null ? "" : x.especialidad.nomEsp) + "-" + (x.tipogasto == null ? "" : x.tipogasto.nomTipGas) + "-" + (x.concepto == null ? "" : x.concepto.nomConGas) }), "idPres", "nomPres");
            crearTablaMovPres(movD);
            return View(model);
        }
        //SOLGASMKT_MODIFICARPRESUPUESTO
        [HttpPost]
        [SessionAuthorize]
        public ActionResult ModMovPres(SolicitudGastoModels model, string movD)
        {
            //lista string de movimientos
            var d = listaDetalle(movD);
            //movimientos de pres    
            List<MovimientoPresModels> listPres = new List<MovimientoPresModels>();
            MovimientoPresModels pre = new MovimientoPresModels();
            //Falta un Condicional si es estado de la solicitud es diferente a anulado o a rechazado 
            if (movD != "")
            {
                foreach (var i in d)
                {
                    string[] item = i.Split(';');
                    pre.idPres = item[1];
                    pre.idSolGas = model.idSolGas;
                    pre.monSolGas = Double.Parse(item[3]);
                    pre.diferencia = 0;
                    pre.idMon = model.idMon;
                    pre.valtipCam = model.valtipCam;
                    pre.idEst = model.idEst;
                    pre.usuMod = SessionPersister.Username;
                    pre.usufchMod = DateTime.Today;
                    listPres.Add(pre);
                    pre = new MovimientoPresModels();
                }
            }
            //Verificar saldo del listado de presupuestos
            bool verdad = true;
            string msm = "";
            foreach (var obj in listPres)
            {
                var val = _pre.obtenerUnPresupuesto(obj.idPres);
                var Saldo = val.Saldo;
                if (val.idMon == obj.idMon)
                {
                    if (Saldo < obj.monSolGas)
                    { verdad = false; if (msm == "") msm = obj.idPres; else msm = msm + "," + obj.idPres; }
                    else { obj.diferencia = obj.monSolGas; }
                }
                else
                {
                    //presupuesto en soles con sol. moneda en dolares
                    if (val.idMon == ConstantesGlobales.monedaDol)
                    {
                        var impMov = Math.Round(obj.monSolGas / obj.valtipCam, 2);
                        if (Saldo < impMov)
                        { verdad = false; if (msm == "") msm = obj.idPres; else msm = msm + "," + obj.idPres; }
                        else { obj.diferencia = impMov; };
                    }
                    //presupuesto en dolares con sol. moneda en soles
                    if (val.idMon == ConstantesGlobales.monedaSol)
                    {
                        var impMov = Math.Round(obj.monSolGas * obj.valtipCam, 2);
                        if (Saldo < impMov)
                        { verdad = false; if (msm == "") msm = obj.idPres; else msm = msm + "," + obj.idPres; }
                        else { obj.diferencia = impMov; };
                    }
                }
            }
            //Primera Validacion
            if (verdad)
            {
                if (_mov.modDetMovPres(listPres,model.idSolGas))
                {
                    //ACTUALIZACION DE PRESUPUESTO///////////////////////////////////////////////////////////////////////////
                    
                    //DETALLES////////////////////////////////////////////////////////////////////////////////////////////
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se modifico registro exitosamente.</div>";
                }
            }
            else {
                TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe elegir un presupuesto" + "</div>";
                return View(model);
            }
            //******************************************
            return RedirectToAction("IndexGeneral", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
        [CustomAuthorize(Roles = "000003,000306")]
        public ActionResult IndexGeneral(string menuArea, string menuVista, string fchEveSolGasI = "", string fchEveSolGasF = "", string estado = "", string solicitante = "")
        {
            /********************************************************/
            //----------------------------------------------------------
            //Primero obtenemos el día actual
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            //Asi obtenemos el primer dia del mes actual
            //--------------------------Fecha--------------------------------
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();
            try
            {
                inicio = DateTime.Parse(fchEveSolGasI);
                fin = DateTime.Parse(fchEveSolGasF);
            }
            catch (Exception e)
            {
                //Asi obtenemos el primer dia del mes actual
                //--------------------------Fecha--------------------------------
                DateTime date = DateTime.Now;
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
                inicio = oPrimerDiaDelMes;
                fin = DateTime.Today;
            }
            //----------------------------------------------------------------
            IEnumerable<SolicitudGastoModels> model = null;
            model = _sis.obtenerSolicitudesGeneral(ConstantesGlobales.mod_marketing, solicitante, estado, inicio.ToString("dd/MM/yyyy"), fin.ToString("dd/MM/yyyy")).ToList();
            //--------------------------Fechas--------------------------------
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //--------------------------Combo por usuario--------------------------------
            ViewBag.solicitante = new SelectList(_usu.obtenerUsuariosGeneral().Select(x => new { idAcc = x.idAcc, nombre = x.empleado.nomComEmp }), "idAcc", "nombre", solicitante);
            ViewBag.estado = new SelectList(_est.obteneEstadosGasto(), "idEst", "nomEst", estado);
            ViewBag.State = new SelectList(_est.obteneEstadosAcambiar(), "idEst", "nomEst");
            //---------------------------------------------------------------
            return View(model);
            //---------------------------------------------------------------
        }
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000305")]
        public ActionResult VisualizarGeneral(string id)
        {
            var model = _sis.obtenerItem(id);
            return View(model);
        }
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000251")]
        public ActionResult Visualizar(string id)
        {
            var model = _sis.obtenerItem(id);
            return View(model);
        }
        [HttpGet]
        public ActionResult VisualizarPartial(string id)
        {
            string familiaD = "";
            string medicoD = "";
            string responsableD = "";
            string actividadD = "";
            string documentoD = "";
            string areaTerap = "";
            var model = _sis.obtenerItem(id);

            //Logica Hidden
            var gdetalle = model.dGas.ToList();
            if (gdetalle.Count() != 0)
            {
                foreach (var d in gdetalle)
                {
                    if (actividadD != "")
                    {
                        actividadD += "|";
                    }
                    actividadD += d.gasto.idTipGasAct + ";" + d.gasto.tipoGastoActividad.nomTipGasAct + ";" + d.idActGas + ";" + d.gasto.nomActGas + ";" + d.monProm + ";" + Math.Round(d.monTotal / d.monProm, 0) + ";" + d.monTotal + ";" + d.idActiv;
                }
            }

            var fdetalle = model.dFam.Select(x => x.familia);
            if (fdetalle.Count() != 0)
            {
                foreach (var d in fdetalle)
                {
                    if (familiaD != "")
                    {
                        familiaD += "|";
                    }
                    familiaD += "0;"+ d.idFamRoe + ";" + d.nomFamRoe +  ";0";
                }
            }

            var mdetalle = model.dMed.Select(x => x.cliente);
            if (mdetalle.Count() != 0)
            {
                foreach (var d in mdetalle)
                {
                    if (medicoD != "")
                    {
                        medicoD += "|";
                    }
                    medicoD += d.idCli + ";" + d.nomCli + ";" + d.nroCloUPCli;
                }
            }

            var rdetalle = model.dResp.Select(x => x.responsable);
            if (rdetalle.Count() != 0)
            {
                foreach (var d in rdetalle)
                {
                    if (responsableD != "")
                    {
                        responsableD += "|";
                    }
                    responsableD += d.idEmp + ";" + d.nomComEmp + ";" + d.nroDocEmp;
                }
            }

            var adetalle = model.dAre.Select(x => x.areaTerap);
            if (adetalle.Count() != 0)
            {
                foreach (var d in adetalle)
                {
                    if (areaTerap != "")
                    {
                        areaTerap += "|";
                    }
                    areaTerap += d.idAreaTerap + ";" + d.numAreaTerap + ";" + d.desAreaTerap;
                }
            }

            creaTablas(familiaD, medicoD, responsableD, actividadD, documentoD);
            crearTablaInterna(areaTerap);

            return View(model);
        }
        //Imprimir
        [CustomAuthorize(Roles = "000003,000227")]
        [HttpGet]
        public ActionResult Imprimir(Boolean html, string sol)
        {
            var solicitud = _sis.obtenerItem(sol);
            ViewBag.pdf = html;
            return new PdfActionResult(solicitud)
            {
                FileDownloadName = "Solicitud - " + solicitud.idSolGas.ToString() + " - " + solicitud.estado.nomEst + ".pdf"
            };
        }

        //acciones Json
        [HttpPost]
        public JsonResult anularSolicitud(string idSolGas)
        {
            var variable = _sis.updateEstadoSol(idSolGas, SessionPersister.UserId, ConstantesGlobales.estadoAnulado);
            if (variable)
            {
                //firma
                FirmasSoliGastoModels fs = new FirmasSoliGastoModels();
                fs.idSolGas = idSolGas;
                fs.idAcc = SessionPersister.UserId;
                fs.idEst = ConstantesGlobales.estadoAnulado;
                fs.idNapro = SessionPersister.NivApr;
                fs.obsFirSol = "Solicitud Anulada";
                fs.usuCrea = SessionPersister.Username;
                fs.usufchCrea = DateTime.Now;

                if (_sis.mergeFirmas(fs))
                {
                    var mov = _mov.obtenerMovimientos(idSolGas);
                    double monSolGas = 0;
                    foreach (var m in mov)
                    {
                        if (m.presupuesto.idMon == m.idMon)
                        {
                            monSolGas = m.monSolGas;
                        }
                        else
                        {
                            if (m.presupuesto.idMon == ConstantesGlobales.monedaSol)
                            {
                                monSolGas = Math.Round(m.monSolGas * m.valtipCam, 2);
                            }
                            if (m.presupuesto.idMon == ConstantesGlobales.monedaDol)
                            {
                                monSolGas = Math.Round(m.monSolGas / m.valtipCam, 2);
                            }
                        }
                        double descontar = m.presupuesto.Saldo + monSolGas;

                        //actualizacion
                        Boolean sp = _mov.updateSaldoPres(m.idPres, descontar);
                        Boolean mp = _mov.updateMovPres(idSolGas, m.idPres, ConstantesGlobales.estadoAnulado, 0);

                        //actualizacion
                        if (!sp || !mp) { break; }

                    }

                }
                //Update Estimacion y Pto estimado
                var item = _sis.obtenerItem(idSolGas);
                if(item.idConGas == ConstantesGlobales.conGas_Congreso)
                { 
                    updateDetEstim_PtoEstim(idSolGas);
                }
            }
            return Json(variable, JsonRequestBehavior.AllowGet);
        }

        //busquedas
        [HttpPost]
        public JsonResult buscarActividades(string idAcc)
        {
            //var variable = _act.obtenerConceptoGastos().Where(x => x.idTipGas == idTipGas).Select(y => new { y.idConGas, y.nomConGas });
            var variable = _act.obtenerActividades().Where(x => x.idAccRes == idAcc &&  x.estimacion != null && ((DateTime.Today >= x.fchIniVig) && (DateTime.Today <= x.fchFinVig))).Select(x => new {  x.idActiv,  x.nomActiv }); 
            return Json(variable, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarConceptoTGasto(string idTipGas)
        {
            var variable = _cgas.obtenerConceptoGastos().Where(x => x.idTipGas == idTipGas).Select(y => new { y.idConGas, y.nomConGas });
            return Json(variable, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarAprobadores(string idAccRes)
        {
            var id = idAccRes.Split('-');
            var codigo = id[0];

            var variable = _asiapr.obtenerAprobadoresAsignados(codigo).Select(x => new { x.idAccApro, nom = x.aprobador.empleado.nomComEmp, });
            return Json(variable, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarLinea(string idAccRes)
        {
            return Json(selectLinea(idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarZona(string idAccRes)
        {
            return Json(selectZona(idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarEspecilidad(string idAccRes)
        {
            return Json(selectEspecialida(idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarTipoDeGasto(string idAccRes)
        {
            return Json(selectTipoDeGasto(idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarActividadxResp(string idAccRes)
        {
            return Json(selectActividad(idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult verificarPresupuesto(string resp, string tip,string gas, string esp)
        {
            Parametros p = new Parametros();
            var id = resp.Split('-');
            var idAccRes = id[0];
            if (p.Resultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).Contains(idAccRes))//verificar si va tener presupuesto
            {
                var obtener = _pre.obtenerListaSeleccion(ConstantesGlobales.plan_Mark, idAccRes, "", "", "", tip ,gas, esp);
                if (obtener.Count() != 0)
                {
                    return Json(obtener.Select(x => new { codigo = x.idPres, nombre = x.tipospres.nomTipPres+"-N°: " + x.idPres + " | " + "Vencimiento: " + x.fchFinVigencia.ToShortDateString() + " | " + "Importe: " + x.moneda.simbMon + " " + x.Monto + " | " + "Saldo: " + x.moneda.simbMon + " " + x.Saldo }), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("VACIO", JsonRequestBehavior.AllowGet);
                }

            }
            else//no esta configurada pata tener presupuesto
            {
                return Json("NO", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult buscarActividad(string idAccRes, string idEsp, List<string> idPres)
        {
            var id = idAccRes.Split('-');
            var res = id[0];
            var act = _act.obtenerActividades().Where(x => x.idAccRes == res && x.idEsp == idEsp && x.estimacion != null && ((DateTime.Today >= x.fchIniVig) && (DateTime.Today <= x.fchFinVig))).Select(x => new { x.idActiv, x.nomActiv, x.estimacion.idPres });
            act = act.Where(x => idPres.Contains(x.idPres));
            return Json(act, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarEstimacion(string idActiv)
        {
            var est = _esti.obtenerTodos().Where(x => x.idActiv == idActiv).Select(x => new { x.idActiv, nom = "N°: " + x.idActiv + " | " + "Importe: " + x.monEstGas + " " + x.moneda.nomMon });
            return Json(est, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarTipoGasto(string idActiv)
        {
            var tipGas = selectTipGasto(idActiv);
            return Json(tipGas, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarGasto(string idActiv, string idTipGasAct)
        {
            var gas = selectGasto(idActiv, idTipGasAct);
            return Json(gas, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarDetalleGasto(string idActiv)
        {
            var df=_esti.obtenerDetalleFamilia(idActiv).Select(x=>new {x.idFamRoe,x.familia.nomFamRoe,x.areaTerap.numAreaTerap,x.areaTerap.idAreaTerap});
            return Json(df, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarDetalleMedicoxActiv(string idActiv)
        {
            var dm = _esti.obtenerDetalleMedico(idActiv).Select(x => new { x.idCli, x.cliente.nomCli, x.cliente.nroMatCli });
            return Json(dm, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult autoProveedor(string prefix)
        {
            var provee=_prov.obtenerLaboratorio().Where(x => x.nomProv.ToUpper().StartsWith(prefix.ToUpper()) || x.niffPro.Contains(prefix)).Take(20).Select(x => new { x.idPro, nomProv = x.niffPro + "-" + x.nomProv });
            return Json(provee, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult autoTComprobante(string prefix)
        {
            var tcom = _tcom.obtenerTComprobante().Where(x => x.nomTipComp.ToUpper().StartsWith(prefix.ToUpper())).Select(x => new { x.idCodComp, x.nomTipComp });
            return Json(tcom, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult existeDocumento(string idPro, string idCodComp, string numSerie,string numCorrelativo)
        {
            var doc = _sis.obtenerDetDocumentos().Where(x=>x.idPro== idPro && x.idCodComp== idCodComp && x.numSerie== numSerie && x.numCorrelativo== numCorrelativo).Count();

            if (doc != 0)
            {//si existe el documento
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

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
        public void cargarCombos(SolicitudGastoModels model,string validacion)
        {
            Parametros p = new Parametros();

            //envio los combos ya cargados
            try
            {
                var id = model.idAccRes.Split('-');
                var idAccRes = id[0];
                //Hay una actividad 
                //*****************************************
                if (validacion != "")
                {
                    if (p.Resultado(ConstantesGlobales.Com_SolGas_Reg_Resp).Contains(model.idAccSol))
                    {
                        ViewBag.responsable = new SelectList(_asiapr.obtenerAproYou(model.idAccSol).Select(x => new { idAcc = x.idAcc + "-" + x.idNapro, nombre = x.empleado.nomComEmp }), "idAcc", "nombre", model.idAccRes);
                    }
                    else
                    {
                        ViewBag.responsable = new SelectList(_usu.obtenerUsuarios().Where(x => x.idAcc == model.idAccSol).Select(x => new { idAcc = x.idAcc + "-" + x.idNapro, nombre = x.empleado.nomComEmp }), "idAcc", "nombre", idAccRes + "-" + SessionPersister.NivApr);
                    }
                    //----
                    ViewBag.tipogasto = new SelectList(_tgas.obtenerTipGasto(), "idTipGas", "nomTipGas", model.idTipGas);
                    //----
                    ViewBag.congasto = new SelectList(_cgas.obtenerConceptoGastos().Where(x => x.idTipGas == model.idTipGas), "idConGas", "nomConGas", model.idConGas);
                    ViewBag.actividad = new SelectList(_act.obtenerActividades().Where(x => x.idAccRes == idAccRes).Select(x => new { x.idActiv,x.nomActiv }), "idActiv", "nomActiv", model.idActiv);
                    ViewBag.estimacion = new SelectList(_esti.obtenerTodos().Where(x => x.idActiv == model.idActiv).Select(x => new { x.idActiv, nom = "N°: " + x.idActiv + " | " + "Importe: " + x.monEstGas + " " + x.moneda.nomMon }), "idActiv", "nom", model.idActivEst);
                    ViewBag.presupuesto = new SelectList(_pre.obtenerListaSeleccion(ConstantesGlobales.plan_Mark, idAccRes, "", "", "", model.idTipGas, model.idConGas, model.idEsp).Select(x => new { codigo = x.idPres, nombre = "N°: " + x.idPres + " | " + "Vencimiento: " + x.fchFinVigencia.ToShortDateString() + " | " + "Importe: " + x.moneda.simbMon + " " + x.Monto + " | " + "Saldo: " + x.moneda.simbMon + " " + x.Saldo }), "codigo", "nombre","0000031");
                    //----
                    //*****************************************
                    //----------------------------------------
                    //Si no hay actividad
                    //*****************************************
                }
                else
                {
                    var actividad = _act.obtenerActividades().Where(x => x.idAccRes == idAccRes && x.idEsp == model.idEsp && x.estimacion != null && ((DateTime.Today >= x.fchIniVig) && (DateTime.Today <= x.fchFinVig))).Select(x => new { x.idActiv, x.nomActiv, x.estimacion.idPres });
                    
                    if (model.idPres != null)
                    {
                        actividad = actividad.Where(x => model.idPres.Contains(x.idPres));
                    }

                    if (p.Resultado(ConstantesGlobales.Com_SolGas_Reg_Resp).Contains(model.idAccSol))
                    {
                        ViewBag.responsable = new SelectList(_asiapr.obtenerAproYou(model.idAccSol).Select(x => new { idAcc = x.idAcc + "-" + x.idNapro, nombre = x.empleado.nomComEmp }), "idAcc", "nombre", model.idAccRes + "-" + SessionPersister.NivApr);
                    }
                    else
                    {
                        ViewBag.responsable = new SelectList(_usu.obtenerUsuarios().Where(x => x.idAcc == model.idAccSol).Select(x => new { idAcc = x.idAcc + "-" + x.idNapro, nombre = x.empleado.nomComEmp }), "idAcc", "nombre", idAccRes + "-" + SessionPersister.NivApr);
                    }
                    //-----
                    ViewBag.tipogasto = new SelectList(_tipU.obtenerTipoDeGastoXusuario(idAccRes).Select(x => new { idTipGas = x.idTipGas, nomTipGas = x.tiposDeGastos.nomTipGas }).Distinct(), "idTipGas", "nomTipGas", model.idTipGas);
                    ViewBag.congasto = new SelectList(_cgas.obtenerConceptoGastos().Where(x => x.idTipGas == model.idTipGas), "idConGas", "nomConGas", model.idConGas);
                    ViewBag.actividad = new SelectList(actividad, "idActiv", "nomActiv", model.idActiv);
                    ViewBag.estimacion = new SelectList(_esti.obtenerTodos().Where(x => x.idActiv == model.idActiv).Select(x => new { x.idActiv, nom = "N°: " + x.idActiv + " | " + "Importe: " + x.monEstGas + " " + x.moneda.nomMon }), "idActiv", "nom", model.idActivEst);
                    ViewBag.presupuesto = new SelectList(_pre.obtenerListaSeleccion(ConstantesGlobales.plan_Mark, idAccRes, "", "", "", model.idTipGas,model.idConGas, model.idEsp).Select(x => new { codigo = x.idPres, nombre = "N°: " + x.idPres + " | " + "Vencimiento: " + x.fchFinVigencia.ToShortDateString() + " | " + "Importe: " + x.moneda.simbMon + " " + x.Monto + " | " + "Saldo: " + x.moneda.simbMon + " " + x.Saldo }), "codigo", "nombre");
                    //----
                }
                //*****************************************
                ViewBag.aprobador = new SelectList(_asiapr.obtenerAprobadoresAsignados(idAccRes).Select(x => new { x.idAccApro, nom = x.aprobador.empleado.nomComEmp }), "idAccApro", "nom", model.idAccApro);
                ViewBag.linea = new SelectList(selectLinea(idAccRes), "value", "text", model.idLin);
                ViewBag.zona = new SelectList(selectZona(idAccRes), "value", "text", model.idZon);
                ViewBag.especialidad = new SelectList(selectEspecialida(idAccRes), "value", "text", model.idEsp);
                ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "nomMon", model.idMon);
                ViewBag.tipocambio = model.valtipCam;
                ViewBag.tipopago = new SelectList(_tipPag.obtenerTipoPago(), "idTipPag", "nomTipPag", model.idTipPag);
                ViewBag.tiposolicitud = new SelectList(_tipSol.obtenerTipoSolicitudes(), "idTipSol", "nomTipSol", model.idTipSol);
                ViewBag.tipogastoA = new SelectList(selectTipGasto(model.idActiv), "value", "text", model.idTipGasAct);
                ViewBag.gastoA = new SelectList(selectGasto(model.idActiv, model.idTipGasAct), "value", "text", model.idActGas);                
            }
            catch (Exception e){

            }
        }
        public void crearTablaMovPres(string movimiento)
        {
            int indice = 0;
            string rowsMv = "";
            //armo tabla tb_familia
            if (movimiento != "")
            {
                var f = listaDetalle(movimiento);
                foreach (var i in f)
                {
                    indice++;
                    string[] item = i.Split(';');
                    rowsMv += "<tr>";
                    rowsMv += "<td class='text-center'>" + indice + "</td>";
                    rowsMv += "<td class='text-center'>" + item[0] + "</td>";
                    rowsMv += "<td class='text-center'>" + item[1] + "</td>";
                    rowsMv += "<td class='text-center'>" + item[2] + "</td>";
                    rowsMv += "<td><button type='button' class='btn btn-info btn-outline btn-circle btn-lg m-r-5 delete' onclick='DeleteFila(event)' id='btneliminar'><i class='ti ti-trash'></i></button><button type = 'button' class='btn btn-info btn-outline btn-circle btn-lg m-r-5 editar' onclick='EditarFila(event)' id='btneditar'><a data-toggle='modal' title='Editar' href='#responsive-modal-PresEdit'><i class='ti ti-pencil-alt'></i></a></button></td>";
                    rowsMv += "</tr>";
                }
            }
            ViewBag.tbMovimiento = rowsMv;
        }
        public void crearTablaInterna(string areaTerap)
        {
            string rowsAT = "";
            //armo tabla tb_familia
            if (areaTerap != "")
            {
                var f = listaDetalle(areaTerap);
                foreach (var i in f)
                {
                    string[] item = i.Split(';');
                    rowsAT += "<tr>";
                    rowsAT += "<td class='text-center'>" + item[0] + "</td>";
                    rowsAT += "<td class='text-center'>" + item[1] + "</td>";
                    rowsAT += "<td class='text-center'>" + item[2] + "</td>";
                    rowsAT += "<td class='elimfila'><span class='glyphicon glyphicon-remove'></span></td>";
                    rowsAT += "</tr>";
                }
            }
            ViewBag.tbareaTerap = rowsAT;
        }
        public void creaTablas(string familiaD, string medicoD, string responsableD,string actividadD,string documentoD)
        {
            string rowsF = "";
            string rowsM = "";
            string rowsR = "";
            string rowsA = "";
            string rowsD = "";

            //armo tabla tb_actividad
            if (documentoD != "")
            {
                var a = listaDetalle(documentoD);
                foreach (var i in a)
                {
                    string[] item = i.Split(';');
                    rowsD += "<tr>";
                    rowsD += "<td class='text-center hidden'>" + item[0] + "</td>";
                    rowsD += "<td class='text-center'>" + item[1] + "</td>";
                    rowsD += "<td class='text-center hidden'>" + item[2] + "</td>";
                    rowsD += "<td class='text-center'>" + item[3] + "</td>";
                    rowsD += "<td class='text-center'>" + item[4] + "</td>";
                    rowsD += "<td class='text-center'>" + item[5] + "</td>";
                    rowsD += "<td class='elimfila'><span class='glyphicon glyphicon-remove'></span></td>";
                    rowsD += "</tr>";
                }
            }
            //armo tabla tb_actividad
            if (actividadD != "")
            {
                var a = listaDetalle(actividadD);
                foreach (var i in a)
                {
                    string[] item = i.Split(';');
                    rowsA += "<tr>";
                    rowsA += "<td class='text-center hidden'>" + item[0] + "</td>";
                    rowsA += "<td class='text-center'>" + item[1] + "</td>";
                    rowsA += "<td class='text-center hidden'>" + item[2] + "</td>";
                    rowsA += "<td class='text-center'>" + item[3] + "</td>";
                    rowsA += "<td class='text-center hidden'>" + item[4] + "</td>";
                    rowsA += "<td class='text-center hidden'>" + item[5] + "</td>";
                    rowsA += "<td class='text-center'>" + item[6] + "</td>";
                    rowsA += "<td class='text-center hidden'>" + item[7] + "</td>";
                    rowsA += "<td class='elimfila'><span class='glyphicon glyphicon-remove'></span></td>";
                    rowsA += "</tr>";
                }
            }
            //armo tabla tb_familia
            if (familiaD != "")
            {
                var f = listaDetalle(familiaD);
                foreach (var i in f)
                {
                    string[] item = i.Split(';');
                    rowsF += "<tr>";
                    rowsF += "<td class='hidden'>" + item[0] + "</td>";
                    rowsF += "<td class='text-center'>" + item[1] + "</td>";
                    rowsF += "<td class='text-center'>" + item[2] + "</td>";
                    rowsF += "<td class='hidden'>" + item[3] + "</td>";
                    rowsF += "<td class='text-center'>" + _est.obtenerItem(item[2]) + "</td>";
                    rowsF += "<td class='elimfila'><span class='glyphicon glyphicon-remove'></span></td>";
                    rowsF += "</tr>";
                }
            }
            //armo tabla tb_Medico
            if (medicoD != "")
            {
                var m = listaDetalle(medicoD);
                foreach (var i in m)
                {
                    string[] item = i.Split(';');
                    rowsM += "<tr>";
                    rowsM += "<td class='text-center'>" + item[0] + "</td>";
                    rowsM += "<td class='text-center'>" + item[1] + "</td>";
                    rowsM += "<td class='text-center'>" + item[2] + "</td>";
                    rowsM += "<td class='text-center'>" + item[3] + "</td>";
                    rowsM += "<td class='elimfila'><span class='glyphicon glyphicon-remove'></span></td>";
                    rowsM += "</tr>";
                }
            }
            //armo tabla tb_responsable
            if (responsableD != "")
            {
                var r = listaDetalle(responsableD);
                foreach (var i in r)
                {
                    string[] item = i.Split(';');
                    rowsR += "<tr>";
                    rowsR += "<td class='text-center'>" + item[0] + "</td>";
                    rowsR += "<td class='text-center'>" + item[1] + "</td>";
                    rowsR += "<td class='text-center'>" + item[2] + "</td>";
                    rowsR += "<td class='elimfila'><span class='glyphicon glyphicon-remove'></span></td>";
                    rowsR += "</tr>";
                }
            }

            ViewBag.tbFamilia = rowsF;
            ViewBag.tbMedico = rowsM;
            ViewBag.tbResponsable = rowsR;
            ViewBag.tbActividad = rowsA;
            ViewBag.tbDocumento = rowsD;
        }
        public List<SelectedModels> selectLinea(string idAccRes)
        {
            var i = idAccRes.Split('-');
            var res = i[0];

            //Parametros p = new Parametros();
            var responsable = _usu.obtenerUsuarios().Where(z => z.idAcc == res).FirstOrDefault();

            var linea = _lin.obtenerLineas().Select(x => new { x.idLin, x.nomLin });
            /*if (p.Resultado(ConstantesGlobales.Com_SolGas_Reg_Cas_01).Contains(responsable.empleado.idCarg))
            {
                var trilogia = _uzl.obtenerTrilogia();
                linea = trilogia.Where(x => x.idAcc == res).Select(x => new { x.linea.idLin, x.linea.nomLin }).Distinct();
            }
            else if (p.Resultado(ConstantesGlobales.Com_SolGas_Reg_Cas_02).Contains(responsable.empleado.idCarg))
            {
                linea = _pl.obtenerLinZonxUsu(responsable.idEmp).Select(x => new { x.linea.idLin, x.linea.nomLin }).Distinct();
            }*/

            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            foreach (var v in linea)
            {
                cbo.value = v.idLin;
                cbo.text = v.nomLin;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }

            return cboList;
        }
        public List<SelectedModels> selectZona(string idAccRes)
        {
            var i = idAccRes.Split('-');
            var res = i[0];

            Parametros p = new Parametros();
            var responsable = _usu.obtenerUsuarios().Where(z => z.idAcc == res).FirstOrDefault();

            var zona = _zon.obtenerZonas().Select(x => new { x.idZon, x.nomZon });
            if (p.Resultado(ConstantesGlobales.Com_SolGas_Reg_Cas_01).Contains(responsable.empleado.idCarg))
            {
                var trilogia = _uzl.obtenerTrilogia();
                zona = trilogia.Where(x => x.idAcc == res).Select(x => new { x.zona.idZon, x.zona.nomZon }).Distinct();
            }

            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            foreach (var v in zona)
            {
                cbo.value = v.idZon;
                cbo.text = v.nomZon;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }

            return cboList;

        }
        public List<SelectedModels> selectEspecialida(string idAccRes)
        {
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            var i = idAccRes.Split('-');
            var res = i[0];
            var esp = _eu.obtenerEspecialidadesXusuario(res);

            if (esp.Count() != 0)
            {
                foreach (var v in esp)
                {
                    cbo.value = v.idEsp;
                    cbo.text = v.especialidad.nomEsp;
                    cboList.Add(cbo);
                    cbo = new SelectedModels();
                }
            }
            else
            {
                var Sesp = _esp.obteneEspecialidades();

                foreach (var v in Sesp)
                {
                    cbo.value = v.idEsp;
                    cbo.text = v.nomEsp;
                    cboList.Add(cbo);
                    cbo = new SelectedModels();
                }
            }
            return cboList;
        }
        public List<SelectedModels> selectTipoDeGasto(string idAccRes)
        {
            var idAcc = idAccRes.Split('-');
            var tipoDeGasto = _tipU.obtenerTipoDeGastoXusuario(idAcc[0]).Select(x => new { x.idTipGas, x.tiposDeGastos.nomTipGas }).Distinct();
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            foreach (var v in tipoDeGasto)
            {
                cbo.value = v.idTipGas;
                cbo.text = v.nomTipGas;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            return cboList;
        }
        public List<SelectedModels> selectActividad(string idAccRes)
        {
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            var i = idAccRes.Split('-');
            var res = i[0];
            var act = _act.obtenerActividades().Where(x=>x.idAccRes== res);

            if (act.Count() != 0)
            {
                foreach (var v in act)
                {
                    cbo.value = v.idActiv;
                    cbo.text = v.nomActiv;
                    cboList.Add(cbo);
                    cbo = new SelectedModels();
                }
            }
            return cboList;
        }
        public List<SelectedModels> selectTipGasto(string idActiv)
        {
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            var obt=_esti.obtenerDetalleEstimacion(idActiv).Select(x=> new { x.gastoActiv.idTipGasAct, x.gastoActiv.tipoGastoActividad.nomTipGasAct }).Distinct();

            foreach (var v in obt)
            {
                cbo.value = v.idTipGasAct;
                cbo.text = v.nomTipGasAct;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            return cboList;
        }
        public List<SelectedModels> selectGasto(string idActiv,string idTipGasAct)
        {
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            var obt = _esti.obtenerDetalleEstimacion(idActiv).Where(x=>x.gastoActiv.idTipGasAct== idTipGasAct).Select(x => new { x.gastoActiv.idActGas, x.gastoActiv.nomActGas }).Distinct();

            foreach (var v in obt)
            {
                cbo.value = v.idActGas;
                cbo.text = v.nomActGas;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            return cboList;
        }
        public string crearDetalleFamiliaD(string familiaD)
        {
            //****
            string detalle = "";
            //****
            //Se convierte con el metodo listadetalle
            //****
            var f = listaDetalle(familiaD);
            //****
            string cadenaFam = "";
            string cadena = "";
            //****
            if (f != null)
            {
                //*************************************************************
                //1.-De la cadena en ";" extraer no repetidas las familias 
                //*************************************************************
                foreach (var i in f)
                {
                    string[] item = i.Split(';');
                    //Llenar en una cadena la familia 
                    cadena = item[0] + ";" + item[1] + ";" + item[1];
                    if (!cadenaFam.Contains(item[0]))
                    {
                        if (cadenaFam != "")
                        { cadenaFam += "|"; }
                        cadenaFam += cadena;
                    }
                }
                //****
                detalle = cadenaFam;
            }
            return detalle;
        }
        public string crearDetalleAT(string familiaD, double importe)
        {
            //****
            string detalle="";
            //****
            //Se convierte con el metodo listadetalle
            //****
            var f = listaDetalle(familiaD);
            //****
            int filas = f.Count();
            string[,] vecDetAT;
            vecDetAT = new string[filas, 3];
            //double total = 0.0;
            double porcen = 0;
            string cadenaFam = "";
            string cadena = "";
            double diferencia = 100.00;
            //****
            int indice = 0;
            //****
            if (filas != 0)
            {
                porcen = (double) 100 / filas;
                porcen = Math.Round(porcen, 2, MidpointRounding.AwayFromZero);
            }
            //****
            if (f != null)
            {
                //*************************************************************
                //1.-De la cadena en ";" extraer no repetidas las familias 
                //*************************************************************
                foreach (var i in f)
                {
                    string[] item = i.Split(';');
                    //Llenar matriz de datos unidos
                    vecDetAT[indice, 0] = item[0];
                    vecDetAT[indice, 1] = item[3];
                    vecDetAT[indice, 2] = porcen.ToString();
                    //aumentar indice
                    indice++;
                    //Llenar en una cadena la familia 
                    cadena = item[0]+";"+item[1]+";"+ item[1];
                    if(!cadenaFam.Contains(item[0]))
                    {
                        if (cadenaFam != "")
                        { cadenaFam += " | "; }
                        cadenaFam += cadena;
                    }
                    diferencia = diferencia - porcen;
                }
                //****
                string ultimoM = vecDetAT[filas - 1, 2];
                double ultimoP = double.Parse(ultimoM);
                ultimoP = ultimoP + diferencia;
                vecDetAT[filas - 1, 2] = ultimoP.ToString();
                //*************************************************************
                //2.-Del Vector creado se pasa a objetos de Area Terapeutica
                //*************************************************************
                //string[] area = areas.Split('|');
                DetSolGasto_AreaTerapModels model = new DetSolGasto_AreaTerapModels();
                List<DetSolGasto_AreaTerapModels> list = new List<DetSolGasto_AreaTerapModels>();
                //****
                for(int i=0;i<vecDetAT.GetLength(0);i++)
                {
                    string AT = "";
                    double porc = 0;
                    AT= vecDetAT[i, 1];
                    porc = double.Parse(vecDetAT[i, 2]);
                    //----
                    if(list == null)
                    {
                        model.idAreaTerap = AT;
                        model.valPorcen = porc;
                        list.Add(model);
                    }
                    if(list.Any(item => item.idAreaTerap == AT))
                    {
                        var m = list.Where(t => t.idAreaTerap == AT).FirstOrDefault();
                        m.valPorcen = m.valPorcen + porc;
                    }
                    else
                    {
                        model.idAreaTerap = AT;
                        model.valPorcen = porc;
                        list.Add(model);
                    }
                    //----
                    model = new DetSolGasto_AreaTerapModels();
                    //----
                }
                //*************************************************************
                //3.-Pasar de la clase a la cadena de texto
                //*************************************************************
                double valor = 0;
                int cantFil = list.Count();
                int counter = 0;
                double dif = importe;
                foreach (var d in list)
                {
                    counter++;
                    valor = importe * (d.valPorcen/100);
                    valor = Math.Round(valor, 2, MidpointRounding.AwayFromZero);
                    dif = dif - valor;
                    if (cantFil==counter)
                    {
                        valor = valor + dif;
                    }
                    if (detalle != "")
                    {
                        detalle += "|";
                    }
                    detalle += d.idAreaTerap + ";" + d.valPorcen + ";" + valor;
                }
                //**************************************************************
            }
            return detalle;
        }
        public Boolean guardarSolicitudDetalles(SolicitudGastoModels model, string familiaD, string medicoD, string responsableD,string actividadD,string documentoD,string areaterap)
        {
            var codigos = model.idAccRes.Split('-');
            model.idAccRes = codigos[0];
            model.idNapro = codigos[1];
            var resultado = _sis.crear(model);
            if (resultado)
            {
                //DETALLES////////////////////////////////////////////////////////////////////////////////////////////
                //Creo la firma
                FirmasSoliGastoModels fs = new FirmasSoliGastoModels();
                fs.idSolGas = model.idSolGas;
                fs.idAcc = model.idAccRes;
                fs.idEst = model.idEst;
                fs.idNapro = model.idNapro;
                fs.obsFirSol = "Solicitud Creada";
                fs.usuCrea = SessionPersister.Username;
                fs.usufchCrea = DateTime.Now;
                _sis.mergeFirmas(fs);

                var f = listaDetalle(familiaD);
                var m = listaDetalle(medicoD);
                var r = listaDetalle(responsableD);
                var g = listaDetalle(actividadD);
                var d= listaDetalle(documentoD);
                var a = listaDetalle(areaterap);

                //familia
                List<DetSolGasto_FamModels> listFam = new List<DetSolGasto_FamModels>();
                DetSolGasto_FamModels fam = new DetSolGasto_FamModels();
                if (f != null)
                {
                    foreach (var i in f)
                    {
                        string[] item = i.Split(';');
                        fam.idFamRoe = item[0];
                        fam.idSolGas = model.idSolGas;
                        fam.valPorcen = 0.0;
                        fam.obsFam = "";
                        fam.usuCrea = SessionPersister.Username;
                        fam.usufchCrea = DateTime.Now;
                        listFam.Add(fam);
                        fam = new DetSolGasto_FamModels();
                    }
                }


                //medico
                List<DetSolGasto_MedModels> listMed = new List<DetSolGasto_MedModels>();
                DetSolGasto_MedModels med = new DetSolGasto_MedModels();
                if (m != null)
                {
                    foreach (var i in m)
                    {
                        string[] item = i.Split(';');
                        med.idCli = item[0];
                        med.idSolGas = model.idSolGas;
                        med.valPorcen = 0.0;
                        med.usuCrea = SessionPersister.Username;
                        med.usufchCrea = DateTime.Now;
                        listMed.Add(med);
                        med = new DetSolGasto_MedModels();
                    }
                }

                //responsable
                List<DetSolGasto_RespModels> listRes = new List<DetSolGasto_RespModels>();
                DetSolGasto_RespModels res = new DetSolGasto_RespModels();
                if (r != null)
                {
                    foreach (var i in r)
                    {
                        string[] item = i.Split(';');
                        res.idEmp = item[0];
                        res.idSolGas = model.idSolGas;
                        res.valPorcen = 0.0;
                        res.usuCrea = SessionPersister.Username;
                        res.usufchCrea = DateTime.Now;
                        listRes.Add(res);
                        res = new DetSolGasto_RespModels();
                    }
                }

                //gasto
                /*terminar*/
                List<DetSolGasto_GasModels> listGas = new List<DetSolGasto_GasModels>();
                DetSolGasto_GasModels gas = new DetSolGasto_GasModels();
                if (g != null)
                {
                    foreach (var i in g)
                    {
                        string[] item = i.Split(';');
                        gas.idSolGas = model.idSolGas;
                        gas.idActiv = model.idActiv;
                        gas.idActGas = item[2];
                        gas.monTotal =double.Parse(item[6]);
                        gas.monProm = double.Parse(item[4]);
                        gas.obsAct = "se registro detalle gasto";
                        gas.usuCrea = SessionPersister.Username;
                        gas.usufchCrea = DateTime.Now;
                        listGas.Add(gas);
                        gas = new DetSolGasto_GasModels();
                    }
                }

                //Documento
                List<DetSolGasto_DocModels> listDoc = new List<DetSolGasto_DocModels>();
                DetSolGasto_DocModels doc = new DetSolGasto_DocModels();
                if (d != null)
                {
                    int id = 1;
                    foreach (var i in d)
                    {
                        string[] item = i.Split(';');

                        doc.idSolGas = model.idSolGas;
                        doc.idDetDoc = id.ToString();
                        doc.idPro = item[0];
                        doc.idCodComp = item[2];
                        doc.numSerie = item[4];
                        doc.numCorrelativo = item[5];
                        doc.usuCrea = SessionPersister.Username;
                        doc.usufchCrea = DateTime.Now;
                        listDoc.Add(doc);
                        doc = new DetSolGasto_DocModels();
                        id++;
                    }
                }
                //Area Terapeutica
                List<DetSolGasto_AreaTerapModels> listArea = new List<DetSolGasto_AreaTerapModels>();
                DetSolGasto_AreaTerapModels area = new DetSolGasto_AreaTerapModels();
                if (a != null)
                {
                    foreach (var i in a)
                    {
                        string[] item = i.Split(';');
                        area.idAreaTerap = item[0];
                        area.idSolGas = model.idSolGas;
                        area.valPorcen = double.Parse(item[1]);
                        area.valor = double.Parse(item[2]);
                        area.usuCrea = SessionPersister.Username;
                        area.usufchCrea = DateTime.Now;
                        listArea.Add(area);
                        area = new DetSolGasto_AreaTerapModels();
                    }
                }

                //guadamos detalles
                var rpt1 = _sis.crearDetalleF(listFam);
                var rpt2 = _sis.crearDetalleM(listMed);
                var rpt3 = _sis.crearDetalleR(listRes);
                var rpt4 = false;
                var rpt5 = _sis.crearDetalleD(listDoc);
                var rpt6 = _sis.crearDetalleA(listArea);

                if (model.procedeA)
                {
                    rpt4 = _sis.crearDetalleG(listGas);
                }
                else
                {
                    rpt4 = true;
                }
                if (rpt1 & rpt2 & rpt3 & rpt4 & rpt6)
                {//detalle fue generado bien
                    return true;
                }
                else
                {//detalle fue generado mal
                    return false;
                }
            }
            else
            {//cabecera fue generado mal
                return false;
            }
        }
        public Boolean calculoGasto(string idSolGas, string idActiv)
        {
            string mensaje = "";
            var dSolGas= _esti.obtenerDetalleSolGas(idActiv);

            var dSolGasSol = dSolGas.Where(x=>x.idSolGas== idSolGas).ToList();

            foreach (var i in dSolGasSol)
            {
                //Se llama el detalle de estimacion de gastos por Actividad y Gasto
                var anterior = _esti.obtenerDetalleEstimacion(idActiv).Where(x => x.idActGas == i.idActGas).FirstOrDefault();
                //****Se declaran las variables a usar
                double salAnt = 0;
                double salAct = 0;
                double dif = 0;

                //**********************************
                //Se guarda el salReal Anterior
                salAnt = anterior.salReal;
                //**********************************
                //Se valida si existe 1 registro por Actividad y Gasto
                int cant = dSolGas.Where(x => x.idActGas==i.idActGas).Select(x =>  x.idSolGas ).Distinct().Count();

                if (cant == 1)
                {
                    if(i.solicitud.idMon == anterior.cabecera.idMon)
                    { 
                        salAct = i.monTotal; 
                    }
                    else
                    {
                        if (i.solicitud.idMon == ConstantesGlobales.monedaSol)
                        {
                            salAct = Math.Round(i.monTotal / i.solicitud.valtipCam, 2);
                        }
                    }
                    _esti.modificarDetEstimSalReal(salAct, i.idActiv, i.idActGas, out mensaje);
                }
                else
                {
                    if (i.solicitud.idMon == anterior.cabecera.idMon)
                    {
                        salAct = i.monTotal + anterior.salReal;
                    }
                    else
                    {
                        if (i.solicitud.idMon == ConstantesGlobales.monedaSol)
                        {
                            salAct = Math.Round(i.monTotal / i.solicitud.valtipCam, 2) + anterior.salReal;
                        }
                    }
                    _esti.modificarDetEstimSalReal(salAct, i.idActiv, i.idActGas, out mensaje);
                }
                //**********************************
                //Se busca la diferencia entre saldo Real Anterior y Actual
                dif = salAnt - salAct;
                //Se determino el valor del saldo estimado
                var pre = _pre.obtenerItem(anterior.cabecera.idPres);
                pre.diferencia = dif;
                //Se actualiza el saldo del presupuesto estimado
                _pre.modificarSaldoEstimado(pre, out mensaje);
            }

                return true;
        }
        public bool updateDetEstim_PtoEstim(string id)
        {
            string mensaje = "";
            double sumaEstA = 0;
            string idPr = "";
            try
            {
                //var movEst = _sis.    ().Where(x => x.dGas.Select(y => y.idSolGas).Contains(id)).ToList();
                var movEst = _sis.obtenerDetSolGastos(id).ToList();
                //var anterior = _esti.obtenerDetalleEstimacion().Where(x => x.idActGas == i.idActGas).FirstOrDefault();
                foreach (var i in movEst)
                {
                    var ant = _esti.obtenerDetalleEstimacion(i.idActiv).Where(x => x.idActGas == i.idActGas).FirstOrDefault();
                    double dif = 0;

                    double valor = i.monTotal;
                    if (i.solicitud.idMon == ant.cabecera.idMon)
                    {
                        dif = valor;
                    }
                    else
                    {
                        if (i.solicitud.idMon == ConstantesGlobales.monedaSol)
                        {
                            dif = Math.Round(valor / i.solicitud.valtipCam, 2);
                        }
                    }
                    sumaEstA += dif;
                    double salAct = ant.salReal - dif;
                    _esti.modificarDetEstimSalReal(salAct, i.idActiv, i.idActGas, out mensaje);
                    idPr = ant.cabecera.idPres;
                }
                //**********************************
                //Se determino el valor del saldo estimado
                var pre = _pre.obtenerItem(idPr);
                pre.diferencia = sumaEstA;
                //Se actualiza el saldo del presupuesto estimado
                _pre.modificarSaldoEstimado(pre, out mensaje);
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error :" + ex.Message;
                return false;
            }
        }
        [HttpPost]
        public JsonResult guardarFile(HttpPostedFileBase uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.ContentLength > 0)
            {
                //------ Definir el InputStream para la lectura del file --------
                byte[] FileByteArray = new byte[uploadedFile.ContentLength];
                uploadedFile.InputStream.Read(FileByteArray, 0, uploadedFile.ContentLength);
                //---------------------- captar key del Form -------------------
                //Captar key el idSol  and UploadPath
                string solicitud = Convert.ToString(Request.Form["solicitud"]);
                //************************** GUARDAR FILE ***********************
                //Use Namespace called :  System.IO
                string FileName = Path.GetFileNameWithoutExtension(uploadedFile.FileName);
                //To Get File Extension
                string FileExtension = Path.GetExtension(uploadedFile.FileName);
                //Add Current Date To Attached File Name
                //DateTime.Now.ToString("yyyyMMdd") 
                FileName = FileName.Trim() + FileExtension;
                //----------------Validar que no se repita el nombre--------------
                var similares = _sis.obtenerDetSolFiles().Where(x => x.nomFile == FileName).ToList();
                if (similares.Count() > 0)
                {
                    return Json("NO", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Get Upload path from Web.Config file AppSettings.
                    string UploadPath = ConfigurationManager.AppSettings["UserFilePath"].ToString();
                    //To copy and save file into server.
                    uploadedFile.SaveAs(Path.Combine(@UploadPath, FileName));
                    //*************************** CONVERSION DE PDF A IMG ******************************
                    //En Stand By
                    //*************************** GUARDAR EN BD ******************************
                    //Crear el objeto File
                    DetSolGasto_FileModels model = new DetSolGasto_FileModels();
                    model.idSolGas = solicitud;
                    model.nomFile = FileName;
                    string AbsolutePath = UploadPath + FileName;
                    string RelativePath = AbsolutePath.Replace(UploadPath, "../Import/GastoMkt/");
                    model.pathFile = RelativePath;
                    _sis.crearDetalleFile(model);
                    //***************************************************************
                    return Json("SI", JsonRequestBehavior.AllowGet);
                }
                //---------------------------------------------------------------
            }
            else
            { return Json("NO", JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public JsonResult obtenerFiles(string id)
        {
            var files = _sis.obtenerDetSolFiles().Where(x=>x.idSolGas==id).Select(x => new {  nom = x.nomFile, path = x.pathFile, idFile = x.idFile });
            return Json(files, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult eliminarFile(string idSolGas,string idFile, string filename)
        {
            try { 
                //Eliminar archivo de
                string fullPath = Request.MapPath("../Import/GastoMkt/" + filename);
                if (System.IO.File.Exists(fullPath))
                {
                    int idf = Int32.Parse(idFile);
                    //Eliminar en la BD el objeto File
                    _sis.eliminarDetalleFile(idSolGas, idf);

                    //Eliminar el file 
                    System.IO.File.Delete(fullPath);

                    return Json("SI", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NO", JsonRequestBehavior.AllowGet);
                }
            }catch(Exception ex)
            {
                string mensaje = ex.Message.ToString();
                return Json("NO", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000371")]
        public JsonResult modificarEstado(string sol, string idNewEst, string idOldEst,string obs)
        {
            //valores iniciales
            Boolean correcto = true;
            Boolean TienePresupuesto = false;
            Boolean TieneActividad = false;
            //---
            var movpres = _mov.obtenerMovimientos(sol).ToList();
            var actividad = _sis.obtenerDetSolGastos(sol).ToList();
            //---
            string idPres = "";
            string idMon = "";
            double monNeto = 0;
            double valTC = 0;
            string idAct = "";
            string idActGas = "";
            //---
            if (movpres.Count()!=0)
            {
                TienePresupuesto = true;
                foreach (var r in movpres)
                {
                    idPres = r.idPres;
                    idMon = r.idMon;
                    monNeto = r.monSolGas;
                    valTC = r.valtipCam;
                }
            }
            if (actividad.Count() != 0)
            {
                TieneActividad = true;
                foreach (var a in actividad)
                {
                    idAct = a.idActiv;
                    idActGas = a.idActGas;
                }
            }
            //Obtener registros
            switch (idNewEst)
            {
                //ESTADO ANULADO
                case "14":
                    if (idOldEst==ConstantesGlobales.estadoPreApro || idOldEst == ConstantesGlobales.estadoAprobado || idOldEst==ConstantesGlobales.estadoAtendido)
                    {
                        //******************************************
                        //1.Actualizar estado de la solicitud
                        _sis.updateEstadoSol(sol, "", idNewEst);
                        //2.Agregar firma del cambio de estado
                        FirmasSoliGastoModels f = new FirmasSoliGastoModels();
                        f.idSolGas = sol;
                        f.idAcc = SessionPersister.UserId;
                        f.idEst = idNewEst;
                        f.usuCrea = SessionPersister.Username;
                        f.usufchCrea = DateTime.Now;
                        f.idNapro = SessionPersister.NivApr;
                        f.obsFirSol = obs;
                        _sis.mergeFirmas(f);
                        //3.Si hay presupuesto , modificar Saldo y Estim
                        if (TienePresupuesto)
                        {
                            _mov.updateEstMovPres(sol, idNewEst);
                            double monto = 0;
                            if (idMon == ConstantesGlobales.monedaDol)
                            {
                                monto = monNeto;
                            }
                            else
                            {
                                monto = monNeto / valTC;
                            }
                            double actualizado = monto;
                            _mov.updateSaldoPres(idPres, actualizado);
                            _mov.updateSaldoPresEstim(idPres, actualizado);
                        }
                        //4.Si hay actividad, modificar 
                        if (TieneActividad)
                        {
                            string msn = "";
                            double monto = 0;
                            if (idMon == ConstantesGlobales.monedaDol)
                            {
                                monto = monNeto;
                            }
                            else
                            {
                                monto = monNeto / valTC;
                            }
                            double actualizado = monto;
                            _esti.modificarDetEstimSalReal(actualizado, idAct, idActGas, out msn);
                        };
                        //******************************************
                    }
                    else
                    {
                        correcto = false;
                    }
                    break;
                //ESTADO APROBADO
                case "9":
                    if (idOldEst==ConstantesGlobales.estadoAtendido)
                    {
                        //1.Actualizar estado de la solicitud
                        _sis.updateEstadoSol(sol, "", idNewEst);
                        //2.Agregar firma del cambio de estado
                        FirmasSoliGastoModels f = new FirmasSoliGastoModels();
                        f.idSolGas = sol;
                        f.idAcc = SessionPersister.UserId;
                        f.idEst = idNewEst;
                        f.usuCrea = SessionPersister.Username;
                        f.usufchCrea = DateTime.Now;
                        f.idNapro = SessionPersister.NivApr;
                        f.obsFirSol = obs;
                        _sis.mergeFirmas(f);
                        if (TienePresupuesto)
                        {
                            _mov.updateEstMovPres(sol, idNewEst);
                        }
                     }
                    else
                    {
                        correcto = false;
                    }
                    break;
            }
            return Json(correcto, JsonRequestBehavior.AllowGet);
        }
        //SOLGASMKT_REPORTE 000238
        [CustomAuthorize(Roles = "000003,000283")]
        [HttpGet]
        public ActionResult Reporte_0(string menuArea, string menuVista, string fchEveSolGasI = "", string fchEveSolGasF = "")
        {
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();
            //--
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            //--
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
            Parametros p = new Parametros();
            var parametro = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).ToList();
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp }).ToList();
            var tipGas = _pre.obtenerPresupuesto().Where(x => x.idTipoPres == ConstantesGlobales.plan_Mark && x.idTipGas != null).Select(x => new { idTipGas = x.idTipGas, nomTipGas = x.tipogasto.nomTipGas }).Distinct().ToList();
            var conGas = _pre.obtenerPresupuesto().Where(x => x.idTipoPres == ConstantesGlobales.plan_Mark && x.idConGas != null).Select(x => new { idCon = x.idConGas, nombre = x.concepto.nomConGas }).Distinct().ToList();
            //-----
            ViewBag.responsable = new SelectList(result.Select(x => new { idAcc = x.value, nombre = x.nomComEmp }), "idAcc", "nombre");
            ViewBag.tipConcepto = new SelectList(tipGas.Select(x => new { idTipGas = x.idTipGas, nombre = x.nomTipGas }), "idTipGas", "nombre");
            ViewBag.concepto = new SelectList(conGas.Select(x => new { idCon = x.idCon, nombre = x.nombre }), "idCon", "nombre");
            //-----
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //----
            return View();
        }
        public string ReporteGeneral(string idAcc,string idTipGas, string idCon, string fchEveSolGasI, string fchEveSolGasF)
        {
            string rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "Export/Reportes/generarReporteGastos1.xls";
            try { 
                //titulos
                int fil = 1;
                int col = 0;
                SLFont font;
                SLRstType rst;
                SLStyle style;
                SLThemeSettings theme = new SLThemeSettings();

                //theme.Light1Color = System.Drawing.Color.White;
                SLDocument sl = new SLDocument();

                //Page Setting
                SLPageSettings ps = new SLPageSettings();
                ps.Orientation = OrientationValues.Landscape;
                ps.View = SheetViewValues.PageBreakPreview;
                // 120% of normal page size
                ps.ScalePage(80);
                ps.ZoomScale = 80;
                ps.PaperSize = SLPaperSizeValues.A4Paper;
                ps.HorizontalDpi = 300;
                ps.VerticalDpi = 300;

                ps.PrintGridLines = false;
                ps.BlackAndWhite = false;
                ps.Draft = false;
                ps.PrintHeadings = false;

                ps.CellComments = CellCommentsValues.AtEnd;
                ps.Errors = PrintErrorValues.NA;
                ps.PageOrder = PageOrderValues.OverThenDown;

                //*********************
                //----------------------
                //------------------------------------------------------------------------
                DateTime p = DateTime.Parse(fchEveSolGasI); //desde
                DateTime a = DateTime.Parse(fchEveSolGasF).AddHours(23).AddMinutes(59);//hasta
                //------------------------------------------------------------------------
                //************************
                var presupuestos = _pre.obtenerPresupuesto().Where(x => (x.idAccRes == (idAcc == "" ? x.idAccRes : idAcc)) && ((x.idTipGas == (idTipGas == "" ? x.idTipGas : idTipGas)) || (x.idConGas == (idCon == "" ? x.idConGas : idCon))) && ((x.fchIniVigencia >= p) && (x.fchFinVigencia <= a)) && (x.idTipoPres == ConstantesGlobales.plan_Mark) && (x.idEst != ConstantesGlobales.estadoInactivo)).ToList();
                //leer presupuestos inicio
                col = 1;
                //************************
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Major, 20);
                font.Underline = UnderlineValues.Double;
                rst = new SLRstType();
                rst.AppendText("Reporte de Gastos y Presupuesto de Marketing", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                style.Alignment.Vertical = VerticalAlignmentValues.Center;
                sl.SetCellStyle(fil, col, style);
                sl.MergeWorksheetCells(1, 1, 1, 11);
                sl.SetRowHeight(1, 1, 40);
                //-----------------------------------------
                fil = fil + 2;
                //-----------------------------------------
                foreach (var item in presupuestos)
                {
                    if (item.movimiento.Count != 0)
                    {
                        //inicio for cabecera
                        //******************************
                        //(0)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);
                        rst = new SLRstType();
                        rst.AppendText("ID de Presupuesto", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetColumnWidth(fil, col, 30);
                        col = 2;
                        sl.SetCellValue(fil, col, item.idPres);
                        //(1)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);
                        rst = new SLRstType();
                        rst.AppendText("Tipo y Conc. de Gasto", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetColumnWidth(fil, col, 30);
                        col = 2;
                        if(item.concepto != null && item.tipogasto != null)
                        { 
                            sl.SetCellValue(fil, col, item.tipogasto.nomTipGas + "-" + item.concepto.nomConGas); 
                        }
                        if(item.tipogasto != null && item.concepto == null)
                        { 
                            sl.SetCellValue(fil, col, item.tipogasto.nomTipGas + "- CUALQUIERA" ); 
                        }
                        if(item.concepto == null && item.concepto != null)
                        {
                            var cong = item.concepto;
                            var tipg = _tgas.obtenerItem(cong.idTipGas);
                            sl.SetCellValue(fil, col,  tipg.nomTipGas + "-" + cong.nomConGas); 
                        }
                        //(2)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);
                        rst = new SLRstType();
                        rst.AppendText("Especialidad", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        sl.SetCellValue(fil, col, item.especialidad.nomEsp);
                        //(3)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Fecha Inicio", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        sl.SetCellValue(fil, col, item.fchIniVigencia.ToString("dd-MM-yyyy"));
                        //(3)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Fecha Final", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        sl.SetCellValue(fil, col, item.fchFinVigencia.ToString("dd-MM-yyyy"));
                        //(4)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Moneda", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        style = sl.CreateStyle();
                        style.FormatCode = "#,##0.00";
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                        sl.SetCellValue(fil, col, item.moneda.nomMon);
                        sl.SetCellStyle(fil, col, style);
                        //(5)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Inversion" , font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        style = sl.CreateStyle();
                        style.FormatCode = "#,##0.00";
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                        double inversion = item.Monto;
                        sl.SetCellValue(fil, col,  item.Monto);
                        sl.SetCellStyle(fil, col, style);
                        //(6)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Saldo", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        style = sl.CreateStyle();
                        style.FormatCode = "#,##0.00";
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                        double saldo = item.Saldo;
                        sl.SetCellValue(fil, col, item.Saldo);
                        sl.SetCellStyle(fil, col, style);
                        //(7)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Consumo", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        style = sl.CreateStyle();
                        style.FormatCode = "#,##0.00";
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                        double consumo = 0;
                        int fil_consumo = fil;
                        int col_consumo = col;
                        sl.SetCellValue(fil, col, consumo);
                        sl.SetCellStyle(fil, col, style);
                        //(8)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Diferencia TC", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        style = sl.CreateStyle();
                        style.FormatCode = "#,##0.00";
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                        double diferencia = 0;
                        sl.SetCellValue(fil, col, diferencia);
                        sl.SetCellStyle(fil, col, style);
                        //******************************
                        //Ahora cabecera de detalle 
                        //******************************
                        fil = fil + 1;
                        col = 2;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("N° Solicitud", font);
                        style = sl.CreateStyle();
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetCellStyle(fil, col, style);

                        col = 3;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Titulo", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.AutoFitColumn(col);

                        col = 4;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Zona", font);
                        style = sl.CreateStyle();
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetCellStyle(fil, col, style);

                        col = 5;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Familia de Productos", font);
                        style = sl.CreateStyle();
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetCellStyle(fil, col, style);

                        col = 6;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        //rst.AppendText("Fecha de Eve.", font);
                        rst.AppendText("Mes de Eve.", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());

                        col = 8;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Mon.", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.AutoFitColumn(col);

                        col = 8;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Soles", font);
                        style = sl.CreateStyle();
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetCellStyle(fil, col, style);
                        sl.AutoFitColumn(col);

                        col = 9;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Mon.", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.AutoFitColumn(col);

                        col = 10;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Dolares", font);
                        style = sl.CreateStyle();
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetCellStyle(fil, col, style);
                        sl.AutoFitColumn(col);

                        col = 11;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Estado", font);
                        style = sl.CreateStyle();
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetCellStyle(fil, col, style);
                        sl.AutoFitColumn(col);

                        col = 12;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Observación", font);
                        style = sl.CreateStyle();
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Left;
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetCellStyle(fil, col, style);
                        sl.AutoFitColumn(col);

                        //*********-------------------****************
                        SLStyle style2 = sl.CreateStyle();
                        style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Medium;
                        style2.Border.BottomBorder.Color = System.Drawing.Color.Black;
                        style2.Border.TopBorder.BorderStyle = BorderStyleValues.Medium;
                        style2.Border.TopBorder.Color = System.Drawing.Color.Black;
                        // --------------------------------------
                        //style2.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Blue, System.Drawing.Color.DarkSalmon);
                        sl.SetCellStyle(fil, 2, fil, 11, style2);
                        //*********-------------------****************
                        fil = fil + 1;
                        //----------------------------
                        var gastos = _mov.obtenerMovxPres(item.idPres);
                        //----------------------------
                        double sumaD = 0;
                        double sumaS = 0;
                        //----------------------------
                        foreach (var det in gastos)
                        {
                            bool procede = true;
                            bool valid = det.solGasto.liquidacion == null ? false: true;
                            //**************************************************
                            if (valid)
                            {
                                string devolucionT = det.solGasto.liquidacion.idEst;
                                if (devolucionT == ConstantesGlobales.estadoDTotal)
                                {
                                    procede = false;
                                }
                                else
                                {
                                    procede = true;
                                }
                            }
                            else
                            {
                                procede = true;
                            }
                            if (procede)
                            {  
                                //**************************************************
                                //asignar valores a las variables
                                fil = fil + 1;
                                //---------------------------------------------------
                                col = 2;
                                sl.SetCellValue(fil, col, det.idSolGas);
                                //---------------------------------------------------
                                col = 3;
                                sl.SetCellValue(fil, col, det.solGasto.titSolGas);

                                //---------------------------------------------------
                                col = 4;
                                //--
                                var zona = _zon.obtenerItem(det.solGasto.idZon);
                                sl.SetCellValue(fil, col, zona.nomZon);
                                style = sl.CreateStyle();
                                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                                sl.SetCellStyle(fil, col, style);

                                //---------------------------------------------------
                                col = 5;
                                string famproductos= "";
                                //var counter = 0;
                                foreach (var pro in det.solGasto.dFam)
                                {
                                    var prod = _prod.obtenerItem(pro.idFamRoe);
                                    if (famproductos == "")
                                    { famproductos = prod.nomFamRoe; }
                                    else { famproductos = famproductos + "," + prod.nomFamRoe; }
                                }
                                //--
                                sl.SetCellValue(fil, col, famproductos);
                                style = sl.CreateStyle();
                                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                                sl.SetCellStyle(fil, col, style);
                                //--
                                //fil = fil - counter;
                                //---------------------------------------------------
                                col = 6;
                                /*sl.SetCellValue(fil, col, det.solGasto.fchEveSolGas.ToString("dd-MM-yyyy"));*/
                                sl.SetCellValue(fil, col, det.solGasto.fchEveSolGas.ToString("MMMM"));
                                style = sl.CreateStyle();
                                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                                sl.SetCellStyle(fil, col, style);
                                //---------------------------------------------------
                                //CONVERSIONES --------------------------------------
                                //---------------------------------------------------
                                double liqu = 0;
                                string estad = "";
                                if(det.solGasto.liquidacion!=null)
                                {
                                    liqu = det.solGasto.liquidacion.liqValRea;
                                    estad = det.solGasto.liquidacion.idEst;
                                    if (estad == ConstantesGlobales.estadoDParcial || estad == ConstantesGlobales.estadoDTotal)
                                    {
                                        liqu = -liqu;
                                    }
                                }
                                //---------------------------------------------------
                                // La liquidacion es de toda la solicitud , si la solicitud tiene varios presupuestos ver la division
                                // ver programacion a varios 
                                //---------------------------------------------------
                                //double neto = (double)det.solGasto.monNeto + liqu;
                                double neto = (double)det.monSolGas;
                                double tc = det.solGasto.valtipCam;
                                double conversionD = 0;
                                double conversionS = 0;
                                if (det.solGasto.idMon == "1")
                                {
                                    conversionD = Math.Round(neto / tc, 2);
                                    conversionS = Math.Round(neto);
                                }
                                else
                                {
                                    conversionD = Math.Round(neto);
                                    conversionS = Math.Round(neto * tc, 2);
                                }
                                //---------------------------------------------------

                                //---------------------------------------------------
                                //SOLES 
                                //---------------------------------------------------
                                col = 7;
                                sl.SetCellValue(fil, col, "S/.");

                                col = 8;
                                style = sl.CreateStyle();
                                style.FormatCode = "#,##0.00";
                                sl.SetCellValue(fil, col, conversionS);
                                sl.SetCellStyle(fil, col, style);

                                //---------------------------------------------------
                                //DOLARES
                                //---------------------------------------------------
                                col = 9;
                                sl.SetCellValue(fil, col, "$");

                                col = 10;
                                style = sl.CreateStyle();
                                style.FormatCode = "#,##0.00";
                                sl.SetCellValue(fil, col, conversionD);
                                sl.SetCellStyle(fil, col, style);

                                //---------------------------------------------------
                                //ESTADO
                                col = 11;
                                sl.SetCellValue(fil, col, det.estado.nomEst);
                                style = sl.CreateStyle();
                                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                                sl.SetCellStyle(fil, col, style);
                                //---------------------------------------------------

                                //OBSERVACION
                                //---------------------------------------------------
                                col = 12;
                                sl.SetCellValue(fil, col, det.solGasto.obsSolGas);
                                style = sl.CreateStyle();
                                style.Alignment.Horizontal = HorizontalAlignmentValues.Left;
                                sl.SetCellStyle(fil, col, style);
                                //---------------------------------------------------

                                //sumamos todas las variables 
                                sumaD = sumaD + conversionD;
                                sumaS = sumaS + conversionS;
                                //fil = fil + counter - 1;
                            }
                        }
                        //------------*************************----------------------
                        consumo = sumaD;
                        sl.SetCellValue(fil_consumo, col_consumo, consumo);
                        diferencia = (inversion - saldo) - consumo;
                        sl.SetCellValue(fil_consumo + 1, col_consumo, diferencia);
                        //------------*************************-----------------------
                        //fin detalle
                        fil = fil + 1;

                        col = 3;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Total", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetColumnWidth(fil, col, 35);
                        col = 6;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("S/.", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 7;
                        style = sl.CreateStyle();
                        style.SetFont(FontSchemeValues.Minor, 12);
                        style.Font.Bold = true;
                        style.FormatCode = "#,##0.00";
                        sl.SetCellValue(fil, col, sumaS);
                        sl.SetCellStyle(fil, col, style);
                        sl.SetColumnWidth(fil, col, 15);
                        col = 8;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("$", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 9;
                        style = sl.CreateStyle();
                        style.SetFont(FontSchemeValues.Minor, 12);
                        style.Font.Bold = true;
                        style.FormatCode = "#,##0.00";
                        sl.SetCellValue(fil, col, sumaD);
                        sl.SetCellStyle(fil, col, style);
                        sl.SetColumnWidth(fil, col, 15);
                        // --------------------------------------
                        SLStyle style3 = sl.CreateStyle();
                        style3.Border.BottomBorder.BorderStyle = BorderStyleValues.Double;
                        style3.Border.BottomBorder.Color = System.Drawing.Color.Black;
                        style3.Border.TopBorder.BorderStyle = BorderStyleValues.Double;
                        style3.Border.TopBorder.Color = System.Drawing.Color.Black;
                        //style3.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Blue, System.Drawing.Color.DarkSalmon);
                        sl.SetCellStyle(fil, 2, fil, 11, style3);
                        //------------
                        fil = fil + 2;

                    }
                }
                //-----------------------------------------
                SLStyle styleF = sl.CreateStyle();
                styleF.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                styleF.Border.RemoveAllBorders();
                sl.SetCellStyle(1, 1, fil, 12, styleF);
                sl.SetPageSettings(ps);
                sl.AutoFitColumn(4, 12);
                //-----------------------------------------
                sl.SaveAs(rutaArchivoCompleta);
                //-----------------------------------------
            }
            catch (Exception ex)
            {
                SLFont font;
                SLRstType rst;
                SLStyle style;
                SLDocument sl = new SLDocument();
                //Page Setting
                SLPageSettings ps = new SLPageSettings();
                ps.Orientation = OrientationValues.Landscape;
                ps.View = SheetViewValues.PageBreakPreview;
                // 120% of normal page size
                ps.ScalePage(56);
                ps.ZoomScale = 80;
                ps.PaperSize = SLPaperSizeValues.A4Paper;
                ps.HorizontalDpi = 300;
                ps.VerticalDpi = 300;

                ps.PrintGridLines = false;
                ps.BlackAndWhite = false;
                ps.Draft = false;
                ps.PrintHeadings = false;

                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Major, 20);
                font.Underline = UnderlineValues.Double;
                rst = new SLRstType();
                rst.AppendText("No se genero el reporte ...", font);
                sl.SetCellValue(1, 1, rst.ToInlineString());
                //-----------------------------------------
                SLStyle styleF = sl.CreateStyle();
                styleF.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                styleF.Border.RemoveAllBorders();
                sl.SetCellStyle(1, 1, 9, 9, styleF);
                sl.SetPageSettings(ps);
                sl.AutoFitColumn(4, 9);
                //-----------------------------------------
                sl.SaveAs(rutaArchivoCompleta);
                //-----------------------------------------
            }
            return rutaArchivoCompleta;
        }
        public string ReporteResumen(string idAcc, string idTipGas,string idCon, string fchEveSolGasI, string fchEveSolGasF)
        {
            string rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "Export/Reportes/generarReporteGastos2.xls";
            try
            {
                //titulos
                int fil = 1;
                int col = 0;
                SLFont font;
                SLRstType rst;
                SLStyle style;
                SLThemeSettings theme = new SLThemeSettings();

                //theme.Light1Color = System.Drawing.Color.White;
                SLDocument sl = new SLDocument();

                //Page Setting
                SLPageSettings ps = new SLPageSettings();
                ps.Orientation = OrientationValues.Portrait;
                //ps.View = SheetViewValues.PageBreakPreview;
                // 120% of normal page size
                ps.ScalePage(120);
                ps.ZoomScale = 100;
                ps.PaperSize = SLPaperSizeValues.A4Paper;
                ps.HorizontalDpi = 300;
                ps.VerticalDpi = 300;

                //ps.PrintGridLines = false;
                ps.BlackAndWhite = false;
                ps.Draft = false;
                ps.PrintHeadings = false;

                ps.CellComments = CellCommentsValues.AtEnd;
                ps.Errors = PrintErrorValues.NA;
                ps.PageOrder = PageOrderValues.OverThenDown;

                //*********************
                //----------------------
                //------------------------------------------------------------------------
                DateTime p = DateTime.Parse(fchEveSolGasI); //desde
                DateTime a = DateTime.Parse(fchEveSolGasF).AddHours(23).AddMinutes(59);//hasta
                //------------------------------------------------------------------------
                //----------------------
                //*********************
                //var listDeGastos = _sis.obtenerSolicitudes().Where(x => x.idAccRes == (idAcc == "" ? x.idAccRes: idAcc ) && (x.idEst != ConstantesGlobales.estadoAnulado || x.idEst != ConstantesGlobales.estadoRechazado || x.idEst != ConstantesGlobales.estadoDTotal)).ToList();
                var presupuestos = _pre.obtenerPresupuesto().Where(x => x.idAccRes == (idAcc == "" ? x.idAccRes : idAcc) && ((x.idTipGas == (idTipGas == "" ? x.idTipGas : idTipGas)) || (x.idConGas == (idCon == "" ? x.idConGas : idCon))) && ((x.fchIniVigencia >= p) && (x.fchFinVigencia <= a)) && (x.idTipoPres == ConstantesGlobales.plan_Mark) && (x.idEst != ConstantesGlobales.estadoInactivo)).ToList();
                var usuPres = presupuestos.Select(x => new { idResp = x.idAccRes, nomResp = x.responsable.empleado.nomComEmp }).Distinct().ToList();
                //leer presupuestos inicio
                col = 1;

                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Major, 20);
                font.Underline = UnderlineValues.Double;
                rst = new SLRstType();
                rst.AppendText("Reporte  Resumen de Gastos y Presupuesto - Marketing", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                style.Alignment.Vertical = VerticalAlignmentValues.Center;
                sl.SetCellStyle(fil, col, style);
                sl.MergeWorksheetCells(1, 1, 1, 8);
                sl.SetRowHeight(1, 1, 40);
                //-----------------------------------------
                fil = fil + 2;
                //-----------------------------------------
                foreach (var usu in usuPres)
                {
                    //inicio Divisoria
                    //******************************
                    //(0.0)-------------------------
                    fil = fil + 1;
                    col = 1;
                    font = new SLFont();
                    font.Bold = true;//negrita
                    font.SetFont(FontSchemeValues.Minor, 14);
                    rst = new SLRstType();
                    rst.AppendText("RESPONSABLE : ", font);
                    sl.SetCellValue(fil, col, rst.ToInlineString());
                    sl.SetColumnWidth(fil, col, 20);

                    col = 2;
                    style = sl.CreateStyle();
                    style.Font.Bold = true;
                    style.SetFontColor(System.Drawing.Color.Blue);
                    sl.SetCellValue(fil, col, usu.nomResp);
                    sl.SetCellStyle(fil, col, style);

                    //-------------------------------
                    fil = fil + 1;
                    //-------------------------------
                    var conceptos = presupuestos.Where(x => x.idAccRes == usu.idResp && ((x.idTipGas == (idTipGas == "" ? x.idTipGas : idTipGas)) || (x.idConGas == (idCon == "" ? x.idConGas : idCon)))).Select(x => new { idCon = (x.idTipGas == null ? "00" : x.idTipGas) + "|" + (x.idConGas == null ? "00" : x.idConGas), nomCon = (x.idTipGas == null ? x.concepto.tipoGasto.nomTipGas : x.tipogasto.nomTipGas) + "-" + (x.idConGas == null ? "CUALQUIERA" : x.concepto.nomConGas) }).Distinct().ToList();
                    //-------------------------------
                    foreach (var con in conceptos)
                    {
                        //******************************
                        fil = fil + 1;
                        //(1.0)-------------------------

                        col = 2;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("TIPO Y CONCEPTO : ", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetColumnWidth(fil, col, 15);

                        col = 3;
                        style = sl.CreateStyle();
                        style.Font.Bold = true;
                        style.Font.Underline = UnderlineValues.Double;
                        style.Font.FontSize = 14;
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        sl.SetCellValue(fil, col, con.nomCon);
                        sl.SetCellStyle(fil, col, style);

                        //-------------------------------
                        //Cabecera de la lista
                        //******************************
                        fil = fil + 1;
                        //(1.0)-------------------------

                        col = 2;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("N° Pres.", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        //(1.1)-------------------------

                        col = 3;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Especialidad", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        //(1.2)-------------------------

                        col = 4;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Fec.Ini. ", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        //(1.5)-------------------------

                        col = 5;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Fec.Fin ", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        //(1.6)-------------------------

                        col = 6;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Moneda ", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        //(1.7)-------------------------

                        col = 7;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        style = sl.CreateStyle();
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                        rst = new SLRstType();
                        rst.AppendText("Inversion ", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetCellStyle(fil, col, style);
                        //(1.8)-------------------------

                        col = 8;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        style = sl.CreateStyle();
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                        rst = new SLRstType();
                        rst.AppendText("Saldo ", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetCellStyle(fil, col, style);

                        //******************************
                        string idConTip = con.idCon;
                        var id = idConTip.Split('|');
                        var listaPres = presupuestos.Where(x => x.idConGas == (id[1] == "00" ?  null : id[1]) && x.idTipGas==(id[0] == "00" ? null : id[0]) && (x.idAccRes == usu.idResp)).ToList();
                        //******************************
                        double sumaT = 0;
                        double sumaS = 0;
                        foreach (var pre in listaPres)
                        {
                            //asignar valores a las variables
                            fil = fil + 1;
                            //---------------------------------------------------
                            col = 2;
                            sl.SetCellValue(fil, col, pre.idPres);
                            //---------------------------------------------------
                            col = 3;
                            sl.SetCellValue(fil, col, pre.especialidad.nomEsp);
                            //---------------------------------------------------
                            col = 4;
                            sl.SetCellValue(fil, col, pre.fchIniVigencia.ToString("dd-MM-yyyy"));
                            //---------------------------------------------------
                            col = 5;
                            sl.SetCellValue(fil, col, pre.fchFinVigencia.ToString("dd-MM-yyyy"));
                            //---------------------------------------------------
                            col = 6;
                            sl.SetCellValue(fil, col, pre.moneda.simbMon);
                            //---------------------------------------------------
                            col = 7;
                            style = sl.CreateStyle();
                            style.FormatCode = "#,##0.00";
                            style.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                            sl.SetCellValue(fil, col, pre.Monto);
                            sl.SetCellStyle(fil, col, style);
                            //---------------------------------------------------
                            col = 8;
                            style = sl.CreateStyle();
                            style.FormatCode = "#,##0.00";
                            style.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                            sl.SetCellValue(fil, col, pre.Saldo);
                            sl.SetCellStyle(fil, col, style);
                            //---------------------------------------------------
                            //---- sumar variables -------
                            double total = pre.Monto;
                            double saldo = pre.Saldo;
                            sumaT = sumaT + total;
                            sumaS = sumaS + saldo;
                            //----------------------------
                        }
                        fil = fil + 1;
                        //-----***************************************************-----------
                        col = 3;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Total", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetColumnWidth(fil, col, 35);
                        col = 6;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("$", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetColumnWidth(fil, col, 10);
                        col = 7;
                        style = sl.CreateStyle();
                        style.SetFont(FontSchemeValues.Minor, 12);
                        style.Font.Bold = true;
                        style.FormatCode = "#,##0.00";
                        sl.SetCellValue(fil, col, sumaT);
                        sl.SetCellStyle(fil, col, style);
                        sl.SetColumnWidth(fil, col, 15);
                        col = 8;
                        style = sl.CreateStyle();
                        style.SetFont(FontSchemeValues.Minor, 12);
                        style.Font.Bold = true;
                        style.FormatCode = "#,##0.00";
                        sl.SetCellValue(fil, col, sumaS);
                        sl.SetCellStyle(fil, col, style);
                        sl.SetColumnWidth(fil, col, 15);
                        //-----***************************************************-----------
                        // --------------------------------------
                        SLStyle style3 = sl.CreateStyle();
                        style3.Border.BottomBorder.BorderStyle = BorderStyleValues.Double;
                        style3.Border.BottomBorder.Color = System.Drawing.Color.Black;
                        style3.Border.TopBorder.BorderStyle = BorderStyleValues.Double;
                        style3.Border.TopBorder.Color = System.Drawing.Color.Black;
                        //style3.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Blue, System.Drawing.Color.DarkSalmon);
                        sl.SetCellStyle(fil, 2, fil, 8, style3);
                        //------------
                        fil = fil + 2;
                    }
                }
                //-----------------------------------------
                SLStyle styleF = sl.CreateStyle();
                styleF.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                styleF.Border.RemoveAllBorders();
                sl.SetCellStyle(1, 1, fil, 8, styleF);
                sl.SetPageSettings(ps);
                sl.AutoFitColumn(3, 6);
                //-----------------------------------------
                sl.SaveAs(rutaArchivoCompleta);
                //-----------------------------------------
            }
            catch (Exception ex)
            {
            }
            return rutaArchivoCompleta;
        }
        public FileResult Reporte_1(string idRep, string idAcc, string idTipGas ,string idCon, string fchEveSolGasI, string fchEveSolGasF)
        {
            //ruta donde esta la aplicacion y se guarda en la carpte  reportes
            string rutaArchivoCompleta = "";
            try
            {
                switch (idRep)
                {
                    case "1":
                        rutaArchivoCompleta = ReporteGeneral(idAcc,idTipGas,idCon,  fchEveSolGasI,  fchEveSolGasF);
                        break;
                    case "2":
                        rutaArchivoCompleta = ReporteResumen(idAcc, idTipGas, idCon,  fchEveSolGasI,  fchEveSolGasF);
                        break;
                }
                return File(rutaArchivoCompleta, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                rutaArchivoCompleta =  AppDomain.CurrentDomain.BaseDirectory + "Export/Reportes/reporteVacio.xls";
                return File(rutaArchivoCompleta, "application/vnd.ms-excel");
            }
        }
        public FileResult ExportSolicitudes(string search)
        {
            var solicitudesExp = _sis.obtenerSolicitudesaExportar(search, ConstantesGlobales.mod_marketing);
            string path = "~/Export/Solicitudes";
            bool exists = Directory.Exists(Server.MapPath(path));
            if (!exists) Directory.CreateDirectory(Server.MapPath(path));

            using (SLDocument sl = new SLDocument())
            {

                path = path + "/SOLICITUDES EXPORTADAS.xls";
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
                rst.AppendText("Solicitudes", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Left;
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
                sl.SetColumnWidth(col, 25);


                col = 2;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Titulo", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 35);

                col = 3;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("F.Evento", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 25);

                col = 4;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Estado", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 12);

                col = 5;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Tipo de Gasto", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 5);

                col = 6;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Monto", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 5);

                col = 7;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Moneda", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 10);

                col = 8;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("T. de Cambio", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 10);

                col = 9;
                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Minor, 14);
                rst = new SLRstType();
                rst.AppendText("Observación", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                sl.SetColumnWidth(col, 10);

                // --------------------------------------
                //Rayas
                SLStyle style2 = sl.CreateStyle();
                style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.BottomBorder.Color = System.Drawing.Color.Black;
                style2.Border.TopBorder.BorderStyle = BorderStyleValues.Medium;
                style2.Border.TopBorder.Color = System.Drawing.Color.Black;
                sl.SetCellStyle(fil, 1, fil, 18, style2);
                // --------------------------------------

                fil = fil + 1;

                foreach (var s in solicitudesExp)
                {
                    //Código
                    col = 1;
                    sl.SetCellValue(fil, col, s.idSolGas);
                    //Titulo
                    col = 2;
                    sl.SetCellValue(fil, col, s.titSolGas);
                    //F.Evento
                    col = 3;
                    sl.SetCellValue(fil, col, s.fchEveSolGas);
                    //Estado
                    col = 4;
                    sl.SetCellValue(fil, col, s.estado.nomEst);
                    //Tipo de Gasto
                    col = 5;
                    sl.SetCellValue(fil, col, s.concepto.nomConGas);
                    //Monto
                    col = 6;
                    sl.SetCellValue(fil, col, s.monSolGas);
                    //Moneda
                    col = 7;
                    sl.SetCellValue(fil, col, s.moneda.nomMon);
                    //T. de Cambio
                    col = 8;
                    sl.SetCellValue(fil, col, s.valtipCam);
                    //Observacion
                    col = 9;
                    sl.SetCellValue(fil, col, s.obsSolGas);

                    fil = fil + 1;

                }
                sl.SaveAs(Server.MapPath(path));
                return File(Server.MapPath(path), "application/vnd.ms-excel");
            }
        }
        
    }

}