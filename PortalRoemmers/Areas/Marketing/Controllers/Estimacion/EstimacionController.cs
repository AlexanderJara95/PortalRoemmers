using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PortalRoemmers.Helpers;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Security;
using System.Data;
using PortalRoemmers.Areas.Marketing.Services.Estimacion;
using PortalRoemmers.Areas.Marketing.Models.Estimacion;
using PortalRoemmers.Areas.Marketing.Models.Actividad;
using PortalRoemmers.Areas.Marketing.Services.Actividad;
using PortalRoemmers.Areas.Sistemas.Services.Visitador;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Presupuesto;
using PortalRoemmers.Areas.Sistemas.Services.Gasto;
using PortalRoemmers.Areas.Ventas.Services.SolicitudGasto;
using PortalRoemmers.Areas.Sistemas.Services.Trilogia;
using PortalRoemmers.Filters;
using MvcRazorToPdf;
using iTextSharp.text;


namespace PortalRoemmers.Areas.Marketing.Controllers
{
    //AREA ROL 000229
    public class EstimacionController : Controller
    {//ESTIMACION_CONTROLLER 000241
        private EstimacionRepositorio _est;
        private UsuarioRepositorio _usu;
        private EspecialidadRepositorio _esp;
        private ActividadRepositorio _act;
        private LineaRepositorio _lin;
        private MonedaRepositorio _mon;
        private PresupuestoRepositorio _pre;
        private TipGastDeActivRepositorio _tipGasAct;
        private ActividadGastoRepositorio _actGast;
        private TipoCambioRepositorio _tcam;
        private MovimientoPresRepositorio _mov;
        private Esp_Usu_Repositorio _esp_usu;
        private SolicitudGastoRepositorio _solGas;
        private Usu_Zon_LinRepositorio _uzl;
        private Pro_Lin_Repositorio _pl;
        private TipoGastoRepositorio _tipGas;
        private ConceptoGastoRepositorio _cgas;
        private TipoGasto_UsuRepositorio _tipU;

        Ennumerador enu = new Ennumerador();
        public EstimacionController()
        {
            _est = new EstimacionRepositorio();
            _act = new ActividadRepositorio();
            _lin = new LineaRepositorio();
            _usu = new UsuarioRepositorio();
            _esp = new EspecialidadRepositorio();
            _mon = new MonedaRepositorio();
            _tcam = new TipoCambioRepositorio();
            _pre = new PresupuestoRepositorio();
            _tipGasAct = new TipGastDeActivRepositorio();
            _actGast = new ActividadGastoRepositorio();
            _mov = new MovimientoPresRepositorio();
            _esp_usu = new Esp_Usu_Repositorio();
            _solGas = new SolicitudGastoRepositorio();
            _uzl = new Usu_Zon_LinRepositorio();
            _pl = new Pro_Lin_Repositorio();
            _tipGas = new TipoGastoRepositorio();
            _cgas = new ConceptoGastoRepositorio();
            _tipU = new TipoGasto_UsuRepositorio();
        }
        //ESTIMACION_LISTAR - BUSQUEDA 000242
        //=============================================
        [CustomAuthorize(Roles = "000003,000242")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "", string fchEveSolGasI="", string fchEveSolGasF="")
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
                DateTime oUltimoDiaDelMes = new DateTime(date.Year, 12, 31);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = oUltimoDiaDelMes.ToString("dd/MM/yyyy");
                //var actual = DateTime.Today.ToString("dd/MM/yyyy");
                inicio = DateTime.Parse(primero);
                fin = DateTime.Parse(actual);
            }
            //-----------------------------
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //-----------------------------
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _est.obtenerTodos(pagina, search, inicio.ToString(), fin.ToString());
            ViewBag.search = search;
            return View(model);
        }
        //ESTIMACION_REGISTRAR 000243
        //=============================================
        //registrar
        [CustomAuthorize(Roles = "000003,000243")]
        [HttpGet]
        public ActionResult Registrar()
        {
            EstimacionModels model = new EstimacionModels();
            try
            {
                string gastoD = "";
                string familiaD = "";
                //----------------------
                cargarCombos(model);
                creaTablas("0",gastoD, familiaD);
                ViewBag.actual = double.Parse(_tcam.obtenerItem(DateTime.Today).monTCVenta.ToString());
                return View();
            }
            catch (Exception e) { e.Message.ToString();
                TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe ingresar el tipo de cambio ..." + "</div>";
                return View(model);
            }
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(EstimacionModels model, string gastoD, string familiaD)
        {
            ModelState.Count();
            if (ModelState.IsValid)
            {
                if (!model.procede)//tiene presupuesto
                {
                    if (model.idPres != null)
                    {
                        var pre = _pre.obtenerUnPresupuesto(model.idPres);
                        double tp = 0;//total presupuesto
                                      //sumo los presupuesto ,en caso sea diferente lo pongo a la moneda de la solicitud
                        if (model.idMon == pre.idMon)
                        {
                            tp = tp + pre.Estim;
                        }
                        else
                        {
                            //presupuesto en soles con moneda en dolares
                            if (pre.idMon == ConstantesGlobales.monedaDol)
                            {
                                tp = tp + Math.Round(pre.Estim * model.valtipCam, 2);
                            }
                            if (pre.idMon == ConstantesGlobales.monedaSol)
                            {
                                tp = tp + Math.Round(pre.Estim / model.valtipCam, 2);
                            }
                        }
                        //------------------------------------
                        //diferencia = ant.monEstGas - model.monEstGas;
                        tp = tp - model.monEstGas;
                        //************************************
                        //------------------------------------
                        //valido el presupuesto y la solicitud
                        if (tp >= 0)
                        {//realizo toda la logica para descontar
                            if (guardarEstimacionYdetalles(model, gastoD, familiaD))
                            {
                                //Calculamos el saldo actual en la moneda origen del Pto Estimado
                                if (model.idMon != pre.idMon)
                                {
                                    //presupuesto en soles con moneda en dolares
                                    if (pre.idMon == ConstantesGlobales.monedaDol)
                                    {
                                        tp = Math.Round(tp * model.valtipCam, 2);
                                    }
                                    if (pre.idMon == ConstantesGlobales.monedaSol)
                                    {
                                        tp = Math.Round(tp / model.valtipCam, 2);
                                    }
                                }
                                string mensaje = "";
                                ActualizarPresupuestoSegunEstimacion(model, tp, out mensaje);//todo salio bien al guardar detalle ,mensaje final
                                TempData["mensaje"] = mensaje;
                            }
                        }//fin de la logica para descontar
                        else
                        {
                            cargarCombos(model);
                            creaTablas("0", gastoD, familiaD);
                            TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "No cuenta con suficiente saldo" + "</div>";
                            return View(model);
                        }
                    }
                    else
                    {
                        cargarCombos(model);
                        creaTablas("0", gastoD, familiaD);
                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe elegir un presupuesto" + "</div>";
                        return View(model);
                    }
                }
                else
                {
                    if (guardarEstimacionYdetalles(model, gastoD, familiaD))
                    {
                        //DETALLES////////////////////////////////////////////////////////////////////////////////////////////
                        TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                    }
                    else
                    {
                        TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error en guardar detalle" + "</div>";
                    }
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Debe llenar todos los campos." + "</div>";
            }
            try
            {
                ViewBag.actual = double.Parse(_tcam.obtenerItem(DateTime.Today).monTCVenta.ToString());
            }
            catch (Exception e)
            { e.Message.ToString(); }
            cargarCombos(model);
            creaTablas("0", gastoD, familiaD);
            return View(model);
        }
        //ESTIMACION_MODIFICAR 000244
        //=============================================
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000244")]
        public ActionResult Modificar(string id, string modulo)
        {
            string gastoD = "";
            string familiaD = "";
            var model = _est.obtenerItem(id);
            //Logica Hidden
            //<!--------------------------
            var gdetalle = model.detalleEstim_Gas.ToList();
            if (gdetalle.Count() != 0)
            {
                var index = 0;
                foreach (var d in gdetalle)
                {
                    if (gastoD != "")
                    {
                        gastoD += "|";
                    }
                    index += 1;
                    gastoD += ";" + index + ";" + d.gastoActiv.tipoGastoActividad.idTipGasAct + ";" + d.idActGas + ";" + d.gastoActiv.tipoGastoActividad.nomTipGasAct + ";" + d.gastoActiv.nomActGas + ";" + Math.Round(d.monTotal / d.monProm,0) + ";" + d.monProm + ";" + d.monTotal + ";" + d.salReal + ";" + d.idActiv;
                }
            }
            //<!--------------------------
            var fdetalle = model.detalleEstim_Fam.ToList();
            if (fdetalle.Count() != 0)
            {
                foreach (var d in fdetalle)
                {
                    if (familiaD != "")
                    {
                        familiaD += "|";
                    }
                    familiaD += d.idFamRoe + ";" + d.familia.nomFamRoe + ";" + d.areaTerap.numAreaTerap + ";"+d.areaTerap.idAreaTerap;
                }
            }
            //<!--------------------------
            model.fchModActiv = DateTime.UtcNow;
            model.userModActiv = SessionPersister.Username;
            cargarCombos(model);
            creaTablas("1", gastoD, familiaD);
            TempData["Modular"] = modulo;
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(EstimacionModels model, string gastoD, string familiaD)
        {
            string mensaje = "";
            if (ModelState.IsValid)
            {
                if (model.idPres != null)
                {
                    var pre = _pre.obtenerUnPresupuesto(model.idPres);
                    var ant = _est.obtenerItem(model.idActiv);
                    //---*****************************************---
                    double tp = 0;//total presupuesto
                                  //sumo los presupuesto ,en caso sea diferente lo pongo a la moneda de la estimacion
                    double diferencia = 0;
                    if (model.idMon == pre.idMon)
                    {
                        tp = tp + pre.Estim;
                    }
                    else
                    {
                        //presupuesto en soles con moneda en dolares
                        if (pre.idMon == ConstantesGlobales.monedaDol)
                        {
                            tp = tp + Math.Round(pre.Estim * model.valtipCam, 2);
                        }
                        if (pre.idMon == ConstantesGlobales.monedaSol)
                        {
                            tp = tp + Math.Round(pre.Estim / model.valtipCam, 2);
                        }
                    }
                    //------------------------------------
                    diferencia = ant.monEstGas - model.monEstGas;
                    tp = tp + diferencia;
                    //************************************
                    //------------------------------------
                    //valido el presupuesto y la solicitud
                    /*if (tp >= 0)
                    {//realizo toda la logica para descontar*/
                        //Calculamos el saldo actual en la moneda origen del Pto Estimado
                        if (model.idMon != pre.idMon)
                        { 
                            //presupuesto en soles con moneda en dolares
                            if (pre.idMon == ConstantesGlobales.monedaDol)
                            {
                                tp = Math.Round(tp * model.valtipCam, 2);
                            }
                            if (pre.idMon == ConstantesGlobales.monedaSol)
                            {
                                tp = Math.Round(tp / model.valtipCam, 2);
                            }
                        }
                        //Modificar el detalle de gastos de la estimacion
                        string msn = "";
                        if (modificarEstimacionYdetalles(model, gastoD, familiaD, out msn))
                        {
                            //Actualizar el saldo del pto. estimado
                            ActualizarPresupuestoSegunEstimacion( model, tp, out mensaje);//todo salio bien al guardar detalle,mensaje final
                        }
                        TempData["mensaje"] = "<div id='success' class='alert alert-success'>" + msn + mensaje + "</div>";

                    /*}//fin de la logica para descontar
                    else
                    {
                        cargarCombos(model);
                        creaTablas("1", gastoD, familiaD);
                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "No cuenta con suficiente saldo" + "</div>";
                        return View(model);
                    }*/
                    
                }
                else
                {
                    cargarCombos(model);
                    creaTablas("1", gastoD, familiaD);
                    TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Debe elegir un presupuesto" + "</div>";
                    return View(model);
                }
                //---------------
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
                //---------------
            }
            //------En caso no cargue-------
            model = _est.obtenerItem(model.idActiv);
            //Logica Hidden
            model.fchModActiv = DateTime.UtcNow;
            model.userModActiv = SessionPersister.Username;
            cargarCombos(model);
            creaTablas("1", gastoD, familiaD);
            return View(model);
            //------------------------------
        }
        //ESTIMACION_ANULAR 000245
        //=============================================
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000245")]
        public ActionResult Eliminar(string id)
        {
            var model = _est.obtenerItem(id);
            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult Eliminar(EstimacionModels model)
        {
            var pre = _pre.obtenerItem(model.idPres);
            try
            {
                TempData["mensaje"] = _est.eliminar(model.idActiv);
                //---------------------------------------------
                //Si elimina la estimacion 
                //realizo la actualizacion
                double actualizado = 0;
                actualizado = pre.Estim + model.monEstGas;
                //---------------------------------------------
                if (!_mov.updateSaldoPresEstim(model.idPres, actualizado))
                {
                    TempData["mensaje1"] = "<div id='danger' class='alert alert-danger'>" + "Error al modificar saldo presupuesto. " + "</div>";
                }
            }
            catch (Exception e)
            {
                TempData["mensaje"] = "<div id='danger' class='alert alert-danger'>" + "Error al anular estimacion. Entida Relacionada. " + "</div>";
            }
            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
        //ESTIMACION_IMPRIMIR 000246
        //=============================================
        [CustomAuthorize(Roles = "000003,000246")]
        [HttpGet]
        public ActionResult Imprimir(Boolean html, string est)
        {
            var estimacion = _est.obtenerItem(est);
            ViewBag.pdf = html;
            return new PdfActionResult(estimacion)
            {
                FileDownloadName = "Estimacion - " + estimacion.idActiv.ToString() + " - " + estimacion.actividad.nomActiv + ".pdf"
            };
        }
        //=============================================
        //=============================================
        public Boolean ActualizarPresupuestoSegunEstimacion(EstimacionModels model, double SaldoActual,out string result)
        {
            var pre = _pre.obtenerUnPresupuesto(model.idPres);
            //-------------*******************---------------
            if (!_mov.updateSaldoPresEstim(pre.idPres, SaldoActual))
            {
                result = "Error al modificar saldo presupuesto. ";
                return false;
            }
            //-------------*******************----------------
            result = "Descuento exitoso del presupuesto. ";
            return true;
        }
        public Boolean modificarEstimacionYdetalles(EstimacionModels model, string gastoD, string familiaD, out string result)
        {
            model.userModActiv = SessionPersister.Username;
            model.fchModActiv = DateTime.Today;
            var ant = _est.obtenerItem(model.idActiv);
            double SaldoAnterior = ant.monEstGas;
            var resultado = _est.modificar(model);
            if (resultado=="exito")
            {
                //DETALLES////////////////////////////////////////////////////////////////////////////////////////////
                var r = listaDetalle(gastoD);
                var f = listaDetalle(familiaD);

                //gastos    
                List<DetEstim_GastActModels> listGas = new List<DetEstim_GastActModels>();
                DetEstim_GastActModels gas = new DetEstim_GastActModels();

                if (gastoD != "")
                {
                    foreach (var i in r)
                    {
                        string[] item = i.Split(';');
                        gas.idActiv = model.idActiv;
                        gas.idActGas = item[3];
                        gas.monProm = Double.Parse(item[7]);
                        gas.monTotal = Double.Parse(item[8]);
                        gas.salReal = Double.Parse(item[9]);
                        gas.userCreEstGast = model.userCreActiv;
                        gas.fchCreEstGast = model.fchCreActiv;
                        gas.userModEstGast = SessionPersister.Username;
                        gas.fchModEstGast = DateTime.Today;
                        listGas.Add(gas);
                        gas = new DetEstim_GastActModels();
                    }
                }
                //familia
                List<DetEstim_FamProdModels> listFam = new List<DetEstim_FamProdModels>();
                DetEstim_FamProdModels fam = new DetEstim_FamProdModels();
                if (familiaD != "")
                {
                    foreach (var i in f)
                    {
                        string[] item = i.Split(';');
                        fam.idActiv = model.idActiv;
                        fam.idFamRoe = item[0];
                        fam.idAreaTerap = item[3];
                        fam.userCreEstFam = model.userCreActiv;
                        fam.fchCreEstFam = model.fchCreActiv;
                        fam.userModEstFam = SessionPersister.Username;
                        fam.fchModEstFam = DateTime.Today;
                        listFam.Add(fam);
                        fam = new DetEstim_FamProdModels();
                    }
                }
                //guadamos detalles-------------------------------------------------
                bool rpt1 = _est.modificaDetalleGastosEstim(listGas, model.idActiv);
                bool rpt2 = _est.modificaDetalleFamEstim(listFam, model.idActiv);
                //-------------------------------------------------------------------
                if (rpt1 & rpt2)
                {//detalle fue generado bien
                    result = "Se modificó el registro.";
                    return true;
                }
                else
                {//detalle fue generado mal
                    result = "Error al modificar detalle del registro. ";
                    return false;
                }
            }
            else
            {//cabecera fue generado mal
                result = "Error al modificar cabecera del registro. ";
                return false;
            }
        }
        public Boolean guardarEstimacionYdetalles(EstimacionModels model, string gastoD, string familiaD)
        {
            model.userCreActiv = SessionPersister.Username;
            model.fchCreActiv = DateTime.Today;
            var resultado = _est.crear(model);

            if (resultado)
            {
                //DETALLES////////////////////////////////////////////////////////////////////////////////////////////
                var r = listaDetalle(gastoD);
                var f = listaDetalle(familiaD);
                //gastos    
                List<DetEstim_GastActModels> listGas = new List<DetEstim_GastActModels>();
                DetEstim_GastActModels gas = new DetEstim_GastActModels();
                if (gastoD != "")
                {
                    foreach (var i in r)
                    {
                        string[] item = i.Split(';');
                        gas.idActiv = model.idActiv;
                        gas.idActGas = item[3];
                        gas.monProm = Double.Parse(item[7]);
                        gas.monTotal = Double.Parse(item[8]);
                        gas.salReal = Double.Parse(item[8]);
                        gas.userCreEstGast = SessionPersister.Username;
                        gas.fchCreEstGast = DateTime.Today;
                        listGas.Add(gas);
                        gas = new DetEstim_GastActModels();
                    }
                }
                //familia
                List<DetEstim_FamProdModels> listFam = new List<DetEstim_FamProdModels>();
                DetEstim_FamProdModels fam = new DetEstim_FamProdModels();
                if (familiaD != "")
                {
                    foreach (var i in f)
                    {
                        string[] item = i.Split(';');
                        fam.idActiv = model.idActiv;
                        fam.idFamRoe = item[0];
                        fam.idAreaTerap = item[3];
                        fam.userCreEstFam = SessionPersister.Username;
                        fam.fchCreEstFam = DateTime.Today;
                        listFam.Add(fam);
                        fam = new DetEstim_FamProdModels();
                    }
                }
                //guadamos detalles
                var rpt1 = _est.crearDetalleG(listGas);
                var rpt2 = _est.crearDetalleF(listFam);

                if (rpt1 & rpt2)
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
        public void cargarCombos(EstimacionModels model)
        {
            Parametros p = new Parametros();
            var parametro = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).ToList();
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp }).ToList();
            
            //envio los combos ya cargados
            try
            {
                ActividadModels activ = new ActividadModels();
                activ = model.actividad;
                var idAccRess = "";
                if (activ != null)
                {
                    ViewBag.responsable = new SelectList(result.Select(x => new { idAccRes = x.value, nombre = x.nomComEmp }), "idAccRes", "nombre", activ.idAccRes);
                    idAccRess = model.actividad.idAccRes;
                }
                else
                { 
                    ViewBag.responsable = new SelectList(result.Select(x => new { idAccRes = x.value, nombre = x.nomComEmp }), "idAccRes", "nombre");
                    idAccRess = model.idAccRes;
                }
                //----

                ViewBag.especialidad = new SelectList(selectEspecialidad(model.idAccRes), "value", "text", model.idAccRes);
                ViewBag.actividad = new SelectList(selectActividad(model.idEsp,model.idAccRes), "value", "text",model.idActiv);
                //-----
                ViewBag.linea = new SelectList(_lin.obtenerLineas(), "idLin", "nomLin", model.idLin);
                //------
                var tipGasU = _tipU.obtenerTipoDeGastoXusuario(idAccRess).ToList();
                var tipGas = _tipGas.obtenerTipGasto().ToList();
                var listTipGas = tipGasU.Join(tipGas, e => e.idTipGas, d => d.idTipGas, (e, d) => new { e.idTipGas, d.nomTipGas }).ToList();
                //------
                ViewBag.tipgasto = new SelectList(listTipGas.Select(x => new { idTipGas = x.idTipGas, nomTipGas = x.nomTipGas }).Distinct(), "idTipGas", "nomTipGas", model.idTipGas);
                ViewBag.congasto = new SelectList(_cgas.obtenerConceptoGastosxTipoDeGasto(model.idTipGas).Distinct(), "idConGas", "nomConGas", model.idConGas);
                //----
                ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "abrMon", model.idMon);
                ViewBag.presupuesto = new SelectList(_pre.obtenerListaSeleccion(ConstantesGlobales.plan_Mark, model.idAccRes,"","", "",model.idTipGas,model.idConGas, model.idEsp).Select(x => new { codigo = x.idPres, nombre = "N°: " + x.idPres + " | " + "Vencimiento: " + x.fchFinVigencia.ToShortDateString() + " | " + "Importe: " + x.moneda.simbMon + " " + x.Monto + " | " + "Saldo: " + x.moneda.simbMon + " " + x.Saldo }), "codigo", "nombre", model.idPres);
                ViewBag.tipoDeGastoAct = new SelectList(_tipGasAct.obtenerTipGastoAct(), "idTipGasAct", "nomTipGasAct");
                ViewBag.GastoAct = new SelectList(_actGast.obtenerActividadGastosPorTipo(model.idTipGasAct), "idActGas", "nomActGas"); ;
            }
            catch (Exception ex){
                string msj = ex.Message;
            }
        }
        public List<SelectedModels> selectLinea(string idAccRes)
        {
            //var i = idAccRes.Split('-');
            //var res = i[0];
            var linea = _lin.obtenerLineas().Select(x => new { x.idLin, x.nomLin });
            /*if (idAccRes != null)
            {
                Parametros p = new Parametros();
                var responsable = _usu.obtenerUsuarios().Where(z => z.idAcc == idAccRes).FirstOrDefault();

                //var linea = _lin.obtenerLineas().Select(x => new { x.idLin, x.nomLin });
                if (p.Resultado(ConstantesGlobales.Com_SolGas_Reg_Cas_01).Contains(responsable.empleado.idCarg))
                {
                    var trilogia = _uzl.obtenerTrilogia();
                    linea = trilogia.Where(x => x.idAcc == idAccRes).Select(x => new { x.linea.idLin, x.linea.nomLin }).Distinct();
                }
                else if (p.Resultado(ConstantesGlobales.Com_SolGas_Reg_Cas_02).Contains(responsable.empleado.idCarg))
                {
                    linea = _pl.obtenerLinZonxUsu(responsable.idEmp).Select(x => new { x.linea.idLin, x.linea.nomLin }).Distinct();
                }
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
        public void creaTablas(string Opc, string gastoD, string familiaD)
        {
            string rowsG = "";
            string rowsF = "";

            //armo tabla tb_gastoA
            if (gastoD != "")
            {
                var f = listaDetalle(gastoD);
                foreach (var i in f)
                {
                    string[] item = i.Split(';');
                    //Validamos si el item del gasto existe relacionado a una solicitud de gastos
                    //------
                    int contar = item.Count();
                    int cant = 0;
                    if (contar==11)
                    {
                        cant = _est.obtenerDetalleSolGas(item[10]).Where(x => x.idActGas == item[3]).Select(x => x.idSolGas).Distinct().Count();
                    }
                    //------
                    rowsG += "<tr>";
                    rowsG += "<td class='hidden'>" + item[0] + "</td>";
                    rowsG += "<td class='text-center'>" + item[1] + "</td>";
                    rowsG += "<td class='hidden'>" + item[2] + "</td>";
                    rowsG += "<td class='hidden'>" + item[3] + "</td>";
                    rowsG += "<td class='text-center'>" + item[4] + "</td>";
                    rowsG += "<td class='text-center'>" + item[5] + "</td>";
                    rowsG += "<td class='text-center'>" + item[6] + "</td>";
                    rowsG += "<td class='text-center'>" + item[7] + "</td>";
                    rowsG += "<td class='text-center'>" + item[8] + "</td>";
                    if (cant == 0)
                    {
                        rowsG += "<td class='delete' onclick='ActualizarIdFila(event)'><span class='glyphicon glyphicon-remove'></span></td>";
                        if (Opc =="1")
                        { 
                            rowsG += "<td class='editarPopUp text-nowrap' onclick='ActualizarIdEdit(event)'><a data-toggle='modal' title='Editar' href='#responsive-modal-gasto-mof'><i class='fa fa-pencil text-inverse m-r-10'></i></a></td>";
                            rowsG += "<td class='hidden'>" + item[9] + "</td>";
                        }
                    }
                    else
                    {
                        rowsG += "<td><span class='glyphicon glyphicon-remove hidden'></span></td>";
                        if (Opc == "1")
                        {
                            rowsG += "<td ><a data-toggle='modal' title='Editar' href='#'><i class='fa fa-pencil text-inverse m-r-10 hidden'></i></a></td>";
                            rowsG += "<td class='hidden'>" + item[9] + "</td>";
                        }
                    }
                    rowsG += "</tr>";
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
                    rowsF += "<td class='elimfila'><span class='glyphicon glyphicon-remove'></span></td>";
                    rowsF += "</tr>";
                }
            }

            ViewBag.tb_gastoA = rowsG;
            ViewBag.tb_familiaP = rowsF;
        }
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
        public List<SelectedModels> selectTipoDeGasto(string idAccRes)
        {
            var tipoDeGasto = _tipU.obtenerTipoDeGastoXusuario(idAccRes).Select(x => new { x.idTipGas, x.tiposDeGastos.nomTipGas }).Distinct();
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
        public List<SelectedModels> selectActividad(string idEsp,string idAccRes)
        {
            //-----------------------------
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();
            //-----------------------------
            DateTime date = DateTime.Now;
            DateTime oPrimerDiaDelMes = new DateTime(date.Year, 1, 1);
            DateTime oUltimoDiaDelMes = new DateTime(date.Year, 12, 31);
            var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
            var actual = oUltimoDiaDelMes.ToString("dd/MM/yyyy");
            inicio = DateTime.Parse(primero);
            fin = DateTime.Parse(actual);
            //-----------------------------
            var actividad = _act.obtenerActividades().Where(x=>x.idEsp == idEsp && x.idAccRes == idAccRes && ((x.fchIniVig >= inicio) && (x.fchFinVig <= fin))).Select(x => new { x.idActiv, x.nomActiv }).Distinct();
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            foreach (var v in actividad)
            {
                cbo.value = v.idActiv;
                cbo.text = v.nomActiv;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            return cboList;
        }
        public List<SelectedModels> selectGastoActividad(string idTipGasActEsp)
        {
            var gastoAct = _actGast.obtenerActividadGastos().Where(x => x.idTipGasAct == idTipGasActEsp).Select(x => new { x.idActGas, x.nomActGas }).Distinct();
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            foreach (var v in gastoAct)
            {
                cbo.value = v.idActGas;
                cbo.text = v.nomActGas;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            return cboList;
        }
        [HttpPost]
        public JsonResult verificarPresupuestoMarketing(string resp, string idEsp , string idTipGas, string idConGas)
        {
            Parametros p = new Parametros();
            if (p.Resultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).Contains(resp))//verificar si va tener presupuesto-marketing
            {
                var obtener = _pre.obtenerListaSeleccion(ConstantesGlobales.plan_Mark, resp, "", "","" , idTipGas, idConGas, idEsp);
                if (obtener.Count() != 0)
                {
                    return Json(obtener.Select(x => new { codigo = x.idPres, nombre = "N°: " + x.idPres + " | " + "Vencimiento: " + x.fchFinVigencia.ToShortDateString() + " | " + "Importe: " + x.moneda.simbMon + " " + x.Monto + " | " + "Saldo: " + x.moneda.simbMon + " " + x.Estim }), JsonRequestBehavior.AllowGet);
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
        //Reutilizables
        public string[] listaDetalle(string info)
        {
            string[] detalle = info.Split('|');
            return detalle;
        }
        [HttpPost]
        public JsonResult buscarEspecialidad(string idAccRes)
        {
            return Json(selectEspecialidad(idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarLinea(string idAccRes)
        {
            return Json(selectLinea(idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarTipoDeGasto(string idAccRes)
        {
            return Json(selectTipoDeGasto(idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarActividad(string idEsp , string idAccRes)
        {
            return Json(selectActividad(idEsp,idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarGastoActividad(string idTipGasAct)
        {
            return Json(selectGastoActividad(idTipGasAct), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult obtenerSolGas(string id)
        {
            var gastos = _solGas.obtenerDetSolGastosxEst().Where(x => x.idActiv == id && (x.solicitud.idEst != ConstantesGlobales.estadoAnulado && x.solicitud.idEst != ConstantesGlobales.estadoRechazado && x.solicitud.idEst != ConstantesGlobales.estadoNoAtendido && x.solicitud.idEst != ConstantesGlobales.estadoLiquidado)).Select(x => new { cod = x.idSolGas, tit = (x.solicitud.titSolGas.Length > 20 ? x.solicitud.titSolGas.Substring(0, 20):x.solicitud.titSolGas), est= x.solicitud.estado.nomEst , fec = x.solicitud.fchEveSolGas.ToShortDateString() , costo = x.solicitud.moneda.simbMon+" "+x.solicitud.monNeto}).Distinct();
            return Json(gastos, JsonRequestBehavior.AllowGet);
        }
        public JsonResult obtenerPres(string usu)
        {
            var pres = _pre.obtenerPresupuestoSimple().Where(x => x.idAccRes == usu && (x.idEst != ConstantesGlobales.estadoAnulado && x.idEst != ConstantesGlobales.estadoInactivo && x.idTipoPres==ConstantesGlobales.plan_Mark  && x.idConGas == ConstantesGlobales.conGas_Congreso)).Select(x => new { cod = x.idPres, esp = x.especialidad.nomEsp, mon = x.Monto , sal = Math.Round(x.Saldo,2) , consumo = Math.Round((x.Monto-x.Saldo),2) });
            return Json(pres, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize(Roles = "000003,000303")]
        public ActionResult IndexGeneral(string menuArea, string menuVista, string idAccRes = "", string fchEveSolGasI = "", string fchEveSolGasF = "")
        {
            //------------------------------
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();
            //------------------------------
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
            //---------------------------------
            //Primero obtenemos el día actual
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.FchEveSolGasI = fchEveSolGasI;
            SessionPersister.FchEveSolGasF = fchEveSolGasF;
            //Asi obtenemos el primer dia del mes actual
            //--------------------------------------------------------
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //--------------------------Fecha-------------------------
            IEnumerable<EstimacionModels> model = null;
            model = _est.obtenerEstimaciones().Where(x => (x.actividad.idAccRes == (idAccRes == "" ? x.actividad.idAccRes : idAccRes)) && ((x.actividad.fchIniVig >= inicio) && (x.actividad.fchFinVig <= fin)));
            //--------------------------Combo por usuario--------------------------------
            Parametros p = new Parametros();
            var parametro = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).ToList();
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp }).ToList();
            ViewBag.responsable = new SelectList(result.Select(x => new { idAccRes = x.value, nombre = x.nomComEmp }), "idAccRes", "nombre", idAccRes);
            //---------------------------------------------------------------
            return View(model);
            //---------------------------------------------------------------
        }
        public ActionResult DescargarSolicitud(bool html, string id)
        {
            var solicitud = _solGas.obtenerItem(id);
            ViewBag.pdf = html;
            //////////////////////////////////////////////////
            //Elaborar el PDF adjunto
            //////////////////////////////////////////////////
            /*string FileName = "Solicitud - " + solicitud.idSolGas.ToString() + " - " + solicitud.estado.nomEst;
            byte[] output = ControllerContext.GeneratePdf(solicitud, "DescargarSolicitud", (writer, document) => { document.SetPageSize(PageSize.A4); document.NewPage(); });
            //===================================================
            //--------------------------------------------------
            var path = Server.MapPath("~/Content/Home/Descarga/") + FileName + ".pdf";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            System.IO.File.WriteAllBytes(path, output);
            //---------------------------------------------------
            return File(path, "application/pdf");*/
            //---------------------------------------------------
            return new PdfActionResult(solicitud)
            {
                FileDownloadName = "Solicitud - " + solicitud.idSolGas.ToString() + " - " + solicitud.estado.nomEst + ".pdf"
            };
        }
    }
}