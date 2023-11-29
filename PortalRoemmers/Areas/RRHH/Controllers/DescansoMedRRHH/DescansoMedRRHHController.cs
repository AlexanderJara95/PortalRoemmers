using PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH;
using PortalRoemmers.Areas.RRHH.Services.SolicitudRRHH;
using PortalRoemmers.Areas.RRHH.Services.DescansoMedRRHH;
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
using System.IO;
using System.Text;

namespace PortalRoemmers.Areas.RRHH.Controllers.DescansoMedRRHH
{
    public class DescansoMedRRHHController : Controller
    {
        // GET: RRHH/DescansoMedRRHH
        private DescansoMedRRHHRepositorio _des;
        private UsuarioRepositorio _usu;
        private EmpleadoRepositorio _emp;
        private SubtipoSolicitudRRHHRepositorio _stip;
        private Ennumerador enu;
        private Parametros p;

        public DescansoMedRRHHController()
        {
            _emp = new EmpleadoRepositorio();
            _des = new DescansoMedRRHHRepositorio();
            _stip = new SubtipoSolicitudRRHHRepositorio();
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


            // validación de rango de fechas del año actual
            var modelCant = _des.obtenerTodos(pagina, search, ConstantesGlobales.tipoDescansos, primero.ToString(), actual.ToString());
            
            // filtrado general
            var model = _des.obtenerTodos(pagina, search, ConstantesGlobales.tipoDescansos, inicio.ToString(), fin.ToString());
            ViewBag.SolicitudAdmin = validarSolicitudAdmin(model.SoliRRHH);
            ViewBag.diasUtilizados = modelCant.SoliRRHH.Where(x => x.idAccApro != SessionPersister.UserId).Count();
            //-----------------------------
            return View(model);
        }

        [CustomAuthorize(Roles = "000003,000406")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.procedeA = true;
            //dias restantes sin el inicio y final actual

            SolicitudRRHHModels model = new SolicitudRRHHModels();
            model.idAccSol = SessionPersister.UserId;
            ViewBag.SubtipoSoliRrhh = new SelectList(_stip.obtenerSubtipoSoliRrhh(ConstantesGlobales.tipoDescansos), "idSubTipoSolicitudRrhh", "nomSubtipoSolicitud");

            var actual = DateTime.Today.ToString("dd/MM/yyyy");
            ViewBag.fecha = actual;

            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(SolicitudRRHHModels model, HttpPostedFileBase file)
        {
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];

            var empJefe = _emp.obtenerItem(emple.idEmpJ);
            var usuJefe = _usu.obtenerItemXEmpleado(emple.idEmpJ);
            var usuPrinc = _usu.obtenerItemXEmpleado(emple.idEmp);
            //string responsableD = emple.idEmp + ";" + emple.apePatEmp + " " + emple.apeMatEmp + " " + emple.nom1Emp + " " + emple.nom2Emp + ";" + "";
            string tabla = "tb_SolicitudRRHH";
            int idc = enu.buscarTabla(tabla);
            model.idSolicitudRrhh = idc.ToString("D7");
            model.idSubTipoSolicitudRrhh = model.idSubTipoSolicitudRrhh;
            model.idEstado = ConstantesGlobales.estadoRegistrado;
            model.idAccSol = SessionPersister.UserId;
            model.usuCrea = SessionPersister.Username;
            model.usufchCrea = DateTime.Now;
            model.periodo = DateTime.Now.Year.ToString();

            

            UserSolicitudRRHHModels userSoliRRHH = new UserSolicitudRRHHModels();
            userSoliRRHH.idSolicitudRrhh = model.idSolicitudRrhh;
            userSoliRRHH.idAccRes = SessionPersister.UserId;
            userSoliRRHH.usuCrea = model.usuCrea;
            userSoliRRHH.usufchCrea = model.usufchCrea;

            //imagen
            if (file != null && file.ContentLength > 0)//Seleccione una imagen
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    imageData = binaryReader.ReadBytes(file.ContentLength);
                }
                //setear la imagen a la entidad que se creara
                model.documentoAdjunto = imageData;
            }
            else
            {
                model.documentoAdjunto = Encoding.Default.GetBytes(SessionPersister.UserIma);
            }

            if (emple.idEmpJ != "" || emple.idEmpJ != null)
            {
                model.idAccApro = _usu.obtenerItemXEmpleado(emple.idEmpJ).idAcc;
                try
                {
                    if (_des.crear(model))
                    {
                        enu.actualizarTabla(tabla, idc);
                        TempData["mensaje"] = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                        _des.crearUserSoliRrhh(userSoliRRHH);
                        //envio mensaje al usuario emisor
                        EmailHelper m = new EmailHelper();
                        string mensaje = string.Format("<section> Estimado (a) {0}<BR/> <p>Se registró una solicitud de descanso médico</p></section>", emple.nomComEmp);
                        string titulo = "Solicitud de Descanso Médico";
                        m.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensaje, titulo, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

                        //envio mensaje al usuario receptor
                        EmailHelper m1 = new EmailHelper();
                        string mensaje1 = string.Format("<section> Estimado (a) {0}<BR/> <p>Nueva solicitud de descanso médico de {1}</p></section>", empJefe.nomComEmp, emple.nomComEmp);
                        string titulo1 = "Solicitud de Descanso Médico";
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

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000407")]
        public ActionResult Modificar(string id)
        {
            var model = _des.obtenerItem(id);
            ViewBag.fechaIni = model.fchIniSolicitud.ToShortDateString(); 
            ViewBag.fechaFin = model.fchFinSolicitud.ToShortDateString();
            ViewBag.SubtipoSoliRrhh = new SelectList(_stip.obtenerSubtipoSoliRrhh(ConstantesGlobales.tipoDescansos), "idSubTipoSolicitudRrhh", "nomSubtipoSolicitud");
            ViewBag.SelectedSubtipoSoliRrhh = model.idSubTipoSolicitudRrhh; // Valor del elemento a seleccionar
            return View(model);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(SolicitudRRHHModels model, HttpPostedFileBase file)
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
            //imagen
            if (file != null && file.ContentLength > 0)//Seleccione una imagen
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    imageData = binaryReader.ReadBytes(file.ContentLength);
                }
                //setear la imagen a la entidad que se creara
                model.documentoAdjunto = imageData;
            }
            else
            {
                model.documentoAdjunto = Encoding.Default.GetBytes(SessionPersister.UserIma);
            }

            try
            {
                if (_des.modificar(model))
                {
                    //envio mensaje al usuario emisor
                    EmailHelper mE = new EmailHelper();
                    string mensajeE = string.Format("<section> Estimado (a) {0}<BR/> <p>Se modificó una solicitud de Descando Médico</p></section>", emple.nomComEmp);
                    string tituloE = "Solicitud de Descanso Médico";
                    mE.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensajeE, tituloE, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

                    //envio mensaje al usuario receptor
                    EmailHelper mR = new EmailHelper();
                    string mensajeR = string.Format("<section> Estimado (a) {0}<BR/> <p>Se modificó una solicitd de Descanso Médico de {1}</p></section>", empJefe.nomComEmp, emple.nomComEmp);
                    string tituloR = "Solicitud de Descanso Médico";
                    mR.SendEmail(/*model.solicitante.email*/usuJefe.email, mensajeR, tituloR, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

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
            var model = _des.obtenerItem(id);
            ViewBag.fechaIni = model.fchIniSolicitud.ToShortDateString();
            ViewBag.fechaFin = model.fchFinSolicitud.ToShortDateString();
            int dias = diferenciaDias(model.fchIniSolicitud, model.fchFinSolicitud);
            //traer nombre subtipo id
            var subtipo = _stip.obtenerSubtipoSoliRrhhId(model.idSubTipoSolicitudRrhh);
            ViewBag.SelectedSubtipoSoliRrhh = subtipo.nomSubtipoSolicitud;
            ViewBag.diasTotales = dias;
            return View(model);
        }

        [SessionAuthorize]
        public ActionResult convertirImagen(string idSolicitudRrhh)
        {
            var imagen =  _des.obtenerItem(idSolicitudRrhh);

            if (imagen.documentoAdjunto == null)
            {
                string locacion = Server.MapPath("~/Areas/RRHH/FotoDescanso/desc-medico.jpg");
                FileStream foto = new FileStream(locacion, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                Byte[] arreglo = new Byte[foto.Length];
                BinaryReader reader = new BinaryReader(foto);
                arreglo = reader.ReadBytes(Convert.ToInt32(foto.Length));
                imagen.documentoAdjunto = arreglo;
                reader.Close();
            }
            return File(imagen.documentoAdjunto, "image/jpeg");
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


        public int diferenciaDias(DateTime fechaInicio, DateTime fechaFin)
        {
            TimeSpan duracion = fechaFin.Subtract(fechaInicio);
            int diferenciaDias = (int)duracion.TotalDays + 1;

            return diferenciaDias;
        }
        
        public JsonResult anularSolicitud(string idSolicitudRRHH)
        {
            var variable = _des.updateEstadoSoliRRHH(idSolicitudRRHH, ConstantesGlobales.estadoAnulado);

            var usuPrinc = _usu.obtenerItem(SessionPersister.UserId);
            var empPrinc = _emp.obtenerItem(usuPrinc.idEmp);
            var usuJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);
            var empJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);

            //envio mensaje al usuario emisor
            EmailHelper mE = new EmailHelper();
            string mensajeE = string.Format("<section> Estimado (a) {0}<BR/> <p>Se anuló la solicitud de descanso médico</p></section>", empPrinc.nomComEmp);
            string tituloE = "Anulación de solicitud de Descanso Médico";
            mE.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensajeE, tituloE, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            //envio mensaje al usuario receptor
            EmailHelper mR = new EmailHelper();
            string mensajeR = string.Format("<section> Estimado (a) {0}<BR/> <p>Se anuló la solicitud de descanso médico a {1}</p></section>", empJefe.nomComEmp, empPrinc.nomComEmp);
            string tituloR = "Anulación de solicitud de Descanso Médico";
            mR.SendEmail(/*model.solicitante.email*/usuJefe.email, mensajeR, tituloR, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            return Json(variable, JsonRequestBehavior.AllowGet);
        }
        //9
        public JsonResult aprobarSolicitud(string idSolicitudRRHH)
        {
            var variable = _des.updateEstadoSoliRRHH(idSolicitudRRHH, ConstantesGlobales.estadoAprobado);

            var usuPrinc = _usu.obtenerItem(SessionPersister.UserId);
            var empPrinc = _emp.obtenerItem(usuPrinc.idEmp);
            var usuJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);
            var empJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);

            //envio mensaje al usuario emisor
            EmailHelper mE = new EmailHelper();
            string mensajeE = string.Format("<section> Estimado (a) {0}<BR/> <p>Se aprobó una solicitud de descanso médico</p></section>", empPrinc.nomComEmp);
            string tituloE = "Aprobación de solicitud de Descanso Médico";
            mE.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensajeE, tituloE, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            //envio mensaje al usuario receptor
            EmailHelper mR = new EmailHelper();
            string mensajeR = string.Format("<section> Estimado (a) {0}<BR/> <p>Se aprobó una solicitud de descanso médico a {1}</p></section>", empJefe.nomComEmp, empPrinc.nomComEmp);
            string tituloR = "Aprobación de solicitud de Descanso Médico";
            mR.SendEmail(/*model.solicitante.email*/usuJefe.email, mensajeR, tituloR, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            return Json(variable, JsonRequestBehavior.AllowGet);
        }
        //12
        public JsonResult rechazarSolicitud(string idSolicitudRRHH)
        {
            var variable = _des.updateEstadoSoliRRHH(idSolicitudRRHH, ConstantesGlobales.estadoRechazado);

            var usuPrinc = _usu.obtenerItem(SessionPersister.UserId);
            var empPrinc = _emp.obtenerItem(usuPrinc.idEmp);
            var usuJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);
            var empJefe = _usu.obtenerItemXEmpleado(empPrinc.idEmpJ);

            //envio mensaje al usuario emisor
            EmailHelper mE = new EmailHelper();
            string mensajeE = string.Format("<section> Estimado (a) {0}<BR/> <p>Se denegó una solicitud de descanso médico</p></section>", empPrinc.nomComEmp);
            string tituloE = "Denegación de solicitud de Descanso Médico";
            mE.SendEmail(/*model.solicitante.email*/ usuPrinc.email, mensajeE, tituloE, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            //envio mensaje al usuario receptor
            EmailHelper mR = new EmailHelper();
            string mensajeR = string.Format("<section> Estimado (a) {0}<BR/> <p>Se denegó una solicitud de descanso médico a {1}</p></section>", empJefe.nomComEmp, empPrinc.nomComEmp);
            string tituloR = "Denegación de solicitud de Descanso Médico";
            mR.SendEmail(/*model.solicitante.email*/usuJefe.email, mensajeR, tituloR, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

            return Json(variable, JsonRequestBehavior.AllowGet);
        }


    }

}