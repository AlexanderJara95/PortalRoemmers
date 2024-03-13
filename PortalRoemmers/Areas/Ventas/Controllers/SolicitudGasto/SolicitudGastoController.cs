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
using iTextSharp.text;
using PortalRoemmers.Areas.Sistemas.Services.Presupuesto;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using PortalRoemmers.Areas.Marketing.Services.Estimacion;
using PortalRoemmers.Areas.Sistemas.Services.Producto;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;

namespace PortalRoemmers.Areas.Ventas.Controllers.SolicitudGasto
{
    public class SolicitudGastoController : Controller
    {
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
        private NivelAproRepositorio _niv;
        private EstadoRepositorio _est;
        private PresupuestoRepositorio _pre;
        private MovimientoPresRepositorio _mov;
        private LiquidaRepositorio _liq;
        private EstimacionRepositorio _esti;
        private ProductoRepositorio _product;
        private TipoGasto_UsuRepositorio _tipU;
        private TipoGastoRepositorio _tipGas;
        Ennumerador enu = new Ennumerador();

        public SolicitudGastoController()
        {
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
            _niv= new NivelAproRepositorio();
            _est = new EstadoRepositorio();
            _pre = new PresupuestoRepositorio();
            _mov = new MovimientoPresRepositorio();
            _liq = new LiquidaRepositorio();
            _esti = new EstimacionRepositorio();
            _product = new ProductoRepositorio();
            _tipGas = new TipoGastoRepositorio();
            _tipU = new TipoGasto_UsuRepositorio();
        }
        [CustomAuthorize(Roles = "000003,000192")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "", string fchEveSolGasI = "", string fchEveSolGasF = "")
        {
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
            var model = _sis.obtenerTodos(pagina, search,ConstantesGlobales.mod_ventas, inicio.ToString(), fin.ToString());
            //-----------------------------
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000193")]
        [HttpGet]
        public ActionResult Registrar()
        {
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];

            string familiaD="";
            string medicoD = "";
            string responsableD = emple.idEmp + ";" + emple.apePatEmp + " " + emple.apeMatEmp + " " + emple.nom1Emp + " " + emple.nom2Emp+";"+"";
            SolicitudGastoModels model = new SolicitudGastoModels();
            model.idAccSol = SessionPersister.UserId;
            model.idAccRes = SessionPersister.UserId;
            model.idMon = ConstantesGlobales.monedaSol;
            try {
              model.valtipCam =double.Parse(_tcam.obtenerItem(DateTime.Today).monTCVenta.ToString());
            }
            catch (Exception e)
            {
                TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe ingresar el tipo de cambio ..." + "</div>";
            }
            cargarCombos(model);
            creaTablas(familiaD, medicoD, responsableD);
            var actual = DateTime.Today.ToString("dd/MM/yyyy");
            ViewBag.fecha = actual;

            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(SolicitudGastoModels model, string familiaD, string medicoD, string responsableD)
        {
            model.modSolGas = ConstantesGlobales.mod_ventas;
            //--
            double monto = 0;
            monto = (double)model.monSolGas;
            string areaterap = "";
            string familia = "";
            //--
            try
            {
                model.valtipCam = double.Parse(_tcam.obtenerItem(DateTime.Today).monTCVenta.ToString());
            }
            catch (Exception e) { e.Message.ToString(); }
            if (ModelState.IsValid)
            {
                string tabla = "tb_SolGastos";
                int idc = enu.buscarTabla(tabla);
                model.idSolGas = idc.ToString("D7");

                if (!model.procede)//tiene presupuesto
                {
                    if (familiaD=="") {
                        ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                        cargarCombos(model);
                        creaTablas(familiaD, medicoD, responsableD);
                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe cargar familia de producto" + "</div>";
                        return View(model);
                    }
                    else if (medicoD=="")
                    {
                        ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                        cargarCombos(model);
                        creaTablas(familiaD, medicoD, responsableD);
                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe cargar Médicos" + "</div>";
                        return View(model);
                    }
                    else
                    {
                         areaterap = crearDetalleAT(familiaD, monto);
                         familia = crearDetalleFamiliaD(familiaD);

                        if (model.idPres!=null)
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
                            if (tp >= model.monSolGas)
                            {//realizo toda la logica para descontar
                                if (guardarSolicitudDetalles(model, familia, medicoD, responsableD, areaterap))
                                {
                                    Boolean okM = true;
                                    //si tengo presupuesto
                                    if (model.idPres == null)
                                    {
                                        ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                                        cargarCombos(model);
                                        creaTablas(familiaD, medicoD, responsableD);
                                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe seleccionar un Presupuesto" + "</div>";
                                        return View(model);
                                    }
                                    else
                                    {
                                        //valido que la suma presupuesto sea mayor al de la solicitud
                                        double ress = model.monSolGas;
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
                                                    okM = false;break;
                                                }
                                            }
                                            else
                                            {
                                                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error al guardar el movimiento" + "</div>";
                                                okM = false; break;
                                            }
                                            j++;
                                        } while (ress > 0);
                                    }

                                    if (okM)//todo salio bien al guardar detalle y firmas,mensaje final
                                    {
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
                                ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                                cargarCombos(model);
                                creaTablas(familiaD, medicoD, responsableD);
                                TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "No cuenta con suficiente saldo" + "</div>";
                                return View(model);
                            }
                        }
                        else
                        {
                            ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                            cargarCombos(model);
                            creaTablas(familiaD, medicoD, responsableD);
                            TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe elegir un presupuesto" + "</div>";
                            return View(model);
                        }
                    }
                }
                else
                {
                    if (familiaD == "")
                    {
                        ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                        cargarCombos(model);
                        creaTablas(familiaD, medicoD, responsableD);
                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe cargar familia de producto" + "</div>";
                        return View(model);
                    }
                    else if (medicoD == "")
                    {
                        ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
                        cargarCombos(model);
                        creaTablas(familiaD, medicoD, responsableD);
                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe cargar Médicos" + "</div>";
                        return View(model);
                    }
                    else
                    {

                        if (guardarSolicitudDetalles(model, familia, medicoD, responsableD,areaterap))
                        {
                            //DETALLES////////////////////////////////////////////////////////////////////////////////////////////
                            TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                        }
                        else
                        {
                            TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error en guardar detalle" + "</div>";
                        }
                    }
                }

                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            ViewBag.fecha = model.fchEveSolGas.ToShortDateString();
            cargarCombos(model);
            creaTablas(familiaD, medicoD, responsableD);
            return View(model);
        }
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000225")]
        public ActionResult Modificar(string id)
        {
            string familiaD = "";
            string medicoD = "";
            string responsableD = "";
            string areaD = "";
            var model = _sis.obtenerItem(id);

            //Logica Hidden
            var fdetalle = model.dFam.Select(x=>x.familia);
            if (fdetalle.Count()!=0)
            {
                foreach (var d in fdetalle)
                {
                    if (familiaD != "")
                    {
                        familiaD += "|";
                    }
                    familiaD += "0;" + d.idFamRoe + ";" + d.nomFamRoe + ";0;";
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
                    if (areaD != "")
                    {
                        areaD += "|";
                    }
                    areaD += d.idAreaTerap + ";" + d.numAreaTerap + ";" + d.desAreaTerap;
                }
            }

            model.idTipGas = model.concepto.idTipGas;
            model.usufchMod = DateTime.Now;
            model.usuMod = SessionPersister.Username;
            creaTablas(familiaD, medicoD, responsableD);
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
                    if (areaD != "")
                    {
                        areaD += "|";
                    }
                    areaD += d.idAreaTerap + ";" + d.numAreaTerap + ";" + d.desAreaTerap;
                }
            }
            model.idTipGas = model.concepto.idTipGas;
            creaTablas(familiaD, medicoD, responsableD);
            crearTablaInterna(areaD);
            return View(model);
        }
        [HttpGet]
        public ActionResult VisualizarPartial(string id)
        {
            string familiaD = "";
            string medicoD = "";
            string responsableD = "";
            string areaTerap = "";
            var model = _sis.obtenerItem(id);

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
                    familiaD += "0;" + d.idFamRoe + ";" + d.nomFamRoe + ";0";
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

            creaTablas(familiaD, medicoD, responsableD);
            crearTablaInterna(areaTerap);
            return View(model);
        }
        //Aprobar
        [CustomAuthorize(Roles = "000003,000226")]
        [HttpGet]
        public ActionResult Aprobar(string menuArea, string menuVista)
        {
            //******************************************************************
            //Primero obtenemos el día actual
            DateTime date = DateTime.Now;
            //Asi obtenemos el primer dia del mes actual
            DateTime oPrimerDiaDelMes = new DateTime(date.Year, 1, 1);
            //------------------------------------------------------------------
            ViewBag.aprob = new SelectList(_est.obteneEstadoSegunAprob(), "idEst", "nomEst", ConstantesGlobales.estadoPreApro);
            //------------------------------------------------------------------
            var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
            var actual = DateTime.Today.ToString("dd/MM/yyyy");
            ViewBag.primero = primero;
            ViewBag.actual = actual;
            //******************************************************************
            Parametros p = new Parametros();
            var estados = p.Resultado(ConstantesGlobales.Com_Lis_Apr);
            string id = SessionPersister.UserId;
            //------------------------------------------------------------------
            var model = _sis.obtenerSolicitudesAprobar(id, estados, ConstantesGlobales.tipPag_tran, ConstantesGlobales.estadoPreApro,primero,actual);
            //------------------------------------------------------------------
            ViewBag.Aprobador = new SelectList(_asiapr.obtenerAprobadoresAsignados(SessionPersister.UserId).Select(x => new { x.idAccApro, nom = x.aprobador.empleado.nomComEmp }), "idAccApro", "nom");
            //------------------------------------------------------------------
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
            //------------------------------------------------------------------
        }
        //Aprobar
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Aprobar(string menuArea, string menuVista, string fchEveSolGasI, string fchEveSolGasF, string estado)
        {
            //*****************************************************
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
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = DateTime.Today.ToString("dd/MM/yyyy");
                inicio = DateTime.Parse(primero);
                fin = DateTime.Parse(actual);
            }
            //****************************************************
            //----------------------------------------------------
            ViewBag.aprob = new SelectList(_est.obteneEstadoSegunAprob(), "idEst", "nomEst", estado);
            //----------------------------------------------------
            IEnumerable<SolicitudGastoModels> model = null;
            //****************************************************
            Parametros p = new Parametros();
            var estados = p.Resultado(ConstantesGlobales.Com_Lis_Apr);
            string id = SessionPersister.UserId;
            //*****************************************************
            if (estado == ConstantesGlobales.estadoAprobado || estado == ConstantesGlobales.estadoRechazado)
            {
                model = _sis.obtenerSolicitudesAprobar(id, estados, ConstantesGlobales.tipPag_tran,estado, inicio.ToString(), fin.ToString());
            }
            else
            {
                model = _sis.obtenerSolicitudesAprobar(id, estados, ConstantesGlobales.tipPag_tran, ConstantesGlobales.estadoPreApro, inicio.ToString(), fin.ToString());
            }
            //------------------------------------------------------------------
            ViewBag.Aprobador = new SelectList(_asiapr.obtenerAprobadoresAsignados(SessionPersister.UserId).Select(x => new { x.idAccApro, nom = x.aprobador.empleado.nomComEmp }), "idAccApro", "nom");
            //------------------------------------------------------------------
            //*****************************************************
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //*****************************************************
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }
        //Aprobar Administrador
        [CustomAuthorize(Roles = "000003,000322")]
        [HttpGet]
        public ActionResult AprobarAdministrador(string menuArea, string menuVista, string estado = "9", string id = "175|3")
        {
            //*********************************************************************
            //Primero obtenemos el día actual
            DateTime date = DateTime.Now;
            //Asi obtenemos el primer dia del mes actual
            DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
            //*********************************************************************
            var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
            var actual = DateTime.Today.ToString("dd/MM/yyyy");
            ViewBag.primero = primero;
            ViewBag.actual = actual;
            //*********************************************************************
            ViewBag.estados = new SelectList(_est.obteneEstadosAprobarxAdm(), "idEst", "nomEst", ConstantesGlobales.estadoAprobado);
            ViewBag.aprobadores = new SelectList(_asiapr.obtenerTodosAproAsig().Select(x => new { idaprobadores = x.idAccApro + "|"+ x.aprobador.idNapro , nom = x.aprobador.empleado.nomComEmp }).Distinct(), "idaprobadores", "nom",id);
            ViewBag.aprobador = new SelectList(_asiapr.obtenerAprobadoresAsignados("175").Select(x => new { x.idAccApro, nom = x.aprobador.empleado.nomComEmp }), "idAccApro", "nom");
            //*********************************************************************
            var model = _sis.obtenerSolicitudesAprobarxAdm( ConstantesGlobales.tipPag_tran,id,estado, primero, actual);
            //********************************************************************
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            //********************************************************************
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult AprobarAdministrador(string menuArea, string menuVista, string fchEveSolGasI, string fchEveSolGasF, string estados, string idaprobadores)
        {
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
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = DateTime.Today.ToString("dd/MM/yyyy");
                inicio = DateTime.Parse(primero);
                fin = DateTime.Parse(actual);
            }
            //*********************************************************************
            string[] w = idaprobadores.Split('|');
            ViewBag.estados = new SelectList(_est.obteneEstadosAprobarxAdm(), "idEst", "nomEst", estados);
            ViewBag.aprobadores = new SelectList(_asiapr.obtenerTodosAproAsig().Select(x => new { idaprobadores = x.idAccApro + "|" + x.aprobador.idNapro, nom = x.aprobador.empleado.nomComEmp }).Distinct(), "idaprobadores", "nom", idaprobadores);
            ViewBag.aprobador = new SelectList(_asiapr.obtenerAprobadoresAsignados(w[0]).Select(x => new { x.idAccApro, nom = x.aprobador.empleado.nomComEmp }), "idAccApro", "nom");

            //*********************************************************************
            var model = _sis.obtenerSolicitudesAprobarxAdm(ConstantesGlobales.tipPag_tran, w[0], estados, inicio.ToString(), fin.ToString());
            //*********************************************************************
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //*********************************************************************
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }

        //Imprimir
        [CustomAuthorize(Roles = "000003,000227")]
        [HttpGet]
        public ActionResult Imprimir(Boolean html,string sol)
        {
            var solicitud = _sis.obtenerItem(sol);

            ViewBag.pdf = html;
            return new PdfActionResult(solicitud)
            {
                FileDownloadName = "Solicitud - " + solicitud.idSolGas.ToString()+" - "+ solicitud.estado.nomEst + ".pdf"
            };
        }

        [HttpGet]
        public ActionResult ImprimirV(Boolean html, string solicitudes)
        {
            string[] sols = solicitudes.Split('|');
            var solicitud = _sis.obtenerSolicitudesVarios(sols);
            ViewBag.pdf = html;

            return new PdfActionResult(solicitud, (writer, document) => { document.SetPageSize(new Rectangle(595, 850)); document.NewPage(); })
            {
                FileDownloadName = "Solicitudes"+ "_" + DateTime.Today.Day+"_"+ DateTime.Today.Month+"_"+ DateTime.Today.Year+".pdf"
            };
        }

        [CustomAuthorize(Roles = "000003,000228")]
        [HttpGet]
        public ActionResult Contabilizar(string menuArea, string menuVista)
        {
            //Primero obtenemos el día actual
            DateTime date = DateTime.Now;
            //Asi obtenemos el primer dia del mes actual
            DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);

            ViewBag.conta = new SelectList(_est.obteneEstadoContabilizar(), "idEst", "nomEst", ConstantesGlobales.estadoAprobado);
            var primero= oPrimerDiaDelMes.ToString("dd/MM/yyyy");
            var actual= DateTime.Today.ToString("dd/MM/yyyy");
            ViewBag.primero = primero;
            ViewBag.actual = actual;
            //var model = _sis.obtenerSolicitudes().Where(x => (x.idTipPag == ConstantesGlobales.tipPag_tran) && (x.idEst == ConstantesGlobales.estadoAprobado) && ((x.usufchCrea >= DateTime.Parse(primero)) && (x.usufchCrea <= DateTime.Parse(actual))));
            var model = _sis.obtenerSolicitudesContabilizar(ConstantesGlobales.tipPag_tran, ConstantesGlobales.estadoAprobado, primero, actual);
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Contabilizar(string menuArea, string menuVista, string fchEveSolGasI, string fchEveSolGasF,string estado)
        {
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();
            try
            {
                inicio = DateTime.Parse(fchEveSolGasI);
                fin = DateTime.Parse(fchEveSolGasF);
            }
            catch(Exception e)
            {
                DateTime date = DateTime.Now;
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = DateTime.Today.ToString("dd/MM/yyyy");
                inicio = DateTime.Parse(primero);
                fin = DateTime.Parse(actual);
            }

            ViewBag.conta = new SelectList(_est.obteneEstadoContabilizar(), "idEst", "nomEst", estado);
            IEnumerable<SolicitudGastoModels> model = null;

           // if(estado == ConstantesGlobales.estadoAtendido || estado == ConstantesGlobales.estadoNoAtendido)
           // {
                // model = _sis.obtenerSolicitudes().Where(x => (x.idTipPag == ConstantesGlobales.tipPag_tran) && (x.idEst == estado) && ((x.dFirma.Where(y => y.idEst == estado).Select(y => y.usufchCrea).FirstOrDefault() >= inicio) && ((x.dFirma.Where(y => y.idEst == estado).Select(y => y.usufchCrea).FirstOrDefault() <= fin.AddHours(23).AddMinutes(59)))));
                model = _sis.obtenerSolicitudesContabilizar(ConstantesGlobales.tipPag_tran, estado, inicio.ToString(), fin.ToString());
           // }
           // else
           // {
                // model = _sis.obtenerSolicitudes().Where(x => (x.idTipPag == ConstantesGlobales.tipPag_tran) && (x.idEst == estado) && ((x.usufchCrea >= inicio) && (x.usufchCrea <= fin.AddHours(23).AddMinutes(59))));
               // model = _sis.obtenerSolicitudesContabilizar(ConstantesGlobales.tipPag_tran, estado, inicio.ToString(), fin.ToString());
           // }

            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");

            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }
        [CustomAuthorize(Roles = "000003,000235")]
        [HttpGet]
        public ActionResult Liquidar(string menuArea, string menuVista)
        {
            //Primero obtenemos el día actual
            DateTime date = DateTime.Now;
            //Asi obtenemos el primer dia del mes actual
            DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
            var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
            var actual = DateTime.Today.ToString("dd/MM/yyyy");

            ViewBag.primero = primero;
            ViewBag.actual = actual;
            ViewBag.fecha = actual;
            ViewBag.Liqui = new SelectList(_est.obteneEstadoLiquidar(), "idEst", "nomEst");
            ViewBag.fLiqui = new SelectList(_est.obteneEstadoLiquidarFiltro(), "idEst", "nomEst", ConstantesGlobales.estadoAtendido);
            //var model = _sis.obtenerSolicitudes().Where(x => (x.idTipPag == ConstantesGlobales.tipPag_tran)&&(x.idEst == ConstantesGlobales.estadoAtendido) && ((x.usufchMod >= DateTime.Parse(primero)) && (x.usufchMod <= DateTime.Parse(actual))));
            var model = _sis.obtenerSolicitudesLiquidar(ConstantesGlobales.estadoAtendido, primero, actual);
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;

            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Liquidar(string menuArea, string menuVista, string fchEveSolGasI, string fchEveSolGasF, string estado)
        {

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
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = DateTime.Today.ToString("dd/MM/yyyy");
                inicio = DateTime.Parse(primero);
                fin = DateTime.Parse(actual);
            }


            ViewBag.Liqui = new SelectList(_est.obteneEstadoLiquidar(), "idEst", "nomEst");
            ViewBag.fLiqui = new SelectList(_est.obteneEstadoLiquidarFiltro(), "idEst", "nomEst", ConstantesGlobales.estadoAtendido);
            //var model = _sis.obtenerSolicitudes().Where(x => (x.idTipPag == ConstantesGlobales.tipPag_tran) && (x.idEst == estado) && ((x.usufchCrea >= inicio) && (x.usufchCrea <= fin.AddHours(23).AddMinutes(59))));
            var model = _sis.obtenerSolicitudesLiquidar(estado, inicio.ToString(), fin.ToString());
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            ViewBag.fecha = fin.ToString("dd/MM/yyyy");
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }
        
        
        [CustomAuthorize(Roles = "000003,000403")]
        [HttpGet]
        public ActionResult Deposito(string menuArea, string menuVista)
        {
            //Primero obtenemos el día actual
            DateTime date = DateTime.Now;
            //Asi obtenemos el primer dia del mes actual
            DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);

            ViewBag.conta = new SelectList(_est.obteneEstadosDeposito(), "idEst", "nomEst", ConstantesGlobales.estadoEnGiro);
            var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
            var actual = DateTime.Today.ToString("dd/MM/yyyy");
            ViewBag.primero = primero;
            ViewBag.actual = actual;
            var model = _sis.obtenerSolicitudesContabilizar(ConstantesGlobales.tipPag_tran, ConstantesGlobales.estadoEnGiro, primero, actual);
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Deposito(string menuArea, string menuVista, string fchEveSolGasI, string fchEveSolGasF, string estado)
        {
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
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, date.Month, 1);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = DateTime.Today.ToString("dd/MM/yyyy");
                inicio = DateTime.Parse(primero);
                fin = DateTime.Parse(actual);
            }

            ViewBag.conta = new SelectList(_est.obteneEstadosDeposito(), "idEst", "nomEst", estado);
            IEnumerable<SolicitudGastoModels> model = null;

            model = _sis.obtenerSolicitudesContabilizar(ConstantesGlobales.tipPag_tran, estado, inicio.ToString(), fin.ToString());
           

            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");

            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(model);
        }

        //acciones
        [HttpPost]
        public JsonResult anularSolicitud(string idSolGas)
        {
            var variable = _sis.updateEstadoSol(idSolGas, SessionPersister.UserId, ConstantesGlobales.estadoAnulado);
            if (variable){
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
            }
            return Json(variable, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult aprobarSolicitud(string sol, string apr, string glo)
        {
            //valores iniciales
            Boolean correcto = true;
            Boolean act = false;//actualizacion incorrecta
            Boolean firma = false;//firma incorrecta
            string estado = ConstantesGlobales.estadoPreApro;
            if (SessionPersister.NivApr== ConstantesGlobales.TERCERO)
            {
                estado = ConstantesGlobales.estadoAprobado;
            }
            //convierto las solicitudes en un arreglo
            string[] codigo= sol.Split('|');

            foreach (var c in codigo)
            {
                act =_sis.updateEstadoSol(c, apr, estado);

                if (act)
                {
                    _mov.updateEstMovPres(c, estado);
                    FirmasSoliGastoModels f = new FirmasSoliGastoModels();
                    f.idSolGas = c;
                    f.idAcc = SessionPersister.UserId;
                    f.idEst = estado;
                    f.usuCrea = SessionPersister.Username;
                    f.usufchCrea = DateTime.Now;
                    f.idNapro = SessionPersister.NivApr;
                    f.obsFirSol = glo;
                    firma = _sis.mergeFirmas(f);
                    if (!firma) { break; }//si firma esta incorrecta salgo del for
                    f = new FirmasSoliGastoModels();
                }
                else { break; }
            }
            if (!act || !firma)//ambos deben ser corectos
            {
                correcto = false;
            }
            return Json(correcto, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult rechazarSolicitud(string sol, string apr, string glo)
        {
            //valores iniciales
            Boolean correcto = true;
            Boolean act = false;//actualizacion incorrecta
            Boolean firma = false;//firma incorrecta
            string estado = ConstantesGlobales.estadoRechazado;
            //convierto las solicitudes en un arreglo
            string[] codigo = sol.Split('|');

            foreach (var c in codigo)
            {
                string cod = c;//idSolGas
                act = _sis.updateEstadoSol(cod, apr, estado);
                if (act)
                {
                    FirmasSoliGastoModels f = new FirmasSoliGastoModels();
                    f.idSolGas = cod;
                    f.idAcc = SessionPersister.UserId;
                    f.idEst = estado;
                    f.usuCrea = SessionPersister.Username;
                    f.usufchCrea = DateTime.Now;
                    f.idNapro = SessionPersister.NivApr;
                    f.obsFirSol = glo;
                    firma = _sis.mergeFirmas(f);
                    if (firma)
                    {
                         var mov = _mov.obtenerMovimientos(cod);
                         double monSolGas = 0;
                        foreach (var m in mov)
                        {
                                if (m.presupuesto.idMon ==  m.idMon )
                                {
                                    monSolGas = m.monSolGas;
                                }
                                else
                                {
                                    if (m.presupuesto.idMon == ConstantesGlobales.monedaSol)
                                    {
                                        monSolGas = Math.Round(m.monSolGas * m.valtipCam,2);
                                    }
                                    if (m.presupuesto.idMon == ConstantesGlobales.monedaDol)
                                    {
                                        monSolGas = Math.Round(m.monSolGas / m.valtipCam,2);
                                    }
                                }
                                double descontar = m.presupuesto.Saldo + monSolGas;
                               

                            //actualizacion
                            Boolean sp = _mov.updateSaldoPres(m.idPres, descontar);
                            Boolean mp = _mov.updateMovPres(cod, m.idPres, estado, 0);

                            //actualizacion
                            if (!sp || !mp) { break; }
                        }
                        
                    }
                    else { break; }
                    f = new FirmasSoliGastoModels();
                }
                else { break; }
            }
            if (!act || !firma)//ambos deben ser corectos
            {
                correcto = false;
            }
            return Json(correcto, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult noAtenderSolicitud(string sol, string glo)
        {
            //valores iniciales
            Boolean correcto = true;
            Boolean firma = false;//firma incorrecta
            Boolean act = false;
            string estado = ConstantesGlobales.estadoNoAtendido;
            //convierto las solicitudes en un arreglo
            string[] codigo = sol.Split('|');
           
            foreach (var c in codigo)
            {
                string cod = c;//idSolGas
                act = _sis.updateEstadoSol(cod, "", estado);
                if (act)
                {
                    FirmasSoliGastoModels f = new FirmasSoliGastoModels();
                    f.idSolGas = cod;
                    f.idAcc = SessionPersister.UserId;
                    f.idEst = estado;
                    f.usuCrea = SessionPersister.Username;
                    f.usufchCrea = DateTime.Now;
                    f.idNapro = SessionPersister.NivApr;
                    f.obsFirSol = glo;
                    firma = _sis.mergeFirmas(f);
                    if (firma)
                    {
                        var mov = _mov.obtenerMovimientos(cod);
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
                            Boolean mp = _mov.updateMovPres(cod, m.idPres, estado, 0);

                            //actualizacion
                            if (!sp || !mp) { firma = false; break; }
                        }
                    }
                    else { break; }
                    f = new FirmasSoliGastoModels();
                }
                else { break; }
            }

            if (!act || !firma)//ambos deben ser corectos
            {
                correcto = false;
            }
            return Json(correcto, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult enGiroSolicitud(string solicitudes, string idGlosaApro)
        {
            //valores iniciales
            Boolean correcto = true;
            Boolean act = false;//actualizacion incorrecta
            Boolean firma = false;//firma incorrecta
            string estado = ConstantesGlobales.estadoEnGiro;
            
            //convierto las solicitudes en un arreglo
            string[] sols = solicitudes.Split('|');
            try
            {
                //Realizo la actualizacion de solicitud y creacion de firmas
                foreach (var c in sols)
                {
                    act = _sis.updateEstadoSol(c, "", estado);
                    if (act)
                    {
                        _mov.updateEstMovPres(c, estado);
                        FirmasSoliGastoModels f = new FirmasSoliGastoModels();
                        f.idSolGas = c;
                        f.idAcc = SessionPersister.UserId;
                        f.idEst = estado;
                        f.usuCrea = SessionPersister.Username;
                        f.usufchCrea = DateTime.Now;
                        f.idNapro = SessionPersister.NivApr;
                        f.obsFirSol = idGlosaApro;
                        firma = _sis.mergeFirmas(f);
                        if (!firma) { break; }//si firma esta incorrecta salgo del for
                        f = new FirmasSoliGastoModels();
                    }
                    else { break; }
                }
                if (!act || !firma)//ambos deben ser corectos
                {
                    correcto = false;
                }
            }
            catch (Exception e)
            {
                correcto = false;
            }

            return Json(correcto, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult atenderSolicitud(string solicitudes, string idGlosaAten)
        {
            //valores iniciales
            Boolean correcto = true;
            Boolean act = false;//actualizacion incorrecta
            Boolean firma = false;//firma incorrecta
            string estado = ConstantesGlobales.estadoAtendido;
            //convierto las solicitudes en un arreglo
            string[] sols = solicitudes.Split('|');
            try
            {
                //Realizo la actualizacion de solicitud y creacion de firmas
                foreach (var c in sols)
                {
                    act = _sis.updateEstadoSol(c, "", estado);
                    if (act)
                    {
                        _mov.updateEstMovPres(c, estado);
                        FirmasSoliGastoModels f = new FirmasSoliGastoModels();
                        f.idSolGas = c;
                        f.idAcc = SessionPersister.UserId;
                        f.idEst = estado;
                        f.usuCrea = SessionPersister.Username;
                        f.usufchCrea = DateTime.Now;
                        f.idNapro = SessionPersister.NivApr;
                        f.obsFirSol = idGlosaAten;
                        firma = _sis.mergeFirmas(f);
                        if (!firma) { break; }//si firma esta incorrecta salgo del for
                        f = new FirmasSoliGastoModels();
                    }
                    else { break; }
                }
                if (!act || !firma)//ambos deben ser corectos
                {
                    correcto = false;
                }
            }
            catch (Exception e)
            {
                correcto = false;
            }

            return Json(correcto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult liquidarSolicitud(string solicitud,string idEst,DateTime fchLiq,string obsLiq,double liqValRea)
        {
            string mensaje = "ok";
            var pre = _mov.obtenerMovimientos(solicitud);

            if (idEst==ConstantesGlobales.estadoDParcial)
            {
                LiquidaGastoModels l = new LiquidaGastoModels();
                l.idSolGas = solicitud;
                l.liqValRea = liqValRea;
                l.idEst = idEst;
                l.usuCrea = SessionPersister.Username;
                l.usufchCrea = DateTime.Now;
                l.fchLiq = fchLiq;
                l.obsLiq = obsLiq;
                if (_liq.mergeLiquidacion(l))
                {
                    FirmasSoliGastoModels f = new FirmasSoliGastoModels();
                    f.idSolGas = solicitud;
                    f.idAcc = SessionPersister.UserId;
                    f.idEst = ConstantesGlobales.estadoLiquidado;
                    f.usuCrea = SessionPersister.Username;
                    f.usufchCrea = DateTime.Now;
                    f.idNapro = SessionPersister.NivApr;
                    f.obsFirSol = obsLiq;
                    if (_sis.mergeFirmas(f))
                    {
                       if( _sis.updateEstadoSol(solicitud, "", ConstantesGlobales.estadoLiquidado))
                        {
                            if (pre.Count() > 0)
                            {
                                var j = 0;
                                double montoL = liqValRea;
                                do
                                {
                                    double mov = pre[j].monSolGas;
                                    montoL = montoL - mov;
                                    //saldos nuevos
                                    double nueMov = 0;
                                    double nueSal = 0;

                                    if (montoL >= 0)
                                    {
                                        if (pre[j].idMon != pre[j].presupuesto.idMon)
                                        {
                                            if (pre[j].presupuesto.idMon == ConstantesGlobales.monedaSol)
                                            {
                                                mov = Math.Round(mov * pre[j].valtipCam, 2);
                                            }
                                            else if (pre[j].presupuesto.idMon == ConstantesGlobales.monedaDol)
                                            {
                                                mov = Math.Round(mov / pre[j].valtipCam, 2);
                                            }
                                        }

                                        nueSal = mov + pre[j].presupuesto.Saldo;
                                        nueMov = 0;
                                    }
                                    else
                                    {
                                        mov = montoL + mov;
                                        if (pre[j].idMon != pre[j].presupuesto.idMon)
                                        {
                                            if (pre[j].presupuesto.idMon == ConstantesGlobales.monedaSol)
                                            {
                                                mov = Math.Round(mov * pre[j].valtipCam, 2);
                                            }
                                            else if (pre[j].presupuesto.idMon == ConstantesGlobales.monedaDol)
                                            {
                                                mov = Math.Round(mov / pre[j].valtipCam, 2);
                                            }
                                        }
                                        nueSal = mov + pre[j].presupuesto.Saldo;
                                        nueMov = -1 * montoL;
                                    }


                                    Boolean sp = _mov.updateSaldoPres(pre[j].idPres, nueSal);
                                    Boolean mp = _mov.updateMovPres(pre[j].idSolGas, pre[j].idPres, ConstantesGlobales.estadoLiquidado, nueMov);

                                    //actualizacion
                                    if (!sp || !mp) { mensaje = "Se produjo un error en el saldo o movimiento"; break; }

                                    j++;

                                } while (montoL > 0);
                            }
                            //Devolucion Parcial a la estimacion
                            // Primero debo hallar la solicitud y la estimacion relacionada -- Si existe estimacion
                            var existeEst = _sis.obtenerDetSolGastos(solicitud).ToList().Count();
                            if (existeEst > 0)
                            {
                                updateDetEstim_PtoEstimLiq(solicitud, liqValRea, idEst);
                            }
                            //==================================
                        }
                        else { mensaje = "error en la actualización del estado de solicitud"; }
                    }
                    else { mensaje = "error en la creación o actualización de firmas"; }
                }
                else { mensaje = "error en la creación o actualización de liquidación"; }
            }
            else if(idEst == ConstantesGlobales.estadoDTotal)
            {
                LiquidaGastoModels l = new LiquidaGastoModels();
                l.idSolGas = solicitud;
                l.liqValRea = liqValRea;
                l.idEst = idEst;
                l.usuCrea = SessionPersister.Username;
                l.usufchCrea = DateTime.UtcNow;
                l.fchLiq = fchLiq;
                l.obsLiq = obsLiq;
                if (_liq.mergeLiquidacion(l))
                {
                    FirmasSoliGastoModels f = new FirmasSoliGastoModels();
                    f.idSolGas = solicitud;
                    f.idAcc = SessionPersister.UserId;
                    f.idEst = ConstantesGlobales.estadoLiquidado;
                    f.usuCrea = SessionPersister.Username;
                    f.usufchCrea = DateTime.Now;
                    f.idNapro = SessionPersister.NivApr;
                    f.obsFirSol = obsLiq;
                    if (_sis.mergeFirmas(f))
                    {
                        if (_sis.updateEstadoSol(solicitud, "", ConstantesGlobales.estadoLiquidado))
                        {
                            if(pre.Count()>0)
                            { 
                                foreach (var p in pre)
                                {
                                    double mov = p.monSolGas;
                                    double nueMov = 0;
                                    double nueSal = 0;

                                    if (p.idMon != p.presupuesto.idMon)
                                    {
                                        if (p.presupuesto.idMon == ConstantesGlobales.monedaSol)
                                        {
                                            mov = Math.Round(mov * p.valtipCam, 2);
                                        }
                                        else if (p.presupuesto.idMon == ConstantesGlobales.monedaDol)
                                        {
                                            mov = Math.Round(mov / p.valtipCam, 2);
                                        }
                                    }
                                    nueSal = mov + p.presupuesto.Saldo;

                                    Boolean sp = _mov.updateSaldoPres(p.idPres, nueSal);
                                    Boolean mp = _mov.updateMovPres(p.idSolGas, p.idPres, ConstantesGlobales.estadoLiquidado, nueMov);

                                    //actualizacion
                                    if (!sp || !mp) { mensaje = "Se produjo un error en el saldo o movimiento"; break; }
                                }
                            }
                            //Devolucion Total a la Estimacion
                            // Primero debo hallar la solicitud y la estimacion relacionada -- Si existe estimacion
                            var existeEst = _sis.obtenerDetSolGastos(solicitud).ToList().Count();
                            if (existeEst > 0)
                            {
                                updateDetEstim_PtoEstimLiq(solicitud, liqValRea, idEst);
                            }
                            //==================================
                            //================================
                        }
                        else { mensaje = "error en la actualización del estado de solicitud"; }
                    }
                    else { mensaje = "error en la creación o actualización de firmas"; }
                }
                else { mensaje = "error en la creación o actualización de liquidación"; }
            }
            else if (idEst == ConstantesGlobales.estadoReembolso)
            {
                LiquidaGastoModels l = new LiquidaGastoModels();
                l.idSolGas = solicitud;
                l.liqValRea = liqValRea;
                l.idEst = idEst;
                l.usuCrea = SessionPersister.Username;
                l.usufchCrea = DateTime.Now;
                l.fchLiq = fchLiq;
                l.obsLiq = obsLiq;
                if (_liq.mergeLiquidacion(l))
                {
                    FirmasSoliGastoModels f = new FirmasSoliGastoModels();
                    f.idSolGas = solicitud;
                    f.idAcc = SessionPersister.UserId;
                    f.idEst = ConstantesGlobales.estadoLiquidado;
                    f.usuCrea = SessionPersister.Username;
                    f.usufchCrea = DateTime.Now;
                    f.idNapro = SessionPersister.NivApr;
                    f.obsFirSol = obsLiq;
                    if (_sis.mergeFirmas(f))
                    {
                        if (_sis.updateEstadoSol(solicitud, "", ConstantesGlobales.estadoLiquidado))
                        {
                            if (pre.Count() > 0)
                            {
                                double montoL = liqValRea;
                                double tp = 0;//Variable del total de presupuesto
                                foreach (var p in pre)
                                {
                                    if (p.idMon == p.presupuesto.idMon)
                                    {
                                        tp = tp + p.presupuesto.Saldo;
                                    }
                                    else
                                    {
                                        //presupuesto en dolares con movimiento en soles
                                        if (p.presupuesto.idMon == ConstantesGlobales.monedaDol)
                                        {
                                            tp = tp + Math.Round(p.presupuesto.Saldo * p.valtipCam, 2);
                                        }
                                        //presupuesto en soles con moneda en dolares
                                        if (p.presupuesto.idMon == ConstantesGlobales.monedaSol)
                                        {
                                            tp = tp + Math.Round(p.presupuesto.Saldo / p.valtipCam, 2);
                                        }
                                    }
                                }
                                if (tp >= montoL)
                                {
                                    var j = 0;
                                    do
                                    {
                                        //saldos nuevos
                                        double nueMov = 0;
                                        double nueSal = 0;

                                        if (pre[j].presupuesto.idMon == pre[j].idMon)
                                        {
                                            montoL = montoL - pre[j].presupuesto.Saldo;

                                            if (montoL > 0)
                                            {
                                                nueSal = 0;
                                                nueMov = liqValRea - montoL + pre[j].monSolGas;
                                            }
                                            else
                                            {
                                                nueSal = (montoL * -1);
                                                nueMov = (montoL) + pre[j].presupuesto.Saldo + pre[j].monSolGas;
                                            }
                                        }
                                        else
                                        {
                                            if (pre[j].presupuesto.idMon == ConstantesGlobales.monedaSol)
                                            {
                                                montoL = montoL - Math.Round(pre[j].presupuesto.Saldo / pre[j].valtipCam, 2);

                                                if (montoL > 0)
                                                {
                                                    nueSal = 0;
                                                    nueMov = liqValRea - montoL + pre[j].monSolGas;
                                                }
                                                else
                                                {
                                                    nueSal = Math.Round((montoL * -1) * pre[j].valtipCam, 2);
                                                    nueMov = (montoL) + Math.Round(pre[j].presupuesto.Saldo / pre[j].valtipCam, 2) + pre[j].monSolGas;
                                                }
                                            }
                                            if (pre[j].presupuesto.idMon == ConstantesGlobales.monedaDol)
                                            {
                                                montoL = montoL - Math.Round(pre[j].presupuesto.Saldo * pre[j].valtipCam, 2);

                                                if (montoL >= 0)
                                                {
                                                    nueSal = 0;
                                                    nueMov = liqValRea - montoL + pre[j].monSolGas;
                                                }
                                                else
                                                {
                                                    nueSal = Math.Round((montoL * -1) / pre[j].valtipCam, 2);
                                                    nueMov = (montoL) + Math.Round(pre[j].presupuesto.Saldo * pre[j].valtipCam, 2) + pre[j].monSolGas;
                                                }
                                            }
                                        }
                                        Boolean sp = _mov.updateSaldoPres(pre[j].idPres, nueSal);
                                        Boolean mp = _mov.updateMovPres(pre[j].idSolGas, pre[j].idPres, ConstantesGlobales.estadoLiquidado, nueMov);
                                        if (!sp || !mp) { mensaje = "Se produjo un error en el saldo o movimiento"; break; }

                                        j++;

                                    } while (montoL > 0);
                                    //Reembolso a la estimacion
                                    // Primero debo hallar la solicitud y la estimacion relacionada -- Si existe estimacion
                                    var existeEst = _sis.obtenerDetSolGastos(solicitud).ToList().Count();
                                    if (existeEst > 0)
                                    {
                                        updateDetEstim_PtoEstimLiq(solicitud, liqValRea, idEst);
                                    }
                                    //==================================
                                    //================================
                                    //=========================
                                }
                                else { mensaje = "No cuenta con saldo suficiente para realizar esta liquidación"; }
                            }
                        }
                        else { mensaje = "error en la actualización del estado de solicitud"; }
                    }
                    else { mensaje = "error en la creación o actualización de firmas"; }
                }
                else { mensaje = "error en la creación o actualización de liquidación"; }
            }
            else
            {
                mensaje = "No existe la tipo de configuración que se selecciono";
            }

            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }
        //Adicional
        public JsonResult verSaldoDisponible(string idSolGas)
        {
            MovimientoPresModels movPres = _mov.obtenerMovimiento(idSolGas);
            string simbModemovPres = "";
            if (movPres.idMon == ConstantesGlobales.monedaDol)
            {
                simbModemovPres = "$ ";
            }
            else
            {
                simbModemovPres = "S/. ";
            }

            var result = new
            {
                saldoPres = simbModemovPres + movPres.presupuesto.Saldo.ToString()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //busquedas
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
        public JsonResult buscarTipoDeGasto(string idAccRes)
        {
            return Json(selectTipoDeGasto(idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult verificarPresupuesto(string resp, string apro, string lin, string zon, string tip, string gas, string esp)
        {
            Parametros p = new Parametros();
            var id = resp.Split('-');
            bool verdadPT = false;
            bool verdadPF = false;
            bool verdadPM = false;
            List<SelectedModels> listado = new List<SelectedModels>();
            SelectedModels c = new SelectedModels();
            var idAccRes = id[0];
            if (p.Resultado(ConstantesGlobales.Com_Usu_Pre_Cas_01).Contains(idAccRes))//verificar si va tener presupuesto PLAN DE TRABAJO
            {
                if (apro!="" && lin != "" && zon != "")
                { 
                verdadPT = true;
                var obtener = _pre.obtenerListaSeleccion(ConstantesGlobales.plan_Trab, idAccRes, apro, lin, zon, "","", "");
                if (obtener.Count() != 0)
                {
                    var planDetrab = obtener.Select(x => new { codigo = x.idPres, nombre = x.tipospres.nomTipPres + "-N°: " + x.idPres + " | " + "Vencimiento: " + x.fchFinVigencia.ToShortDateString() + " | " + "Importe: " + x.moneda.simbMon + " " + x.Monto + " | " + "Saldo: " + x.moneda.simbMon + " " + x.Saldo });
                    foreach (var v in planDetrab)
                    {
                        c.value = v.codigo;
                        c.text = v.nombre;
                        listado.Add(c);
                        c = new SelectedModels();
                    }
                }
                }
            }
            if (p.Resultado(ConstantesGlobales.Com_Usu_Pre_Cas_04).Contains(idAccRes))//verificar si va tener presupuesto FUERA DEL PLAN
            {
                verdadPF = true;
                string zonv = "0";
                string linv = "99";
                var obten = _pre.obtenerListaSeleccion(ConstantesGlobales.plan_Fuera, idAccRes, "", "", "", "","", "");
                if (obten.Count() != 0)
                {
                    var planFuera = obten.Select(x => new { codigo = x.idPres, nombre = x.tipospres.nomTipPres + "-N°: " + x.idPres + " | " + "Vencimiento: " + x.fchFinVigencia.ToShortDateString() + " | " + "Importe: " + x.moneda.simbMon + " " + x.Monto + " | " + "Saldo: " + x.moneda.simbMon + " " + x.Saldo });
                    foreach (var v in planFuera)
                    {
                        c.value = v.codigo;
                        c.text = v.nombre;
                        listado.Add(c);
                        c = new SelectedModels();
                    }
                }
            }
            if (p.Resultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).Contains(idAccRes))//verificar si va tener presupuesto
            {
                verdadPM = true;
                var obtenerM = _pre.obtenerListaSeleccion(ConstantesGlobales.plan_Mark, idAccRes, "", "", "", tip, gas, esp);
                if (obtenerM.Count() != 0)
                {
                    var planMkt =obtenerM.Select(x => new { codigo = x.idPres, nombre = x.tipospres.nomTipPres + "-N°: " + x.idPres + " | " + "Vencimiento: " + x.fchFinVigencia.ToShortDateString() + " | " + "Importe: " + x.moneda.simbMon + " " + x.Monto + " | " + "Saldo: " + x.moneda.simbMon + " " + x.Saldo });
                    foreach (var v in planMkt)
                    {
                        c.value = v.codigo;
                        c.text = v.nombre;
                        listado.Add(c);
                        c = new SelectedModels();
                    }
                }
            }
            if (verdadPT || verdadPF || verdadPM)
            {
                return new JsonResult { Data = listado, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else {
                //no esta configurada pata tener presupuesto
                return Json("NO", JsonRequestBehavior.AllowGet);
            }
        }
        //Reutilizables
        public string[] listaDetalle(string info)
        {
            string[] detalle =null ;
            if (info!="")
            {
                detalle = info.Split('|');
            }
            return detalle;
        }
        public void cargarCombos(SolicitudGastoModels model)
        {
            Parametros p = new Parametros();

            //envio los combos ya cargados
            try
            {
                var id = model.idAccRes.Split('-');
                var idAccRes = id[0];

                if (p.Resultado(ConstantesGlobales.Com_SolGas_Reg_Resp).Contains(model.idAccSol))
                {
                    ViewBag.responsable = new SelectList(_asiapr.obtenerAproYou(idAccRes).Select(x => new { idAcc = x.idAcc + "-" + x.idNapro, nombre = x.empleado.nomComEmp }), "idAcc", "nombre", model.idAccRes + "-" + SessionPersister.NivApr);
                }
                else
                {
                    ViewBag.responsable = new SelectList(_usu.obtenerUsuarios().Where(x => x.idAcc == model.idAccSol).Select(x => new { idAcc = x.idAcc + "-" + x.idNapro, nombre = x.empleado.nomComEmp }), "idAcc", "nombre", model.idAccRes + "-" + SessionPersister.NivApr);
                }
                //------
                ViewBag.linea = new SelectList(selectLinea(idAccRes), "value", "text",model.idLin);
                ViewBag.zona = new SelectList(selectZona(idAccRes), "value", "text",model.idZon);
                //------
                ViewBag.tipogasto = new SelectList(_tipU.obtenerTipoDeGastoXusuario(idAccRes).Select(x => new { idTipGas = x.idTipGas, nomTipGas = x.tiposDeGastos.nomTipGas }).Distinct(), "idTipGas", "nomTipGas", model.idTipGas);
                ViewBag.congasto = new SelectList(_cgas.obtenerConceptoGastos().Where(x => x.idTipGas == model.idTipGas), "idConGas", "nomConGas",model.idConGas);
                //------
                ViewBag.aprobador = new SelectList(_asiapr.obtenerAprobadoresAsignados(idAccRes).Select(x => new {x.idAccApro, nom =  x.aprobador.empleado.nomComEmp}), "idAccApro", "nom",model.idAccApro);
                //------
                ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "nomMon", model.idMon);
                ViewBag.especialidad = new SelectList(_esp.obteneEspecialidades(), "idEsp", "nomEsp",model.idEsp);
                ViewBag.tipocambio = model.valtipCam;
                //------
                ViewBag.presupuesto= new SelectList((selectPresupuesto(idAccRes,model.idAccApro,model.idLin,model.idZon)).Select(x => new { codigo = x.value, nombre = x.text }), "codigo", "nombre");
                //------
            }
            catch (Exception e) { }
        }
        public List<SelectedModels> selectPresupuesto(string idAccRes, string idAccApro,string idLin,string idZon)
        {
            //------------Apartado----------------------
            var ptoPlan = _pre.obtenerListaSeleccion(ConstantesGlobales.plan_Trab, idAccRes, idAccApro, idLin, idZon, "", "","").Select(x => new { codigo = x.idPres, nombre = x.tipospres.nomTipPres + "-N°: " + x.idPres + " | " + "Vencimiento: " + x.fchFinVigencia.ToShortDateString() + " | " + "Importe: " + x.moneda.simbMon + " " + x.Monto + " | " + "Saldo: " + x.moneda.simbMon + " " + x.Saldo });
            var ptoFuera = _pre.obtenerListaSeleccion(ConstantesGlobales.plan_Fuera, idAccRes, "", "", "", "", "","").Select(x => new { codigo = x.idPres, nombre = x.tipospres.nomTipPres+"-N°: " + x.idPres + " | " + "Vencimiento: " + x.fchFinVigencia.ToShortDateString() + " | " + "Importe: " + x.moneda.simbMon + " " + x.Monto + " | " + "Saldo: " + x.moneda.simbMon + " " + x.Saldo });
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            foreach (var v in ptoPlan)
            {
                cbo.value = v.codigo;
                cbo.text = v.nombre;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            foreach (var v in ptoFuera)
            {
                cbo.value = v.codigo;
                cbo.text = v.nombre;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            //------------Apartado----------------------
            return cboList; 
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
        public void creaTablas(string familiaD, string medicoD, string responsableD)
        {
            string rowsF = "";
            string rowsM = "";
            string rowsR = "";

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
        }
        public List<SelectedModels> selectLinea(string idAccRes)
        {
            var i = idAccRes.Split('-');
            var res = i[0];

            Parametros p = new Parametros();
            var responsable = _usu.obtenerUsuarios().Where(z => z.idAcc == res).FirstOrDefault();

            var linea = _lin.obtenerLineas().Select(x => new { x.idLin, x.nomLin });
            if (p.Resultado(ConstantesGlobales.Com_SolGas_Reg_Cas_01).Contains(responsable.empleado.idCarg))
            {
                var trilogia = _uzl.obtenerTrilogia();
                linea = trilogia.Where(x => x.idAcc == res).Select(x => new { x.linea.idLin, x.linea.nomLin }).Distinct();
            }
            else if (p.Resultado(ConstantesGlobales.Com_SolGas_Reg_Cas_02).Contains(responsable.empleado.idCarg))
            {
                linea = _pl.obtenerLinZonxUsu(responsable.idEmp).Select(x => new { x.linea.idLin, x.linea.nomLin }).Distinct();
            }

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
            string detalle = "";
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
                porcen = (double)100 / filas;
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
                    cadena = item[0] + ";" + item[1] + ";" + item[1];
                    if (!cadenaFam.Contains(item[0]))
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
                for (int i = 0; i < vecDetAT.GetLength(0); i++)
                {
                    string AT = "";
                    double porc = 0;
                    AT = vecDetAT[i, 1];
                    porc = double.Parse(vecDetAT[i, 2]);
                    //----
                    if (list == null)
                    {
                        model.idAreaTerap = AT;
                        model.valPorcen = porc;
                        list.Add(model);
                    }
                    if (list.Any(item => item.idAreaTerap == AT))
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
                    valor = importe * (d.valPorcen / 100);
                    valor = Math.Round(valor, 2, MidpointRounding.AwayFromZero);
                    dif = dif - valor;
                    if (cantFil == counter)
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
        public Boolean guardarSolicitudDetalles(SolicitudGastoModels model, string familiaD, string medicoD, string responsableD,string areaterap)
        {
            var codigos = model.idAccRes.Split('-');
            model.idAccRes = codigos[0];
            model.idNapro = codigos[1];
            model.monImp = 0;
            model.monNeto = model.monImp + model.monSolGas;
            var resultado = _sis.crear(model);

            if (resultado) { 
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
                var a = listaDetalle(areaterap);

                //familia
                List<DetSolGasto_FamModels> listFam = new List<DetSolGasto_FamModels>();
                DetSolGasto_FamModels fam = new DetSolGasto_FamModels();
                if (f!=null)
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
                var rpt1 =_sis.crearDetalleF(listFam);
                var rpt2 =_sis.crearDetalleM(listMed);
                var rpt3 =_sis.crearDetalleR(listRes);
                var rpt6 =_sis.crearDetalleA(listArea);

                if (rpt1 & rpt2 & rpt3 & rpt6)
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
        //Seguimiento
        [CustomAuthorize(Roles = "000003,000249")]
        public ActionResult Seguimiento(string menuArea, string menuVista, string search = "", string fchEveSolGasI = "", string fchEveSolGasF = "")
        {
            //-------------------------------------------------------------------

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
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, DateTime.Now.Month, 1);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = DateTime.Today.ToString("dd/MM/yyyy");
                inicio = DateTime.Parse(primero);
                fin = DateTime.Parse(actual);
            }

            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");

            
            //-------------------------------------------------------------------
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            string id = SessionPersister.UserId;
            //===Obtener las solicitudes aprobadas por el usuario de la sesion
            var model = _sis.obtenerSolicitudesSeguimiento(id, inicio.ToString("dd/MM/yyyy"), fin.ToString("dd/MM/yyyy")).ToList();
            //===
            return View(model);
        }
        //========= Varia Estimacion segun Tipo de Liquidacion  ========
        public bool updateDetEstim_PtoEstimLiq(string id, double liqValReal, string idEst)
        {
            string mensaje = "";
            double sumaEstA = 0;
            string idPr = "";
            try
            {
                //+++++++++++++++++++++++++++
                var movEst = _sis.obtenerDetSolGastos(id).ToList();
                int cuenta = movEst.Count();
                //+++++++++++++++++++++++++++
                foreach (var i in movEst)
                {
                    var ant = _esti.obtenerDetalleEstimacion(i.idActiv).Where(x => x.idActGas == i.idActGas).FirstOrDefault();
                    double dif = 0;
                    double total = 0;

                    double valor = liqValReal;
                    if (i.solicitud.idMon == ant.cabecera.idMon)
                    {
                        dif = valor;
                        total = i.monTotal;
                    }
                    else
                    {
                        if (i.solicitud.idMon == ConstantesGlobales.monedaSol)
                        {
                            dif = Math.Round(valor / i.solicitud.valtipCam, 2);
                            total = Math.Round(i.monTotal / i.solicitud.valtipCam, 2);
                        }
                    }
                    //+++++++ Por tipo de liquidacion ++++++++
                    double salAct = 0;
                    //Por Devolucion Parcial
                    if (idEst == ConstantesGlobales.estadoDParcial)
                    {
                        double frac = Math.Round(dif / cuenta, 4);
                        sumaEstA += frac;
                        salAct = ant.salReal - frac;
                    }
                    //Por Devolucion Total
                    if (idEst == ConstantesGlobales.estadoDTotal)
                    {
                        double frac = total;
                        sumaEstA += frac;
                        salAct = ant.salReal - dif;
                    }
                    //Por Reembolso
                    if (idEst == ConstantesGlobales.estadoReembolso)
                    {
                        double frac = Math.Round(dif / cuenta, 4);
                        sumaEstA += frac;
                        salAct = ant.salReal + dif;
                    }
                    //++++++++++++++++++++++++++++++++++++++++++
                    _esti.modificarDetEstimSalReal(salAct, i.idActiv, i.idActGas, out mensaje);
                    idPr = ant.cabecera.idPres;
                }
                //**********************************
                //Se determino el valor del saldo estimado
                var pre = _pre.obtenerItem(idPr);
                //+++++++ Por tipo de liquidacion ++++++++
                //Por Devolucion Parcial o Por Devolucion Total
                if (idEst == ConstantesGlobales.estadoReembolso )
                {
                    sumaEstA = sumaEstA * (-1);
                }
                //++++++++++++++++++++++++++++++++++++++++++
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
        [CustomAuthorize(Roles = "000003,000253")]
        public ActionResult IndexGeneral(string menuArea, string menuVista, string fchEveSolGasI ="", string fchEveSolGasF="", string estado="", string solicitante="")
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
            //-----
            IEnumerable<SolicitudGastoModels> model = null;
            model = _sis.obtenerSolicitudesGeneral(ConstantesGlobales.mod_ventas, solicitante, estado, inicio.ToString("dd/MM/yyyy"),fin.ToString("dd/MM/yyyy")).ToList();
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
        [CustomAuthorize(Roles = "000003,000254")]
        public ActionResult VisualizarGeneral(string id)
        {
            var model = _sis.obtenerItem(id);
            return View(model);
        }
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000250")]
        public ActionResult Visualizar(string id)
        {
            var model = _sis.obtenerItem(id);
            return View(model);
        }
        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000370")]
        public JsonResult modificarEstado(string sol, string idNewEst, string idOldEst, string obs)
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
            if (movpres.Count() != 0)
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
                    if (idOldEst == ConstantesGlobales.estadoPreApro || idOldEst == ConstantesGlobales.estadoAprobado || idOldEst == ConstantesGlobales.estadoAtendido || idOldEst== ConstantesGlobales.estadoEnGiro)
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
                    if (idOldEst == ConstantesGlobales.estadoAtendido)
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
        //SOLGASMKT_REPORTE 000239
        [CustomAuthorize(Roles = "000003,000294")]
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
            var presAprob = _pre.obtenerPresupuesto().Where(x => x.idTipoPres == ConstantesGlobales.plan_Trab && (x.aprobador.idEst != ConstantesGlobales.estadoCesado)).Select(x => new { idAcc = x.idAccJ, nombre = x.aprobador.empleado.nomComEmp }).Distinct().ToList();
            var trilogia = _uzl.obtenerTrilogia();

            ViewBag.aprobador = new SelectList(presAprob, "idAcc", "nombre");
            ViewBag.lineas = new SelectList(trilogia.Where(x => x.idAcc == "").Select(x => new { x.linea.idLin, x.linea.nomLin }).Distinct(), "idLin", "nomLin");
            ViewBag.zonas = new SelectList(trilogia.Where(x => x.idAcc == "").Select(x => new { x.zona.idZon, x.zona.nomZon }).Distinct(), "idZon", "nomZon");
            //---
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //--
            return View();
        }
        [HttpPost]
        public JsonResult buscarLineaReporte(string idAcc)
        {
            var trilogia = _uzl.obtenerTrilogia();
            var lineas = trilogia.Where(x => x.idAcc == idAcc).Select(x => new { x.linea.idLin, x.linea.nomLin }).Distinct().ToList();
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            foreach (var v in lineas)
            {
                cbo.value = v.idLin;
                cbo.text = v.nomLin;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            return Json(cboList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarZonaReporte(string idAcc)
        {
            var trilogia = _uzl.obtenerTrilogia();
            var zonas = trilogia.Where(x => x.idAcc == idAcc).Select(x => new { x.zona.idZon, x.zona.nomZon }).Distinct().ToList();
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            foreach (var v in zonas)
            {
                cbo.value = v.idZon;
                cbo.text = v.nomZon;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            return Json(cboList, JsonRequestBehavior.AllowGet);
        }
        public string ReporteGeneral(string idAcc, string idLin , string idZon, string fchEveSolGasI , string fchEveSolGasF)
        {
            string rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "Export/Reportes/generarReporte1.xls";
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
                ps.Orientation = OrientationValues.Landscape;
                ps.View = SheetViewValues.PageBreakPreview;
                // 120% of normal page size
                ps.ScalePage(46);
                ps.ZoomScale = 70;
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

                //----------------------
                //------------------------------------------------------------------------
                DateTime p = DateTime.Parse(fchEveSolGasI); //desde
                DateTime a = DateTime.Parse(fchEveSolGasF).AddHours(23).AddMinutes(59);//hasta
                //------------------------------------------------------------------------
                //----------------------

                //*********************
                //var listDeGastos = _sis.obtenerSolicitudes().Where(x => x.idAccRes == (idAcc == "" ? x.idAccRes: idAcc ) && (x.idEst != ConstantesGlobales.estadoAnulado || x.idEst != ConstantesGlobales.estadoRechazado || x.idEst != ConstantesGlobales.estadoDTotal)).ToList();
                var presupuestos = _pre.obtenerPresupuesto().Where(x => x.idAccJ == (idAcc == "" ? x.idAccJ : idAcc) && (x.idLin == (idLin == "" ? x.idLin: idLin)) && (x.idZon == (idZon == "" ? x.idZon : idZon)) && ((x.fchIniVigencia >= p) && (x.fchFinVigencia <= a)) && (x.idTipoPres == ConstantesGlobales.plan_Trab) && (x.idEst != ConstantesGlobales.estadoInactivo)).ToList();
                //leer presupuestos inicio
                col = 1;

                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Major, 20);
                font.Underline = UnderlineValues.Double;
                rst = new SLRstType();
                rst.AppendText("Reporte de Gastos y Presupuesto - Ventas", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                style.Alignment.Vertical = VerticalAlignmentValues.Center;
                sl.SetCellStyle(fil, col, style);
                sl.MergeWorksheetCells(1, 1, 1, 9);
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
                        rst.AppendText("Responsable", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetColumnWidth(fil, col, 30);
                        col = 2;
                        sl.SetCellValue(fil, col, item.responsable.empleado.nomComEmp);
                        //(2)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);
                        rst = new SLRstType();
                        rst.AppendText("Zona: ", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.SetColumnWidth(fil, col, 30);
                        col = 2;
                        sl.SetCellValue(fil, col, item.zona.nomZon);
                        //(3)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);
                        rst = new SLRstType();
                        rst.AppendText("Linea", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        sl.SetCellValue(fil, col, item.linea.nomLin);
                        //(4)-------------------------
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
                        //(5)-------------------------
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
                        //(6)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Moneda", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        sl.SetCellValue(fil, col, item.moneda.nomMon);
                        //(7)-------------------------
                        fil = fil + 1;
                        col = 1;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 14);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Inversion", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 2;
                        style = sl.CreateStyle();
                        style.FormatCode = "#,##0.00";
                        style.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                        double inversion = item.Monto;
                        sl.SetCellValue(fil, col, item.Monto);
                        sl.SetCellStyle(fil, col, style);
                        //(8)-------------------------
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
                        //(9)-------------------------
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
                        //(10)-------------------------
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
                        sl.SetCellValue(fil, col, rst.ToInlineString());

                        col = 3;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Solicitante", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());

                        col = 4;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Titulo", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        sl.AutoFitColumn(col);

                        col = 5;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Concepto de Gasto", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());

                        col = 6;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("Fecha de Eve.", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());

                        col = 7;
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
                        sl.SetCellValue(fil, col, rst.ToInlineString());
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
                        sl.SetCellValue(fil, col, rst.ToInlineString());
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
                        //fil = fil + 1;
                        //----------------------------
                        var gastos = _mov.obtenerMovxPres(item.idPres);
                        //----------------------------
                        double sumaD = 0;
                        double sumaS = 0;
                        //----------------------------
                        foreach (var det in gastos)
                        {
                            //asignar valores a las variables
                            fil = fil + 1;
                            //---------------------------------------------------
                            col = 2;
                            sl.SetCellValue(fil, col, det.idSolGas);
                            //---------------------------------------------------
                            col = 3;
                            sl.SetCellValue(fil, col, det.solGasto.solicitante.empleado.nomComEmp);
                            //---------------------------------------------------
                            col = 4;
                            sl.SetCellValue(fil, col, det.solGasto.titSolGas);
                            //---------------------------------------------------
                            col = 5;
                            sl.SetCellValue(fil, col, det.solGasto.concepto.nomConGas);
                            //---------------------------------------------------
                            col = 6;
                            sl.SetCellValue(fil, col, det.solGasto.fchEveSolGas.ToString("dd-MM-yyyy"));
                            style = sl.CreateStyle();
                            style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                            sl.SetCellStyle(fil, col, style);
                            //---------------------------------------------------
                            //CONVERSIONES --------------------------------------
                            //---------------------------------------------------
                            double liqu = 0;
                            string estad = "";
                            if (det.solGasto.liquidacion != null)
                            {
                                liqu = det.solGasto.liquidacion.liqValRea;
                                estad = det.solGasto.liquidacion.idEst;
                                if (estad == ConstantesGlobales.estadoDParcial || estad == ConstantesGlobales.estadoDTotal)
                                {
                                    liqu = -liqu;
                                }
                            }
                            //---------------------------------------------------
                            double neto = (double)det.solGasto.monNeto + liqu;
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

                            //sumamos todas las variables 
                            sumaD = sumaD + conversionD;
                            sumaS = sumaS + conversionS;
                        }
                        //------------*************************----------------------
                        if(item.idMon == ConstantesGlobales.monedaDol)
                        {
                            consumo = sumaD;
                        }
                        else
                        {
                            consumo = sumaS;
                        }
                        
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
                        col = 7;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("S/.", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 8;
                        style = sl.CreateStyle();
                        style.SetFont(FontSchemeValues.Minor, 12);
                        style.Font.Bold = true;
                        style.FormatCode = "#,##0.00";
                        sl.SetCellValue(fil, col, sumaS);
                        sl.SetCellStyle(fil, col, style);
                        sl.SetColumnWidth(fil, col, 15);
                        col = 9;
                        font = new SLFont();
                        font.Bold = true;//negrita
                        font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                        rst = new SLRstType();
                        rst.AppendText("$", font);
                        sl.SetCellValue(fil, col, rst.ToInlineString());
                        col = 10;
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
                sl.SetCellStyle(1, 1, fil, 11, styleF);
                sl.SetPageSettings(ps);
                sl.AutoFitColumn(4, 9);
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
        public string ReporteResumen(string idAcc, string idLin, string idZon, string fchEveSolGasI, string fchEveSolGasF)
        {
            string rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "Export/Reportes/generarReporte2.xls";
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

                ps.CellComments = CellCommentsValues.AtEnd;
                ps.Errors = PrintErrorValues.NA;
                ps.PageOrder = PageOrderValues.OverThenDown;

                //----------------------
                //------------------------------------------------------------------------
                DateTime p = DateTime.Parse(fchEveSolGasI); //desde
                DateTime a = DateTime.Parse(fchEveSolGasF).AddHours(23).AddMinutes(59);//hasta
                //------------------------------------------------------------------------
                //----------------------

                //*********************
                var presupuestos = _pre.obtenerPresupuesto().Where(x => x.idAccJ == (idAcc == "" ? x.idAccJ : idAcc) && ((x.fchIniVigencia >= p) && (x.fchFinVigencia <= a)) && (x.idTipoPres == ConstantesGlobales.plan_Trab) && (x.idEst != ConstantesGlobales.estadoInactivo)).ToList();
                var usuPres = presupuestos.Select(x => new { idUsuJ = x.idAccJ, nomUsuJ = x.aprobador.empleado.nomComEmp}).Distinct().ToList();
                //var lineasPres = presupuestos.Select(x => new { idLinea = x.idLin, nomLinea = x.linea.nomLin }).Distinct().ToList();
                //var zonasPres = presupuestos.Select(x => new { idZona = x.idZon, nomZona = x.zona.nomZon }).Distinct().ToList();

                //leer presupuestos inicio
                col = 1;

                font = new SLFont();
                font.Bold = true;//negrita
                font.SetFont(FontSchemeValues.Major, 20);
                font.Underline = UnderlineValues.Double;
                rst = new SLRstType();
                rst.AppendText("Reporte Resumen de Presupuesto - Ventas", font);
                sl.SetCellValue(fil, col, rst.ToInlineString());
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                style.Alignment.Vertical = VerticalAlignmentValues.Center;
                sl.SetCellStyle(fil, col, style);
                sl.MergeWorksheetCells(1, 1, 1, 9);
                sl.SetRowHeight(1, 1, 40);
                //-----------------------------------------
                fil = fil + 1;
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
                    rst.AppendText("JEFE : ", font);
                    sl.SetCellValue(fil, col, rst.ToInlineString());
                    sl.SetColumnWidth(fil, col, 10);

                    col = 2;
                    style = sl.CreateStyle();
                    style.Font.Bold = true;
                    style.SetFontColor(System.Drawing.Color.Blue);
                    sl.SetCellValue(fil, col, usu.nomUsuJ);
                    sl.SetCellStyle(fil, col, style);

                    //-------------------------------
                    fil = fil + 1;
                    //-------------------------------
                    var lineasPres = presupuestos.Where(x=>x.idAccJ== usu.idUsuJ && (x.idLin == (idLin == "" ? x.idLin : idLin))).Select(x => new { idLinea = x.idLin, nomLinea = x.linea.nomLin }).Distinct().ToList();
                    var zonasPres = presupuestos.Where(x => x.idAccJ == usu.idUsuJ && (x.idZon == (idZon == "" ? x.idZon : idZon))).Select(x => new { idZona = x.idZon, nomZona = x.zona.nomZon }).Distinct().ToList();
                    //-------------------------------
                    //-------------------------------
                    foreach (var zon in zonasPres)
                    {
                        //-------------------------------
                        if (zon.idZona != ConstantesGlobales.zona_Lima)
                        {
                            //******************************
                            fil = fil + 1;
                            //(1.0)-------------------------

                            col = 2;
                            font = new SLFont();
                            font.Bold = true;//negrita
                            font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                            rst = new SLRstType();
                            rst.AppendText("ZONA : ", font);
                            sl.SetCellValue(fil, col, rst.ToInlineString() );

                            col = 3;
                            style = sl.CreateStyle();
                            style.Font.Bold = true;
                            style.Font.Underline = UnderlineValues.Double;
                            style.Font.FontSize = 14;
                            font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                            sl.SetCellValue(fil, col,  zon.nomZona);
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
                            rst.AppendText("Responsable", font);
                            sl.SetCellValue(fil, col, rst.ToInlineString());
                            //(1.2)-------------------------

                            col = 4;
                            font = new SLFont();
                            font.Bold = true;//negrita
                            font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                            rst = new SLRstType();
                            rst.AppendText("Zona / Linea ", font);
                            sl.SetCellValue(fil, col, rst.ToInlineString());
                            //(1.4)-------------------------

                            col = 5;
                            font = new SLFont();
                            font.Bold = true;//negrita
                            font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                            rst = new SLRstType();
                            rst.AppendText("Fec.Ini. ", font);
                            sl.SetCellValue(fil, col, rst.ToInlineString());
                            //(1.5)-------------------------

                            col = 6;
                            font = new SLFont();
                            font.Bold = true;//negrita
                            font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                            rst = new SLRstType();
                            rst.AppendText("Fec.Fin ", font);
                            sl.SetCellValue(fil, col, rst.ToInlineString());
                            //(1.6)-------------------------

                            col = 7;
                            font = new SLFont();
                            font.Bold = true;//negrita
                            font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                            rst = new SLRstType();
                            rst.AppendText("Moneda ", font);
                            sl.SetCellValue(fil, col, rst.ToInlineString());
                            //(1.7)-------------------------

                            col = 8;
                            font = new SLFont();
                            font.Bold = true;//negrita
                            font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                            rst = new SLRstType();
                            rst.AppendText("Inversion ", font);
                            sl.SetCellValue(fil, col, rst.ToInlineString());
                            //(1.8)-------------------------

                            col = 9;
                            font = new SLFont();
                            font.Bold = true;//negrita
                            font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                            rst = new SLRstType();
                            rst.AppendText("Saldo ", font);
                            sl.SetCellValue(fil, col, rst.ToInlineString());
                            //******************************
                            var listaPres = presupuestos.Where(x => x.idZon == zon.idZona).ToList();
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
                                sl.SetCellValue(fil, col, pre.responsable.empleado.nomComEmp);
                                //---------------------------------------------------
                                col = 4;
                                sl.SetCellValue(fil, col, pre.linea.nomLin);
                                //---------------------------------------------------
                                col = 5;
                                sl.SetCellValue(fil, col, pre.fchIniVigencia.ToString("dd-MM-yyyy"));
                                //---------------------------------------------------
                                col = 6;
                                sl.SetCellValue(fil, col, pre.fchFinVigencia.ToString("dd-MM-yyyy"));
                                //---------------------------------------------------
                                col = 7;
                                sl.SetCellValue(fil, col, pre.moneda.simbMon);
                                //---------------------------------------------------
                                col = 8;
                                style = sl.CreateStyle();
                                style.FormatCode = "#,##0.00";
                                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                                sl.SetCellValue(fil, col, pre.Monto);
                                sl.SetCellStyle(fil, col, style);
                                //---------------------------------------------------
                                col = 9;
                                style = sl.CreateStyle();
                                style.FormatCode = "#,##0.00";
                                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
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
                            col = 8;
                            style = sl.CreateStyle();
                            style.SetFont(FontSchemeValues.Minor, 12);
                            style.Font.Bold = true;
                            style.FormatCode = "#,##0.00";
                            sl.SetCellValue(fil, col, sumaT);
                            sl.SetCellStyle(fil, col, style);
                            sl.SetColumnWidth(fil, col, 15);
                            col = 9;
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
                            sl.SetCellStyle(fil, 2, fil, 9, style3);
                            //------------
                            fil = fil + 2;
                        }
                        else
                        {
                            foreach(var lin in lineasPres)
                            {
                                //******************************
                                var listaPres = presupuestos.Where(x => x.idLin == lin.idLinea && x.idZon == zon.idZona).ToList();
                                //******************************
                                if (listaPres.Count != 0)
                                {
                                
                                //******************************
                                fil = fil + 1;
                                //(1.0)-------------------------

                                col = 2;
                                font = new SLFont();
                                font.Bold = true;//negrita
                                font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                                rst = new SLRstType();
                                rst.AppendText("LINEA : ", font);
                                sl.SetCellValue(fil, col, rst.ToInlineString());

                                col = 3;
                                style = sl.CreateStyle();
                                style.Font.Bold = true;
                                style.Font.Underline = UnderlineValues.Double;
                                style.Font.FontSize = 14;
                                font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                                sl.SetCellValue(fil, col, lin.nomLinea);
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
                                rst.AppendText("Responsable", font);
                                sl.SetCellValue(fil, col, rst.ToInlineString());
                                //(1.2)-------------------------

                                col = 4;
                                font = new SLFont();
                                font.Bold = true;//negrita
                                font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                                rst = new SLRstType();
                                rst.AppendText("Zona / Linea ", font);
                                sl.SetCellValue(fil, col, rst.ToInlineString());
                                //(1.4)-------------------------

                                col = 5;
                                font = new SLFont();
                                font.Bold = true;//negrita
                                font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                                rst = new SLRstType();
                                rst.AppendText("Fec.Ini. ", font);
                                sl.SetCellValue(fil, col, rst.ToInlineString());
                                //(1.5)-------------------------

                                col = 6;
                                font = new SLFont();
                                font.Bold = true;//negrita
                                font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                                rst = new SLRstType();
                                rst.AppendText("Fec.Fin ", font);
                                sl.SetCellValue(fil, col, rst.ToInlineString());
                                //(1.6)-------------------------

                                col = 7;
                                font = new SLFont();
                                font.Bold = true;//negrita
                                font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                                rst = new SLRstType();
                                rst.AppendText("Moneda ", font);
                                sl.SetCellValue(fil, col, rst.ToInlineString());
                                //(1.7)-------------------------

                                col = 8;
                                font = new SLFont();
                                font.Bold = true;//negrita
                                font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                                rst = new SLRstType();
                                rst.AppendText("Inversion ", font);
                                sl.SetCellValue(fil, col, rst.ToInlineString());
                                //(1.8)-------------------------

                                col = 9;
                                font = new SLFont();
                                font.Bold = true;//negrita
                                font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                                rst = new SLRstType();
                                rst.AppendText("Saldo ", font);
                                sl.SetCellValue(fil, col, rst.ToInlineString());
                                //*****************************************
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
                                        sl.SetCellValue(fil, col, pre.responsable.empleado.nomComEmp);
                                        //---------------------------------------------------
                                        col = 4;
                                        sl.SetCellValue(fil, col, pre.zona.nomZon);
                                        //---------------------------------------------------
                                        col = 5;
                                        sl.SetCellValue(fil, col, pre.fchIniVigencia.ToString("dd-MM-yyyy"));
                                        //---------------------------------------------------
                                        col = 6;
                                        sl.SetCellValue(fil, col, pre.fchFinVigencia.ToString("dd-MM-yyyy"));
                                        //---------------------------------------------------
                                        col = 7;
                                        sl.SetCellValue(fil, col, pre.moneda.simbMon);
                                        //---------------------------------------------------
                                        col = 8;
                                        style = sl.CreateStyle();
                                        style.FormatCode = "#,##0.00";
                                        style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                                        sl.SetCellValue(fil, col, pre.Monto);
                                        sl.SetCellStyle(fil, col, style);
                                        //---------------------------------------------------
                                        col = 9;
                                        style = sl.CreateStyle();
                                        style.FormatCode = "#,##0.00";
                                        style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
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
                                    //-----***************************************************-----------
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
                                    col = 7;
                                    font = new SLFont();
                                    font.Bold = true;//negrita
                                    font.SetFont(FontSchemeValues.Minor, 12);//tamaño de letra
                                    rst = new SLRstType();
                                    rst.AppendText("S/.", font);
                                    sl.SetCellValue(fil, col, rst.ToInlineString());
                                    col = 8;
                                    style = sl.CreateStyle();
                                    style.SetFont(FontSchemeValues.Minor, 12);
                                    style.Font.Bold = true;
                                    style.FormatCode = "#,##0.00";
                                    sl.SetCellValue(fil, col, sumaT);
                                    sl.SetCellStyle(fil, col, style);
                                    sl.SetColumnWidth(fil, col, 15);
                                    col = 9;
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
                                    sl.SetCellStyle(fil, 2, fil, 9, style3);
                                    //------------
                                    fil = fil + 2;
                                }
                            }
                            
                        }
                    }
                }
                //-----------------------------------------
                SLStyle styleF = sl.CreateStyle();
                styleF.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                styleF.Border.RemoveAllBorders();
                sl.SetCellStyle(1, 1, fil, 9, styleF);
                sl.SetPageSettings(ps);
                sl.AutoFitColumn(3, 9);
                //-----------------------------------------
                sl.SaveAs(rutaArchivoCompleta);
                //-----------------------------------------
            }
            catch (Exception ex)
            {

            }
            return rutaArchivoCompleta;
        }
        public FileResult Reporte_1(string idRep, string idAcc, string idLin, string idZon, string fchEveSolGasI, string fchEveSolGasF)
        {
            //ruta donde esta la aplicacion y se guarda en la carpte  reportes
            string rutaArchivoCompleta = "";
            try
            {
                switch (idRep)
                {
                    case "1":
                        rutaArchivoCompleta = ReporteGeneral(idAcc, idLin, idZon, fchEveSolGasI, fchEveSolGasF);
                        break;
                    case "2":
                        rutaArchivoCompleta = ReporteResumen(idAcc, idLin, idZon, fchEveSolGasI, fchEveSolGasF);
                        break;
                }
                return File(rutaArchivoCompleta, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "Export/Reportes/reporteVacio.xls";
                return File(rutaArchivoCompleta, "application/vnd.ms-excel");
            }

        }

    }
}