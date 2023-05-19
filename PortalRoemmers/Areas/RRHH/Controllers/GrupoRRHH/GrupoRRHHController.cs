using Newtonsoft.Json;
using PortalRoemmers.Areas.RRHH.Models.Grupo;
using PortalRoemmers.Areas.RRHH.Services.Grupo;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.Grupo
{
    public class GrupoRRHHController : Controller
    {
        // GET: RRHH/SolicitudRRHH
        private GrupoRRHHRepositorio _gru;
        private AreaRoeRepositorio _are;
        private UsuarioRepositorio _usu;
        private EmpleadoRepositorio _emp;
        private Ennumerador enu;
        private Parametros p;

        public GrupoRRHHController()
        {
            _gru = new GrupoRRHHRepositorio();
            _are = new AreaRoeRepositorio();
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
        public ActionResult Registrar()
        {
            ViewBag.Areas = new SelectList(_are.obtenerArea(), "idAreRoe", "nomAreRoe");

            var opcionesAutocompletar = new List<string>();
            foreach (var area in ViewBag.Areas)
            {
                opcionesAutocompletar.Add(area.Text);
            }

            ViewBag.OpcionesAutocompletar = JsonConvert.SerializeObject(opcionesAutocompletar);

            GrupoRRHHModels model = new GrupoRRHHModels();
            model.usuCrea = SessionPersister.UserId;

            ///-----------------------
            ///----------------------- 
            var actual = DateTime.Today.ToString("dd/MM/yyyy");

            ViewBag.fecha = actual;

            return View();
        }

        public JsonResult ObtenerTodasLasAreas(string term)
        {
            var areas = _are.obtenerArea()
                .Where(a => a.nomAreRoe.ToLower().Contains(term.ToLower()))
                .Select(a => new { id = a.idAreRoe, text = a.nomAreRoe })
                .ToList();
            return Json(areas, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerTodosLosUsuarios(string term, List<string> list)
        {

            var filtroUsuarios = _usu.obtenerUsuarios()
                .Where(a => list.Contains(a.empleado.idAreRoe)).ToList();
            
            var usuarios = filtroUsuarios
                .Where(a => a.username.ToLower().Contains(term.ToLower()) || a.empleado.nomComEmp.ToLower().Contains(term.ToLower()))
                .Select(a => new { id = a.idAcc, text = (a.empleado.nomComEmp + " (" + a.username + ")") })
                .ToList();

            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(List<string> areas, List<string> usuarios, string descripcion)
        {
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];
            string tabla = "tb_GrupoRRHH";
            GrupoRRHHModels model = new GrupoRRHHModels();
            int idc = enu.buscarTabla(tabla);
            model.idGrupoRrhh = idc.ToString("D7");
            model.descGrupo = descripcion;
            model.usuCrea = SessionPersister.Username;
            model.usufchCrea = DateTime.Now;
            model.idEstado = ConstantesGlobales.estadoActivo;

            try
            {
                if (_gru.crear(model))
                {
                    enu.actualizarTabla(tabla, idc);
                    foreach (var area in areas)
                    {
                        AreaGrupoRRHHModels areaGrupoRRHH = new AreaGrupoRRHHModels();
                        areaGrupoRRHH.idGrupoRrhh = model.idGrupoRrhh;
                        areaGrupoRRHH.idAreRoe = area;
                        areaGrupoRRHH.usuCrea = model.usuCrea;
                        areaGrupoRRHH.usufchCrea = model.usufchCrea;
                        _gru.crearAreaGrupoRrhh(areaGrupoRRHH);
                    }
                    foreach (var usuario in usuarios)
                    {
                        ExcluGrupoRRHHModels excluGrupoRRHH = new ExcluGrupoRRHHModels();
                        excluGrupoRRHH.idGrupoRrhh = model.idGrupoRrhh;
                        excluGrupoRRHH.idAcc = usuario;
                        excluGrupoRRHH.usuCrea = model.usuCrea;
                        excluGrupoRRHH.usufchCrea = model.usufchCrea;
                        _gru.crearExcluGrupoRrhh(excluGrupoRRHH);
                    }
                    TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";

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

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });

        }
                        
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000407")]
        public ActionResult Visualizar(string id)
        {
            var model = _gru.obtenerItem(id);
            model.areas = _gru.obtenerAreaGrupoRrhh(id);
            model.excluidos = _gru.obtenerExcluGrupoRrhh(id);
            return View(model);
        }

        public JsonResult anularGrupo(string idGrupoRRHH)
        {
            var variable = _gru.updateEstadoGrupo(idGrupoRRHH, ConstantesGlobales.estadoAnulado);
            return Json(variable, JsonRequestBehavior.AllowGet);
        }

        public JsonResult activarGrupo(string idGrupoRRHH)
        {
            var variable = _gru.updateEstadoGrupo(idGrupoRRHH, ConstantesGlobales.estadoActivo);
            return Json(variable, JsonRequestBehavior.AllowGet);
        }


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

            /*if (validarLimiteVacaciones(model.fchIniSolicitud, model.fchFinSolicitud, diasRestantes))
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
            }*/

            TempData["mensaje"] = mensaje;

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }


    }

}