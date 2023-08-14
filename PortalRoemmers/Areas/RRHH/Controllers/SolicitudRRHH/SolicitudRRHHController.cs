using PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH;
using PortalRoemmers.Areas.RRHH.Services.SolicitudRRHH;
using PortalRoemmers.Areas.RRHH.Models.Grupo;
using PortalRoemmers.Areas.RRHH.Services.Grupo;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.SolicitudRRHH
{
    public class SolicitudRRHHController : Controller
    {
        // GET: RRHH/SolicitudRRHH
        private SolicitudRRHHRepositorio _soli;
        private UsuarioRepositorio _usu;
        private EmpleadoRepositorio _emp;
        private GrupoRRHHRepositorio _gru;
        private Ennumerador enu;
        private Parametros p;

        public SolicitudRRHHController()
        {
            _emp = new EmpleadoRepositorio();
            _soli = new SolicitudRRHHRepositorio();
            _usu = new UsuarioRepositorio();
            _gru = new GrupoRRHHRepositorio();
            p = new Parametros();
            enu = new Ennumerador();
        }

        /*public ActionResult Index()
        {
            return View();
        }*/
        [CustomAuthorize(Roles = "000003,000405")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "", string idAcc = "", string fchEveSolGasI = "", string fchEveSolGasF = "")
       {
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];
            //-----------------------------
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();

            DateTime primero = new DateTime();
            DateTime actual = new DateTime();

            DateTime date = DateTime.Now;
            DateTime oPrimerDiaDelMes = new DateTime(date.Year, 1, 1);
            DateTime ultimoDelanio = new DateTime(date.Year, 12, 31);
            primero = DateTime.Parse(oPrimerDiaDelMes.ToString("dd/MM/yyyy"));
            actual = DateTime.Parse(ultimoDelanio.ToString("dd/MM/yyyy"));
            /*var actual = DateTime.Today.ToString("dd/MM/yyyy");*/

            //-----------------------------
            try
            {
                inicio = DateTime.Parse(fchEveSolGasI);
                fin = DateTime.Parse(fchEveSolGasF);
            }
            catch (Exception e)
            {
                inicio = primero;
                fin = actual;
            }
            //-----------------------------
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();

            //-----------------------------
            ViewBag.search = search;
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            ViewBag.userId = SessionPersister.UserId;

            //-----------------------------
            //var parametro = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).ToList();
            //var usuario = _usu.obtenerUsuarios().ToList();
            //var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp });
            //-----------------------------
            //ViewBag.gerentesProd = new SelectList(result.Select(x => new { idAccResGP = x.value, nombre = x.nomComEmp }), "idAccResGP", "nombre");
            //ViewBag.actividades = new SelectList(_act.obtenerActividades().Where(x => x.idAccRes == idAcc && x.estimacion != null && ((DateTime.Today >= x.fchIniVig) && (DateTime.Today <= x.fchFinVig))).Select(x => new { idActividades = x.idActiv, nomActiv = x.nomActiv }), "idActividades", "nomActiv");
            //-----------------------------
            // validación de rango de fechas del año actual
            var modelCant = _soli.obtenerTodos(pagina, search, ConstantesGlobales.subTipoVacaciones, primero.ToString(), actual.ToString());
            int total = diasTotalesVacaciones(emple.ingfchEmp.Value,emple.idAreRoe);
            int rest = diasRestantes(modelCant.SoliRRHH, total);
            // filtrado general
            var model = _soli.obtenerTodos(pagina, search, ConstantesGlobales.subTipoVacaciones, inicio.ToString(), fin.ToString());
            ViewBag.SolicitudMasiva = validarSolicitudMasivaAdmin(model.SoliRRHH);
            ViewBag.SolicitudAdmin = validarSolicitudAdmin(model.SoliRRHH);
            ViewBag.diasRestantes =  rest;
            ViewBag.diasUtilizados = total - rest;
            //-----------------------------
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000406")]
        [HttpGet]
        public ActionResult Registrar(int diasRestantes)
        {
            ViewBag.procedeA = true;
            //dias restantes sin el inicio y final actual
            ViewBag.diasRestantes = diasRestantes;

            SolicitudRRHHModels model = new SolicitudRRHHModels();
            model.idAccSol = SessionPersister.UserId;

            ///-----------------------
            ///----------------------- 
            var actual = DateTime.Today.ToString("dd/MM/yyyy");

            ViewBag.fecha = actual;
            return View();

        }

        [CustomAuthorize(Roles = "000003,000406")]
        [HttpGet]
        public ActionResult RegistrarMasivamente()
        {
            ViewBag.procedeA = true;
            SolicitudRRHHModels model = new SolicitudRRHHModels();
            model.idAccSol = SessionPersister.UserId;
            ViewBag.GrupoRrhh = new SelectList(_gru.obtenerGruposRrhh(), "idGrupoRrhh", "descGrupo");
            var actual = DateTime.Today.ToString("dd/MM/yyyy");

            ViewBag.fecha = actual;

            return View();
        }


        [HttpPost]
        [SessionAuthorize]
        public ActionResult RegistrarMasivamente(SolicitudRRHHModels model)
        {
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];
            //string responsableD = emple.idEmp + ";" + emple.apePatEmp + " " + emple.apeMatEmp + " " + emple.nom1Emp + " " + emple.nom2Emp + ";" + "";
            string idGrupoRrhh = model.idAccSol;
            string tabla = "tb_SolicitudRRHH";
            int idc = enu.buscarTabla(tabla);
            var empJefe = _emp.obtenerItem(emple.idEmpJ);
            var usuJefe = _usu.obtenerItemXEmpleado(emple.idEmpJ);
            var usuPrinc = _usu.obtenerItemXEmpleado(emple.idEmp);

            model.idSolicitudRrhh = idc.ToString("D7");
            model.idEstado = ConstantesGlobales.estadoRegistrado;
            model.idAccSol = SessionPersister.UserId;
            model.usuCrea = SessionPersister.Username;
            model.usufchCrea = DateTime.Now;

            UserSolicitudRRHHModels userSoliRRHH = new UserSolicitudRRHHModels();
            userSoliRRHH.idSolicitudRrhh = model.idSolicitudRrhh;
            userSoliRRHH.idAccRes = SessionPersister.UserId;
            userSoliRRHH.usuCrea = model.usuCrea;
            userSoliRRHH.usufchCrea = model.usufchCrea;

            GrupoSolicitudRRHHModels grupoSoliRRHH = new GrupoSolicitudRRHHModels();
            grupoSoliRRHH.idSolicitudRrhh = model.idSolicitudRrhh;
            grupoSoliRRHH.idGrupoRrhh = idGrupoRrhh;
            grupoSoliRRHH.descGrupo = model.descSolicitud;
            grupoSoliRRHH.usuCrea = model.usuCrea;
            grupoSoliRRHH.usufchCrea = model.usufchCrea;


            if (emple.idEmpJ != "" || emple.idEmpJ != null)
            {
                model.idAccApro = _usu.obtenerItemXEmpleado(emple.idEmpJ).idAcc;
                model.idSubTipoSolicitudRrhh = ConstantesGlobales.subTipoVacacionesM;
                try
                {
                    if (_soli.crear(model))
                    {
                        enu.actualizarTabla(tabla, idc);
                        TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                        _soli.crearUserSoliRrhh(userSoliRRHH);
                        _soli.crearGrupoSoliRrhh(grupoSoliRRHH);

                        //envio mensaje al usuario emisor
                        EmailHelper m = new EmailHelper();
                        string mensaje = string.Format("<section> Estimado (a) {0}<BR/> <p>Se registró una solicitud de vacaciones</p></section>", emple.nomComEmp);
                        string titulo = "Solicitud de Vacaciones";
                        m.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensaje, titulo, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

                        //envio mensaje al usuario receptor
                        EmailHelper m1 = new EmailHelper();
                        string mensaje1 = string.Format("<section> Estimado (a) {0}<BR/> <p>Nuevo registro de vacaciones de {1}</p></section>", empJefe.nomComEmp, emple.nomComEmp);
                        string titulo1 = "Solicitud de Vacaciones";
                        m.SendEmail(/*model.solicitante.email*/usuJefe.email, mensaje1, titulo1, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

                    }
                    else
                    {
                        TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error en guardar el registro" + "</div>";
                    }
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Se necesita asignar Jefe" + "</div>";
            }

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });

        }


        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(SolicitudRRHHModels model, int diasRestantes)
        {
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];
            //EmpleadoModels jef = new EmpleadoModels();
            var empJefe =_emp.obtenerItem(emple.idEmpJ);
            var usuJefe = _usu.obtenerItemXEmpleado(emple.idEmpJ);
            var usuPrinc = _usu.obtenerItemXEmpleado(emple.idEmp);
            //string responsableD = emple.idEmp + ";" + emple.apePatEmp + " " + emple.apeMatEmp + " " + emple.nom1Emp + " " + emple.nom2Emp + ";" + "";
            string tabla = "tb_SolicitudRRHH";
            int idc = enu.buscarTabla(tabla);
            model.idSolicitudRrhh = idc.ToString("D7");

            model.idEstado = ConstantesGlobales.estadoRegistrado;
            model.idAccSol = SessionPersister.UserId;
            model.usuCrea = SessionPersister.Username;
            model.usufchCrea = DateTime.Now;

            UserSolicitudRRHHModels userSoliRRHH = new UserSolicitudRRHHModels();
            userSoliRRHH.idSolicitudRrhh = model.idSolicitudRrhh;
            userSoliRRHH.idAccRes = SessionPersister.UserId;
            userSoliRRHH.usuCrea = model.usuCrea;
            userSoliRRHH.usufchCrea = model.usufchCrea;
            if (validarLimiteVacaciones(model.fchIniSolicitud,model.fchFinSolicitud, diasRestantes))
            {
                if (emple.idEmpJ != "" || emple.idEmpJ != null)
                {
                    model.idAccApro = _usu.obtenerItemXEmpleado(emple.idEmpJ).idAcc;
                    model.idSubTipoSolicitudRrhh = ConstantesGlobales.subTipoVacaciones;
                    try
                    {
                        if (_soli.crear(model))
                        {
                            enu.actualizarTabla(tabla, idc);
                            TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                            _soli.crearUserSoliRrhh(userSoliRRHH);

                            //envio mensaje al usuario emisor
                            EmailHelper m = new EmailHelper();
                            string mensaje = string.Format("<section> Estimado (a) {0}<BR/> <p>Se registró una solicitud de vacaciones</p></section>", emple.nomComEmp);
                            string titulo = "Solicitud de Vacaciones";
                            m.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensaje, titulo, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

                            //envio mensaje al usuario receptor
                            EmailHelper m1 = new EmailHelper();
                            string mensaje1 = string.Format("<section> Estimado (a) {0}<BR/> <p>Nueva solicitud de vacaciones de {1}</p></section>", empJefe.nomComEmp, emple.nomComEmp);
                            string titulo1 = "Solicitud de Vacaciones";
                            m.SendEmail(/*model.solicitante.email*/usuJefe.email, mensaje1, titulo1, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);
                        }
                        else
                        {
                            TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Error en guardar el registro" + "</div>";
                        }
                    }
                    catch (Exception e)
                    {
                        e.Message.ToString();
                    }
                }
                else{
                    TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Se necesita asignar Jefe" + "</div>";
                }
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Has superado la cantidad de días disponibles" + "</div>";
            }

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });

        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000407")]
        public ActionResult Modificar(string id, string diasRestantes)
        {
            var model = _soli.obtenerItem(id);
            ViewBag.fechaIni = model.fchIniSolicitud.ToShortDateString();
            ViewBag.fechaFin = model.fchFinSolicitud.ToShortDateString();
            int diasHabiles = calcularDiasHabiles(model.fchIniSolicitud,model.fchFinSolicitud);
            //dias restantes sin el inicio y final actual
            ViewBag.diasRestantes = Convert.ToInt32(diasRestantes) + diasHabiles;
            //model.idSubTipoSolicitudRrhh = model.idSubTipoSolicitudRrhh;            
            return View(model);
        }

        
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(SolicitudRRHHModels model,int diasRestantes)
        {
            model.usufchMod = DateTime.Now;
            model.usuMod = SessionPersister.Username;
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];
            var empJefe = _emp.obtenerItem(emple.idEmpJ);
            var usuJefe = _usu.obtenerItemXEmpleado(emple.idEmpJ);
            var usuPrinc = _usu.obtenerItemXEmpleado(emple.idEmp);
            var mensaje = "";
            if (model.idEstado == ConstantesGlobales.estadoRechazado)
            {
                model.idEstado = ConstantesGlobales.estadoModificado;               
            }

            if (validarLimiteVacaciones(model.fchIniSolicitud, model.fchFinSolicitud, diasRestantes))
            {
                try
                {
                    if (_soli.modificar(model))
                    {
                        //envio mensaje al usuario emisor
                        EmailHelper mE = new EmailHelper();
                        string mensajeE = string.Format("<section> Estimado (a) {0}<BR/> <p>Se modificó una solicitud de vacaciones</p></section>", emple.nomComEmp);
                        string tituloE = "Solicitud de Vacaciones";
                        mE.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensajeE, tituloE, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

                        //envio mensaje al usuario receptor
                        EmailHelper mR = new EmailHelper();
                        string mensajeR = string.Format("<section> Estimado (a) {0}<BR/> <p>Se modificó una solicitud de vacaciones de {1}</p></section>", empJefe.nomComEmp, emple.nomComEmp);
                        string tituloR = "Solicitud de Vacaciones";
                        mR.SendEmail(/*model.solicitante.email*/usuJefe.email, mensajeR, tituloR, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);
                        mensaje = "<div id='success' class='alert alert-success'>Se modificó correctamente el registro.</div>";
                    }
                    else
                    {
                        mensaje = "<div id='warning' class='alert alert-warning'>" + "Error en la modificación del registro" + "</div>";
                    }

                }
                catch (Exception e) {
                    e.Message.ToString();
                }
            }
            else
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + "Has superado la cantidad de días disponibles" + "</div>";
            }

            TempData["mensaje"] = mensaje;

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }


        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult ModificarMasivamente(string id)
        {
            var model = _soli.obtenerItem(id);
            var modelGrupo = _soli.obtenerGrupoSoliRrhh(id);
            ViewBag.fechaIni = model.fchIniSolicitud.ToShortDateString();
            ViewBag.fechaFin = model.fchFinSolicitud.ToShortDateString();
            //dias restantes sin el inicio y final actual
            //model.idSubTipoSolicitudRrhh = model.idSubTipoSolicitudRrhh;
            ViewBag.GrupoRrhh = new SelectList(_gru.obtenerGruposRrhh(), "idGrupoRrhh", "descGrupo");
            ViewBag.SelectedGrupoRrhh = modelGrupo.idGrupoRrhh; // Valor del elemento a seleccionar
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult ModificarMasivamente(SolicitudRRHHModels model)
        {
            string idGrupoRrhh = model.idAccSol;
            model.usufchMod = DateTime.Now;
            model.usuMod = SessionPersister.Username;
            model.idAccSol = SessionPersister.UserId;

            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];
            var mensaje = "";
            if (model.idEstado == ConstantesGlobales.estadoRechazado)
            {
                model.idEstado = ConstantesGlobales.estadoModificado;
            }                     

            GrupoSolicitudRRHHModels grupoSoliRRHH = new GrupoSolicitudRRHHModels();
            grupoSoliRRHH.idSolicitudRrhh = model.idSolicitudRrhh;
            grupoSoliRRHH.idGrupoRrhh = idGrupoRrhh;
            grupoSoliRRHH.descGrupo = model.descSolicitud;
            grupoSoliRRHH.usuMod = model.usuCrea;
            grupoSoliRRHH.usufchMod = model.usufchCrea;

            try
            {
                if (_soli.modificar(model) && _soli.modificarGrupoSoliRrhh(grupoSoliRRHH))
                {
                    mensaje = "<div id='success' class='alert alert-success'>Se modificó correctamente el registro.</div>";
                }
                else
                {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + "Error en la modificación del registro" + "</div>";
                }

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }

            TempData["mensaje"] = mensaje;

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000407")]
        public ActionResult Visualizar(string id)
        {
            var model = _soli.obtenerItem(id);
            ViewBag.fechaIni = model.fchIniSolicitud.ToShortDateString();
            ViewBag.fechaFin = model.fchFinSolicitud.ToShortDateString();
            int dias = calcularDiasHabiles(model.fchIniSolicitud, model.fchFinSolicitud);
            //dias restantes sin el inicio y final actual
            ViewBag.dias = dias;
            //model.idSubTipoSolicitudRrhh = model.idSubTipoSolicitudRrhh;            
            return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000407")]
        public ActionResult VisualizarMasivamente(string id)
        {
            var model = _soli.obtenerItem(id);
            ViewBag.fechaIni = model.fchIniSolicitud.ToShortDateString();
            ViewBag.fechaFin = model.fchFinSolicitud.ToShortDateString();
            int dias = calcularDiasHabiles(model.fchIniSolicitud, model.fchFinSolicitud);
            //dias restantes sin el inicio y final actual
            ViewBag.dias = dias;
            //model.idSubTipoSolicitudRrhh = model.idSubTipoSolicitudRrhh;            
            return View(model);
        }

        public int diasTotalesVacaciones(DateTime fchIngreso, string idAreRoe)
        {
            int anioActual = DateTime.Now.Year;

            //Si el trabajador ingresó el año anterior
            if ((anioActual - fchIngreso.Year == 1) )
            {
                //si no es ventas
                if (idAreRoe != ConstantesGlobales.areaVentas)
                {
                    DateTime ultimodia = new DateTime(DateTime.Now.Year - 1, 12, 31); //ultimo dia del año anterior
                    TimeSpan diferencia = ultimodia.Subtract(fchIngreso); //diferencia entre días
                    int diaAnio = DateTime.IsLeapYear(DateTime.Now.Year) ? 366 : 365; //dias del año
                    double cantidadDias = Math.Round((30 * diferencia.Days) / (diaAnio) * 1.0); //diás de vacaciones
                    cantidadDias = (cantidadDias % 7) == 6 ? cantidadDias - (((Math.Floor(cantidadDias / 7)) * 2) + 1) : cantidadDias - ((Math.Floor(cantidadDias / 7)) * 2);
                    return Convert.ToInt32(cantidadDias);
                }
                else
                {
                    return 22;
                }
                
            }
            else
            {                
                if ((anioActual - fchIngreso.Year > 1))
                {
                    return 22;                    
                }
                else
                {
                    if (idAreRoe == ConstantesGlobales.areaVentas)
                    {
                        DateTime ultimodia = new DateTime(DateTime.Now.Year, 12, 31); //ultimo dia del año actual
                        TimeSpan diferencia = ultimodia.Subtract(fchIngreso); //diferencia entre días
                        int diaAnio = DateTime.IsLeapYear(DateTime.Now.Year) ? 366 : 365; //dias del año
                        double cantidadDias = Math.Round((30 * diferencia.Days) / (diaAnio) * 1.0); //diás de vacaciones
                        cantidadDias = (cantidadDias % 7) == 6 ? cantidadDias - (((Math.Floor(cantidadDias / 7)) * 2) + 1) : cantidadDias - ((Math.Floor(cantidadDias / 7)) * 2);
                        return Convert.ToInt32(cantidadDias);
                    }
                    else
                    {
                        return 0;
                    }                    
                }
            }
        }

        public bool validarSolicitudAdmin(List<SolicitudRRHHModels> soli)
        {
            foreach (var item in soli)
            {
                if (item.idAccApro == ViewBag.userId)
                {
                    return true;
                }
            }
            return false;
        }
        public bool validarSolicitudMasivaAdmin(List<SolicitudRRHHModels> soli)
        {
            foreach (var item in soli)
            {
                if (item.idSubTipoSolicitudRrhh == ConstantesGlobales.subTipoVacacionesM && (item.idAccSol == SessionPersister.UserId || item.idAccApro == SessionPersister.UserId))
                {
                    return true;
                }
            }
            return false;
        }

        public int diasRestantes(List<SolicitudRRHHModels> soli , int cantTotalDisponible)
        {
            foreach (var item in soli)
            {
                if (((item.idAccSol == SessionPersister.UserId && item.idSubTipoSolicitudRrhh == ConstantesGlobales.subTipoVacaciones) ||
                    (item.idAccSol != SessionPersister.UserId && item.idAccApro != SessionPersister.UserId && item.idSubTipoSolicitudRrhh == ConstantesGlobales.subTipoVacacionesM))
                    && (item.idEstado == ConstantesGlobales.estadoAprobado))
                {
                    cantTotalDisponible -= calcularDiasHabiles(item.fchIniSolicitud, item.fchFinSolicitud);
                }
            }
            return cantTotalDisponible;
        }

        public bool validarLimiteVacaciones(DateTime fechaInicio, DateTime fechaFin, int diasRestantes)
        {
            int diasHabiles=calcularDiasHabiles(fechaInicio, fechaFin);
            if ((diasRestantes - diasHabiles)>=0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int calcularDiasHabiles(DateTime fechaInicio, DateTime fechaFin)
        {
            int diasHabiles = 0;
            TimeSpan tiempoTotal = fechaFin - fechaInicio;

            for (int i = 0; i <= tiempoTotal.TotalDays; i++)
            {
                DateTime fechaActual = fechaInicio.AddDays(i);

                if (fechaActual.DayOfWeek != DayOfWeek.Saturday && fechaActual.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasHabiles++;
                }
            }
            return diasHabiles;
        }

        public JsonResult anularSolicitud(string idSolicitudRRHH)
        {
            var variable = _soli.updateEstadoSoliRRHH(idSolicitudRRHH, ConstantesGlobales.estadoAnulado);

            var usuPrinc = _usu.obtenerItem(SessionPersister.UserId);
            var empPrinc = _emp.obtenerItem(usuPrinc.idEmp);
            var usuJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);
            var empJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);

            //envio mensaje al usuario emisor
            EmailHelper mE = new EmailHelper();
            string mensajeE = string.Format("<section> Estimado (a) {0}<BR/> <p>Se anuló la solicitud de vacaciones</p></section>", empPrinc.nomComEmp);
            string tituloE = "Anulación de solicitud de Vacaciones";
            mE.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensajeE, tituloE, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            //envio mensaje al usuario receptor
            EmailHelper mR = new EmailHelper();
            string mensajeR = string.Format("<section> Estimado (a) {0}<BR/> <p>Se anuló la solicitud de vacaciones a {1}</p></section>", empJefe.nomComEmp, empPrinc.nomComEmp);
            string tituloR = "Anulación de solicitud de Vacaciones";
            mR.SendEmail(/*model.solicitante.email*/usuJefe.email, mensajeR, tituloR, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            return Json(variable, JsonRequestBehavior.AllowGet);
        }
        //9
        public JsonResult aprobarSolicitud(string idSolicitudRRHH)
        {
            var variable = _soli.updateEstadoSoliRRHH(idSolicitudRRHH, ConstantesGlobales.estadoAprobado);

            var usuPrinc = _usu.obtenerItem(SessionPersister.UserId);
            var empPrinc = _emp.obtenerItem(usuPrinc.idEmp);
            var usuJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);
            var empJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);

            //envio mensaje al usuario emisor
            EmailHelper mE = new EmailHelper();
            string mensajeE = string.Format("<section> Estimado (a) {0}<BR/> <p>Se aprobó una solicitud de vacaciones</p></section>", empPrinc.nomComEmp);
            string tituloE = "Aprobación de solicitud de Vacaciones";
            mE.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensajeE, tituloE, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            //envio mensaje al usuario receptor
            EmailHelper mR = new EmailHelper();
            string mensajeR = string.Format("<section> Estimado (a) {0}<BR/> <p>Se aprobó una solicitud de vacaciones a {1}</p></section>", empJefe.nomComEmp, empPrinc.nomComEmp);
            string tituloR = "Aprobación de solicitud de Vacaciones";
            mR.SendEmail(/*model.solicitante.email*/usuJefe.email, mensajeR, tituloR, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            return Json(variable, JsonRequestBehavior.AllowGet);
        }
        //12
        public JsonResult rechazarSolicitud(string idSolicitudRRHH)
        {
            var variable = _soli.updateEstadoSoliRRHH(idSolicitudRRHH, ConstantesGlobales.estadoRechazado);

            var usuPrinc = _usu.obtenerItem(SessionPersister.UserId);
            var empPrinc = _emp.obtenerItem(usuPrinc.idEmp);
            var usuJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);
            var empJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);

            //envio mensaje al usuario emisor
            EmailHelper mE = new EmailHelper();
            string mensajeE = string.Format("<section> Estimado (a) {0}<BR/> <p>Se denegó una solicitud de vacaciones</p></section>", empPrinc.nomComEmp);
            string tituloE = "Denegación de solicitud de Vacaciones";
            mE.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensajeE, tituloE, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            //envio mensaje al usuario receptor
            EmailHelper mR = new EmailHelper();
            string mensajeR = string.Format("<section> Estimado (a) {0}<BR/> <p>Se denegó una solicitud de vacaciones a {1}</p></section>", empJefe.nomComEmp, empPrinc.nomComEmp);
            string tituloR = "Denegación de solicitud de Vacaciones";
            mR.SendEmail(/*model.solicitante.email*/usuJefe.email, mensajeR, tituloR, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            return Json(variable, JsonRequestBehavior.AllowGet);
        }


    }

}