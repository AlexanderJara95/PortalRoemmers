using PortalRoemmers.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PortalRoemmers.Areas.Sistemas.Services.Presupuesto;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Trilogia;
using PortalRoemmers.Areas.Sistemas.Services.Gasto;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Sistemas.Services.Visitador;
using PortalRoemmers.Filters;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Presupuesto
{
    public class PresupuestoController : Controller
    {//PRESUPUESTOCONTROLLER 000222
        Ennumerador enu = new Ennumerador();
        private PresupuestoRepositorio _pto;
        private MonedaRepositorio _mon;
        private EstadoRepositorio _est;
        private UsuarioRepositorio _usu;
        private LineaRepositorio _lin;
        private ZonaRepositorio _zon;
        private TipoPresupuestoRepositorio _tPto;
        private Usu_Zon_LinRepositorio _uzl;
        private Pro_Lin_Repositorio _pl;
        private ConceptoGastoRepositorio _cgas;
        private EspecialidadRepositorio _esp;
        private AsigAproRepositorio _asiapr;
        private TipoPresupuestoRepositorio _tipPres;
        private TipoGastoRepositorio _tipGas;
        private Esp_Usu_Repositorio _eu;

        // GET: Sistemas/
        public PresupuestoController()
        {
            _pto = new PresupuestoRepositorio();
            _mon = new MonedaRepositorio();
            _est = new EstadoRepositorio();
            _usu = new UsuarioRepositorio();
            _lin = new LineaRepositorio();
            _zon = new ZonaRepositorio();
            _tPto = new TipoPresupuestoRepositorio();
            _uzl = new Usu_Zon_LinRepositorio();
            _pl = new Pro_Lin_Repositorio();
            _cgas = new ConceptoGastoRepositorio();
            _esp = new EspecialidadRepositorio();
            _asiapr = new AsigAproRepositorio();
            _tipPres = new TipoPresupuestoRepositorio();
            _tipGas = new TipoGastoRepositorio();
            _eu = new Esp_Usu_Repositorio();
        }
        //-------------LISTAR 000223---------------------
        [CustomAuthorize(Roles = "000003,000223")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "", string idEst = "1", string fchEveSolGasI = "", string fchEveSolGasF = "")
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
                DateTime oUltimoDiaDelMes = new DateTime(date.Year, 12, 31);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = oUltimoDiaDelMes.ToString("dd/MM/yyyy");
                //var actual = DateTime.Today.ToString("dd/MM/yyyy");
                inicio = DateTime.Parse(primero);
                fin = DateTime.Parse(actual);
            }
            //-----------------------------
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            //-----------------------------
            var model = _pto.obtenerTodos(pagina, search, inicio.ToString(), fin.ToString());
            //-----------------------------
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            ViewBag.search = search;
            ViewBag.estado = new SelectList(_est.obteneEstadoGlobal(), "idEst", "nomEst", idEst);
            //-----------------------------
            return View(model);
        }
        //-------------INDEX GENERAL -----------------------
        [CustomAuthorize(Roles = "000003,000340")]
        public ActionResult IndexGeneral(string menuArea, string menuVista, string idTipoPres = "" , string idEst = "1" ,string idAccRes = "", string fchEveSolGasI = "", string fchEveSolGasF = "")
        {
            //------------------------------
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();
            //------------------------------
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
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
            SessionPersister.FchEveSolGasI = fchEveSolGasI;
            SessionPersister.FchEveSolGasF = fchEveSolGasF;
            //Asi obtenemos el primer dia del mes actual
            //--------------------------------------------------------
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //--------------------------Fecha-------------------------
            IEnumerable<PresupuestoModels> model = null;
            model = _pto.obtenerPresupuesto().Where(x => (x.idTipoPres==(idTipoPres==""?x.idTipoPres: idTipoPres)) && (x.idEst==(idEst==""?x.idEst:idEst)) && (x.idAccRes == (idAccRes==""?x.idAccRes:idAccRes)) &&((x.fchIniVigencia >= inicio) && (x.fchFinVigencia <= fin)));
            //FILTRO DE TIPO DE PRESUPUESTO
            ViewBag.tipoPres = new SelectList(_tipPres.obtenerTipPres(), "idTipoPres", "nomTipPres", idTipoPres);
            ViewBag.estados = new SelectList(_est.obteneEstadoGlobal(), "ideST", "nomEst", idEst);
            //FILTRO DE USUARIOS
            Parametros p = new Parametros();
            var parametro1 = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_01).ToList();
            var parametro2 = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).ToList();
            var parametro3 = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_04).ToList();
            //Union de Parametros 
            var parametro = parametro1.Union(parametro2).Union(parametro3).ToList();
            //Join con los datos de usuario
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp }).Distinct().OrderBy(x=>x.nomComEmp).ToList();
            ViewBag.Usuarios = new SelectList(result.Select(x => new { idAccRes = x.value, nombre = x.nomComEmp }), "idAccRes", "nombre", idAccRes);
            //--------------------------Combo por usuario--------------------------------
            //---------------------------------------------------------------
            return View(model);
            //---------------------------------------------------------------
        }
        //--------------------------------------------------
        //-------------REGISTRAR 000224---------------------
        [CustomAuthorize(Roles = "000003,000224")]
        [HttpGet]
        public ActionResult Registrar()
        {
            var actual = DateTime.Today.ToString("dd/MM/yyyy");
            ViewBag.actual = actual;
            PresupuestoModels model = new PresupuestoModels();
            cargarCombos(model);
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(PresupuestoModels model)
        {
            if (ModelState.IsValid)
            {
                model.idEst = ConstantesGlobales.estadoActivo;
                model.Saldo = model.Monto;
                model.Estim = model.Monto;
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;
                if (model.idConGas =="00")
                {
                    model.idConGas = null;
                }
                TempData["mensaje"] = _pto.crear(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            cargarCombos(model);
            return View(model);
        }
        //-------------MODIFICAR 000247---------------------
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000247")]
        public ActionResult Modificar(string id,string modulo)
        {
            var model = _pto.obtenerItem(id);
            ViewBag.TipoPres = new SelectList(_tPto.obtenerTipPres().Select(x => new { x.idTipoPres, x.nomTipPres }), "idTipoPres", "nomTipPres", model.idTipoPres);
            ViewBag.responsable = new SelectList(_usu.obtenerUsuarios().Select(x => new { idAccRes = x.idAcc, nombre = x.empleado.nomComEmp }), "idAccRes", "nombre", model.idAccRes);
            ViewBag.linea = new SelectList(_lin.obtenerLineas().Select(x => new { x.idLin, x.nomLin }).Distinct(), "idLin", "nomLin", model.idLin);
            ViewBag.zona = new SelectList(_zon.obtenerZonas().Select(x => new { x.idZon, x.nomZon }).Distinct(), "idZon", "nomZon", model.idZon);
            ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "abrMon", model.idMon);
            ViewBag.Jefes = new SelectList(_usu.obtenerUsuarios().Select(x => new { idAccJ = x.idAcc, nombre = x.empleado.nomComEmp }), "idAccJ", "nombre", model.idAccJ);
            ViewBag.tipgasto = new SelectList(_tipGas.obtenerTipGasto().Select(x => new { idTipGas = x.idTipGas, nomTipGas = x.nomTipGas }).Distinct(), "idTipGas", "nomTipGas", model.idTipGas);
            ViewBag.congasto = new SelectList(_cgas.obtenerConceptoGastos(), "idConGas", "nomConGas", model.idConGas);
            ViewBag.especialidad = new SelectList(_esp.obteneEspecialidades(), "idEsp", "nomEsp", model.idEsp);
            TempData["Modular"] = modulo;
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(PresupuestoModels model)
        {
            if (ModelState.IsValid)
            {
                model.usufchMod = DateTime.Now;
                model.usuMod = SessionPersister.Username;
                if (model.idConGas == "00")
                {
                    model.idConGas = null;
                }
                TempData["mensaje"] = _pto.modificar(model);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            return View(model);
        }
        //-------------ANULAR 000248---------------------
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000248")]
        public ActionResult Anular(string id)
        {
            var model = _pto.obtenerItem(id);

            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Anular(PresupuestoModels model)
        {
            model.usuMod = SessionPersister.Username;
            model.usufchMod = DateTime.Now;
            string mensaje = "";
            _pto.anular(model, out mensaje);
            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
        //
        [HttpPost]
        public JsonResult buscarTipoPresupuesto(string idTipPres)
        {
            Parametros p = new Parametros();
            var variable = "No hay parametro disponible";
            if (p.Resultado(ConstantesGlobales.Com_PtoGas_Tipo_Cas_01).Contains(idTipPres))
            {
                variable = ConstantesGlobales.Com_PtoGas_Tipo_Cas_01;
            }
            else if (p.Resultado(ConstantesGlobales.Com_PtoGas_Tipo_Cas_02).Contains(idTipPres))
            {
                variable = ConstantesGlobales.Com_PtoGas_Tipo_Cas_02;
            }   
            return Json(variable, JsonRequestBehavior.AllowGet);
        }
        public void cargarCombos(PresupuestoModels model)
        {
            //******************************************************
            string Opcion = "";
            if (model.idTipoPres == ConstantesGlobales.plan_Trab)
            {
                Opcion = ConstantesGlobales.Com_PtoGas_Tipo_Cas_01;
            }
            else if (model.idTipoPres == ConstantesGlobales.plan_Mark)
            {
                Opcion = ConstantesGlobales.Com_PtoGas_Tipo_Cas_02;
            }
            else if (model.idTipoPres == ConstantesGlobales.plan_Fuera)
            {
                Opcion = ConstantesGlobales.Com_PtoGas_Tipo_Cas_03;
            }
            //******************************************************
            Parametros p = new Parametros();
            var parametro = p.selectResultado(Opcion).ToList();
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp }).ToList();
            var selectedList = new SelectList(result.Select(x => new { idAccRes = x.value, nombre = x.nomComEmp }), "idAccRes", "nombre");
            ViewBag.responsable = new SelectList(result.Select(x => new { idAccRes = x.value, nombre = x.nomComEmp }), "idAccRes", "nombre",model.idAccRes);
            //******************************************************
            ViewBag.TipoPres = new SelectList(_tPto.obtenerTipPres().Select(x => new { x.idTipoPres, x.nomTipPres }), "idTipoPres", "nomTipPres", model.idTipoPres);
            ViewBag.linea = new SelectList(_uzl.obtenerTrilogia().Where(x => x.idAcc == model.idAccRes).Select(x => new { x.linea.idLin, x.linea.nomLin }).Distinct(), "idLin", "nomLin", model.idLin);
            ViewBag.zona = new SelectList(_uzl.obtenerTrilogia().Where(x => x.idAcc == model.idAccRes).Select(x => new { x.zona.idZon, x.zona.nomZon }).Distinct(), "idZon", "nomZon", model.idZon);
            ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "abrMon", model.idMon);
            ViewBag.Jefes = new SelectList(_asiapr.obtenerAprobadoresAsignados(model.idAccRes).Select(x => new { idAccJ = x.idAccApro, nom = x.aprobador.empleado.nomComEmp }).Distinct(), "idAccJ", "nom",model.idAccJ);
            ViewBag.tipgasto = new SelectList(_tipGas.obtenerTipGasto().Select(x=>new { idTipGas = x.idTipGas, nomTipGas = x.nomTipGas }).Distinct(), "idTipGas", "nomTipGas",model.idTipGas);
            ViewBag.congasto = new SelectList(_cgas.obtenerConceptoGastos().Where(x => x.idTipGas == model.idTipGas), "idConGas", "nomConGas", model.idConGas);
            ViewBag.especialidad = new SelectList(_esp.obteneEspecialidades(), "idEsp", "nomEsp", model.idEsp);
            
        }
        [HttpPost]
        public JsonResult buscarResponsable(string idTipPres)
        {
            //******************************************************
            string Opcion = "";
            if (idTipPres == ConstantesGlobales.plan_Trab)
            {
                Opcion = ConstantesGlobales.Com_PtoGas_Tipo_Pres_01;
            }
            else if (idTipPres == ConstantesGlobales.plan_Mark)
            {
                Opcion = ConstantesGlobales.Com_PtoGas_Tipo_Pres_02;
            }
            else if (idTipPres == ConstantesGlobales.plan_Fuera)
            {
                Opcion = ConstantesGlobales.Com_PtoGas_Tipo_Pres_03;
            }
            //******************************************************
            Parametros p = new Parametros();
            var parametro = p.selectResultado(Opcion).ToList();
            var usuario = _usu.obtenerUsuarios().ToList();
            var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp }).ToList();
            //******************************************************
            var selectedList = new SelectList(result.Select(x => new { idAccRes = x.value, nombre = x.nomComEmp }), "idAccRes", "nombre");
            return Json(select(selectedList, ""), JsonRequestBehavior.AllowGet);
            //******************************************************
        }
        [HttpPost]
        public JsonResult buscarLinea(string idAccRes)
        {
            var selectedList = new SelectList(_uzl.obtenerTrilogia().Where(x => x.idAcc == idAccRes).Select(x => new { x.linea.idLin, x.linea.nomLin }).Distinct(), "idLin", "nomLin");
            return Json(select(selectedList, idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarZona(string idAccRes)
        {
            var selectedList = new SelectList(_uzl.obtenerTrilogia().Where(x => x.idAcc == idAccRes).Select(x => new { x.zona.idZon, x.zona.nomZon }).Distinct(), "idZon", "nomZon");
            return Json(select(selectedList,idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarJefe(string idAccRes)
        {
            var selectedList = new SelectList(_asiapr.obtenerAprobadoresAsignados(idAccRes).Select(x => new { idAccJ = x.idAccApro, nom = x.aprobador.empleado.nomComEmp }).Distinct(), "idAccJ", "nom"); 
            return Json(select(selectedList, idAccRes), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscarEspecilidad(string idAccRes)
        {
            return Json(selectEspecialida(idAccRes), JsonRequestBehavior.AllowGet);
        }
        public List<SelectedModels> select(SelectList sel,string idAccRes)
        {
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();
            foreach (var v in sel)
            {
                cbo.value = v.Value;
                cbo.text = v.Text;
                cboList.Add(cbo);
                cbo = new SelectedModels();
            }
            return cboList;
        }
        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000382")]
        public JsonResult modificarSaldo(string idpres , string diferencia)
        {
            //valores iniciales
            //---------------------------
            Boolean correcto = true;
            string mensaje = "";
            //---------------------------
            PresupuestoModels p = new PresupuestoModels();
            p.idPres = idpres;
            p.usuMod = SessionPersister.Username;
            p.usufchMod = DateTime.Now;
            p.diferencia = Double.Parse(diferencia);
            //---------------------------
            correcto = _pto.modificarSaldo(p,out mensaje);
            TempData["mensaje"] = mensaje;
            //---------------------------
            //correcto = false;
            return Json(correcto, JsonRequestBehavior.AllowGet);
        }
        public List<SelectedModels> selectEspecialida(string idAccRes)
        {
            List<SelectedModels> cboList = new List<SelectedModels>();
            SelectedModels cbo = new SelectedModels();

            //var i = idAccRes.Split('-');
            //var res = i[0];
            var esp = _eu.obtenerEspecialidadesXusuario(idAccRes);

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
    }
}