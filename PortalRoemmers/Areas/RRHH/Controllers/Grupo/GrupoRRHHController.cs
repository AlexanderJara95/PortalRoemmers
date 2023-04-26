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

namespace PortalRoemmers.Areas.RRHH.Controllers.Grupo
{
    public class GrupoRRHHController : Controller
    {
        // GET: RRHH/SolicitudRRHH
        private GrupoRRHHRepositorio _gru;
        private UsuarioRepositorio _usu;
        private EmpleadoRepositorio _emp;
        private Ennumerador enu;
        private Parametros p;

        public GrupoRRHHController()
        {
            _gru = new GrupoRRHHRepositorio();
            _usu = new UsuarioRepositorio();
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
                DateTime ultimoDelanio = new DateTime(date.Year, 12, 31);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = ultimoDelanio.ToString("dd/MM/yyyy");
                /*var actual = DateTime.Today.ToString("dd/MM/yyyy");*/

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
            ViewBag.userId = SessionPersister.UserId;

            //-----------------------------
            //var parametro = p.selectResultado(ConstantesGlobales.Com_Usu_Pre_Cas_03).ToList();
            //var usuario = _usu.obtenerUsuarios().ToList();
            //var result = parametro.Join(usuario, e => e.value, d => d.idAcc, (e, d) => new { e.value, d.empleado.nomComEmp });
            //-----------------------------
            //ViewBag.gerentesProd = new SelectList(result.Select(x => new { idAccResGP = x.value, nombre = x.nomComEmp }), "idAccResGP", "nombre");
            //ViewBag.actividades = new SelectList(_act.obtenerActividades().Where(x => x.idAccRes == idAcc && x.estimacion != null && ((DateTime.Today >= x.fchIniVig) && (DateTime.Today <= x.fchFinVig))).Select(x => new { idActividades = x.idActiv, nomActiv = x.nomActiv }), "idActividades", "nomActiv");
            //-----------------------------

            var model = _gru.obtenerTodos(pagina, search, ConstantesGlobales.tipoVacaciones, inicio.ToString(), fin.ToString());
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

            GrupoRRHHModels model = new GrupoRRHHModels();
            model.usuCrea = SessionPersister.UserId;

            ///-----------------------
            ///----------------------- 
            var actual = DateTime.Today.ToString("dd/MM/yyyy");

            ViewBag.fecha = actual;

            return View();
        }

        /*
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(GrupoRRHHModels model, int diasRestantes)
        {
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];
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
            if (validarLimiteVacaciones(model.fchIniSolicitud, model.fchFinSolicitud, diasRestantes))
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
                            _soli.crearUserSolRrhh(userSoliRRHH);
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
            }
            else
            {
                TempData["mensaje"] = "<div id='warning' class='alert alert-warning'>" + "Has superado la cantidad de días disponibles" + "</div>";
            }

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });

        }
        */



        /*
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000407")]
        public ActionResult Modificar(string id, string diasRestantes)
        {
            var model = _soli.obtenerItem(id);
            ViewBag.fechaIni = model.fchIniSolicitud.ToShortDateString();
            ViewBag.fechaFin = model.fchFinSolicitud.ToShortDateString();
            int diasHabiles = calcularDiasHabiles(model.fchIniSolicitud, model.fchFinSolicitud);
            //dias restantes sin el inicio y final actual
            ViewBag.diasRestantes = Convert.ToInt32(diasRestantes) + diasHabiles;
            model.idSubTipoSolicitudRrhh = model.idSubTipoSolicitudRrhh;
            model.usufchMod = DateTime.Now;
            model.usuMod = SessionPersister.Username;
            return View(model);
        }*/


        /*
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(GrupoRRHHModels model, int diasRestantes)
        {
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];
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
            }
            else
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + "Has superado la cantidad de días disponibles" + "</div>";
            }

            TempData["mensaje"] = mensaje;

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }*/


    }

}