using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Equipo;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using PortalRoemmers.Services;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Usuario
{
    public class UsuarioController : Controller
    {//USUARIOCONTROLLER  000169
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
        private NivelAproRepositorio _niv;
        private AccountRepositorio _am ;
        private PaisRepositorio _pai;
        private SangreRepositorio _san;
        private BancoRepositorio _ban;
        private MonedaRepositorio _mon;
        private SedeRepositorio _sed;
        private TipoFamiliaRepositorio _tipfam;
        private NivelEstudioRepositorio _nivest;
        private EstudioEmpleadoRepositorio _estemp;
        private FamiliaEmpleadoRepositorio _famemp;

        public UsuarioController()
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
            _niv = new NivelAproRepositorio();
            _am = new AccountRepositorio();
            _pai = new PaisRepositorio();
            _san = new SangreRepositorio();
            _ban= new BancoRepositorio();
            _mon = new MonedaRepositorio();
            _sed = new SedeRepositorio();
            _tipfam = new TipoFamiliaRepositorio();
            _nivest = new NivelEstudioRepositorio();
            _estemp= new EstudioEmpleadoRepositorio();
            _famemp = new FamiliaEmpleadoRepositorio();

        }

        // Listar Usuarios Listar
        [CustomAuthorize(Roles = "000003,000170")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _usu.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar usuario
        [CustomAuthorize(Roles = "000003,000171")]
        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.nivelApro = new SelectList(_niv.obtenerNivelApro().Select(x => new { x.idNapro, nombre = x.idNapro + "-" + x.nomNapro }), "idNapro", "nombre");
            ViewBag.EmpleadoR = new SelectList(_emp.obtenerEmpleados(), "idEmp", "nomComEmp");
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(UsuarioModels account, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                //cargo la imagen
                if (file != null && file.ContentLength > 0)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(file.ContentLength);
                    }
                    //setear la imagen a la entidad que se creara
                    account.rutaImgPer = imageData;
                }


                string mensaje = "";
                Boolean valor = _usu.crear(account);

                if (valor)
                {
                    try { 
                    //generar token
                    string token = _am.generarToken(account.idAcc);
                    EmailHelper m = new EmailHelper();
                    string titulo = "Confirmación de correo electrónico";
                    string mensajec = string.Format("<section> Estimado {0}<BR/> <p>Gracias por su registro, por favor haga clic en el siguiente enlace para completar su registro:</p><BR/>  <a href=\"{1}\" title=\"Confirmar correo electrónico del usuario\">{1}</a> </section>", account.nomComEmp, Url.Action("ConfirmEmail", "Account", new { Token = token, Area = "" }, Request.Url.Scheme));
                    m.SendEmail(account.email, mensajec, titulo, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);
                    mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
                    }
                    catch (Exception e)
                    {
                        mensaje = "<div id='warning' class='alert alert-warning'>Se produjo un error enviar al enviar el correo </div>";
                    }

                }
                else {
                    mensaje = "<div id='warning' class='alert alert-warning'> Se produjo un error al crear el usuario </div>";
                }

                TempData["mensaje"] = mensaje;

                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

            ViewBag.nivelApro = new SelectList(_niv.obtenerNivelApro().Select(x => new { x.idNapro, nombre = x.idNapro + "-" + x.nomNapro }), "idNapro", "nombre");
            ViewBag.EmpleadoR = new SelectList(_emp.obtenerEmpleados(), "idEmp", "nomComEmp");

            return View(account);
        }
        //modificar usuario
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000172")]
        public ActionResult Modificar(string id) {
            var usu = _usu.obtenerItem(id);
            usu.confirmPassword = usu.userpass;
            ViewBag.nivelApro = new SelectList(_niv.obtenerNivelApro().Select(x => new { x.idNapro, nombre = x.idNapro + "-" + x.nomNapro }), "idNapro", "nombre",usu.idNapro);
            ViewBag.EstU = new SelectList(_est.obteneEstadoUsuario(), "idEst", "nomEst", usu.idEst);
            return View(usu);

        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(UsuarioModels account, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)//Seleccione una imagen
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    imageData = binaryReader.ReadBytes(file.ContentLength);
                }
                //setear la imagen a la entidad que se creara
                account.rutaImgPer = imageData;
            }
            else
            {
                var usu = _usu.obtenerItem(account.idAcc);
                account.rutaImgPer = usu.rutaImgPer;
            }

            if (ModelState.IsValid)
            {
                TempData["mensaje"] = _usu.modificar(account);
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }

            ViewBag.nivelApro = new SelectList(_niv.obtenerNivelApro().Select(x => new { x.idNapro, nombre = x.idNapro + "-" + x.nomNapro }), "idNapro", "nombre", account.idNapro);
            ViewBag.EstU = new SelectList(_est.obteneEstadoUsuario(), "idEst", "nomEst", account.idEst);;
            return View(account);
        }
        //Eliminar usuario
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000173")]
        public ActionResult Eliminar(string id)
        {
            var usu = _usu.obtenerItem(id);
            ViewBag.tipDoc = new SelectList(_ide.obtenerTipDocs(), "idTipDoc", "nomTipDoc", usu.empleado.idTipDoc);
            ViewBag.cargo = new SelectList(_car.obtenerCargo(), "idCarg", "desCarg", usu.empleado.idCarg);
            return View(usu);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(UsuarioModels account)
        {
            TempData["mensaje"] = _usu.updateEstUsu(account.idAcc);
            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "ALL")]
        public ActionResult PerfilUsuario(string idAcc)
        {
            SessionPersister.ActiveMenu = string.Empty;
            SessionPersister.ActiveVista = string.Empty;
            var usu = _usu.obtenerItem(idAcc);
            string pais = "";
            string depa = "";
            string prov = "";
            string dist = "";
            if (usu.empleado.cCod_Ubi!=null)
            {
                pais = usu.empleado.cCod_Ubi.Substring(0, 2);
                depa = usu.empleado.cCod_Ubi.Substring(2, 2);
                prov = usu.empleado.cCod_Ubi.Substring(4, 2);
                dist = usu.empleado.cCod_Ubi.Substring(6, 2);
            }        
            usu.confirmPassword = usu.userpass;

            //cargar detalle
            var estudios = _estemp.obtenerEstudiosEmpleado(usu.idEmp);
            ViewBag.tbEstudio = cargarTablaEstudio(estudios);

            //cargar familia
            var familia = _famemp.obtenerFamiliasEmpleado(usu.idEmp);
            ViewBag.tb_Familia = cargarTablaFamilia(familia);

            //personal
            ViewBag.tipDoc = new SelectList(_ide.obtenerTipDocs(), "idTipDoc", "nomTipDoc", usu.empleado.idTipDoc);
            ViewBag.estado = new SelectList(_civ.obtenerEstCi(), "idEstCiv", "nomEstCiv", usu.empleado.idEstCiv);
            ViewBag.pais = new SelectList(_ubic.ubicacionPersonal().Select(x => new { x.cCod_Pais, x.cPais }).Distinct(), "cCod_Pais", "cPais", pais);
            ViewBag.departamento = new SelectList(_ubic.ubicacionPersonal().Where(x => x.cCod_Pais == pais).Select(y => new { y.cCod_Dpto, y.cDepartamento }).Distinct().OrderBy(x => x.cDepartamento), "cCod_Dpto", "cDepartamento", depa);
            ViewBag.provincia = new SelectList(_ubic.ubicacionPersonal().Where(x => x.cCod_Dpto == depa && x.cCod_Pais == pais).Select(y => new { y.cCod_Provincia, y.cProvincia }).Distinct().OrderBy(x => x.cProvincia), "cCod_Provincia", "cProvincia", prov);
            ViewBag.ubicacion = new SelectList(_ubic.ubicacionPersonal().Where(x => x.cCod_Provincia == prov && x.cCod_Pais == pais && x.cCod_Dpto == depa).Select(y => new { y.cCod_Ubi, y.cDistrito }).Distinct().OrderBy(x => x.cDistrito), "cCod_Ubi", "cDistrito", dist);
            ViewBag.sexoUsu = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen", usu.empleado.idGen);
            ViewBag.nacio = new SelectList(_pai.obtenerPaises(), "idPais", "nomPais", usu.empleado.idPais);
            ViewBag.sangre = new SelectList(_san.obtenerSangre(), "idSan", "nomSan", usu.empleado.idSan);
            ViewBag.vivPad = new SelectList(ListadoSioNo(), "Value", "Text", usu.empleado.vivConPadEmp);
            ViewBag.tinhij = new SelectList(ListadoSioNo(), "Value", "Text", usu.empleado.hijTieEmp);
            //laboral
            ViewBag.Afp = new SelectList(_afp.obteneAfpUsu(), "idAfp", "nomAfp", usu.empleado.idAfp);
            ViewBag.tipcomi = new SelectList(ListadoTipoComision(), "Value", "Text", usu.empleado.comAfiEmp);
            ViewBag.tipseg = new SelectList(ListadoTipoSeguro(), "Value", "Text", usu.empleado.segSalEmp);
            ViewBag.bancocts = new SelectList(_ban.obtenerBancos(), "idBan", "nomBan", usu.empleado.idBancts);
            ViewBag.bancosuel = new SelectList(_ban.obtenerBancos(), "idBan", "nomBan", usu.empleado.idBansuel);
            ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "nomMon", usu.empleado.idMon);
            ViewBag.sede = new SelectList(_sed.obtenerSedes(), "cod_sede", "nom_sede", usu.empleado.cod_sede);
            //ESTUDIO
            ViewBag.nivelEstudio = new SelectList(_nivest.obtenerListadoNivelEstudio(), "idNivEstu", "nomNivEstu");
            ViewBag.curAct = new SelectList(ListadoEstadoED(), "Value", "Text");
            ViewBag.desAnio = new SelectList(ListadoMes(), "Value", "Text");
            ViewBag.hasAnio = new SelectList(ListadoMes(), "Value", "Text");
            //FAMILIA
            ViewBag.tipoFamilia = new SelectList(_tipfam.obtenerListadoTipoFamilia(), "idTipFam", "nomTipFa");

            return View(usu);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult PerfilUsuario(UsuarioModels account, HttpPostedFileBase file, string estudioDet,string familiaDet) {

            //Detalle Estudio
            if (estudioDet.Length!=0)
            {
                _estemp.eliminarCmd(account.idEmp);

                EstudioEmpleadoModels model = new EstudioEmpleadoModels();
                List<EstudioEmpleadoModels> listmodel = new List<EstudioEmpleadoModels>();
                var  array= listaDetalle(estudioDet);

                foreach (var i in array)
                {
                    string[] item = i.Split(';');
                    model.idEmp = account.idEmp;
                    model.idEstu = item[0];
                    model.nomInsEstu = item[1];
                    model.idNivEstu = item[2];
                    model.carProEstu = item[3];
                    model.curActEstu = item[4];
                    model.mesDesEstu = item[5];
                    model.anioDesEstu = item[6];
                    model.mesHasEstu = item[7];
                    model.anioHasEstu = item[8];
                    model.usuCrea = SessionPersister.Username;
                    model.usufchCrea = DateTime.Now;
                    listmodel.Add(model);
                    model = new EstudioEmpleadoModels();
                }
                _estemp.crear(listmodel);
            }

            //Detalle Familia
            if (familiaDet.Length != 0)
            {
                _famemp.eliminarCmd(account.idEmp);

                FamiliaEmpleadoModels model = new FamiliaEmpleadoModels();
                List<FamiliaEmpleadoModels> listmodel = new List<FamiliaEmpleadoModels>();
                var array = listaDetalle(familiaDet);

                foreach (var i in array)
                {
                    string[] item = i.Split(';');
                    model.idEmp = account.idEmp;
                    model.dniEmpFam = item[0];
                    model.nomComEmpFam = item[1];
                    model.fchNacEmpFam = DateTime.Parse(item[2]);
                    model.idTipFam = item[3];
                    model.ocuEmpFam = item[4];
                    model.usuCrea = SessionPersister.Username;
                    model.usufchCrea = DateTime.Now;
                    listmodel.Add(model);
                    model = new FamiliaEmpleadoModels();
                }
                _famemp.crear(listmodel);
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
                account.rutaImgPer = imageData;
            }
            else {
                account.rutaImgPer = Encoding.Default.GetBytes(SessionPersister.UserIma);
            }
            //datos empleado
            if (ModelState.IsValid)
            {
                account.empleado.nomComEmp = account.empleado.apePatEmp + " " + account.empleado.apeMatEmp + " " + account.empleado.nom1Emp + " " + account.empleado.nom2Emp;
                TempData["mensaje"] = _usu.modificar(account)+" "+_emp.modificar(account.empleado);
            }
            else {
                string mensaje = "<div id='warning' class='alert alert-warning'>Verificar los datos.</div>";
                TempData["mensaje"] = mensaje;
            }

            var usu = _usu.obtenerItem(account.idAcc);

            string pais = "";
            string depa = "";
            string prov = "";
            string dist = "";
            if (usu.empleado.cCod_Ubi != null)
            {
                pais = usu.empleado.cCod_Ubi.Substring(0, 2);
                depa = usu.empleado.cCod_Ubi.Substring(2, 2);
                prov = usu.empleado.cCod_Ubi.Substring(4, 2);
                dist = usu.empleado.cCod_Ubi.Substring(6, 2);
            }

            //personal
            ViewBag.tipDoc = new SelectList(_ide.obtenerTipDocs(), "idTipDoc", "nomTipDoc", usu.empleado.idTipDoc);
            ViewBag.estado = new SelectList(_civ.obtenerEstCi(), "idEstCiv", "nomEstCiv", usu.empleado.idEstCiv);
            ViewBag.pais = new SelectList(_ubic.ubicacionPersonal().Select(x => new { x.cCod_Pais, x.cPais }).Distinct(), "cCod_Pais", "cPais", pais);
            ViewBag.departamento = new SelectList(_ubic.ubicacionPersonal().Where(x => x.cCod_Pais == pais).Select(y => new { y.cCod_Dpto, y.cDepartamento }).Distinct().OrderBy(x => x.cDepartamento), "cCod_Dpto", "cDepartamento",depa);
            ViewBag.provincia = new SelectList(_ubic.ubicacionPersonal().Where(x => x.cCod_Dpto == depa && x.cCod_Pais == pais).Select(y => new { y.cCod_Provincia, y.cProvincia }).Distinct().OrderBy(x => x.cProvincia), "cCod_Provincia", "cProvincia", prov);
            ViewBag.ubicacion = new SelectList(_ubic.ubicacionPersonal().Where(x => x.cCod_Provincia == prov && x.cCod_Pais == pais && x.cCod_Dpto ==depa).Select(y => new { y.cCod_Ubi, y.cDistrito }).Distinct().OrderBy(x => x.cDistrito), "cCod_Ubi", "cDistrito", dist);
            ViewBag.sexoUsu = new SelectList(_gen.obteneGeneUsu(), "idGen", "nomGen", usu.empleado.idGen);
            ViewBag.nacio = new SelectList(_pai.obtenerPaises(), "idPais", "nomPais", usu.empleado.idPais);
            ViewBag.sangre = new SelectList(_san.obtenerSangre(), "idSan", "nomSan", usu.empleado.idSan);
            ViewBag.vivPad = new SelectList(ListadoSioNo(), "Value", "Text", usu.empleado.vivConPadEmp);
            ViewBag.tinhij = new SelectList(ListadoSioNo(), "Value", "Text", usu.empleado.hijTieEmp);
            
            //laboral
            ViewBag.Afp = new SelectList(_afp.obteneAfpUsu(), "idAfp", "nomAfp", usu.empleado.idAfp);
            ViewBag.tipcomi = new SelectList(ListadoTipoComision(), "Value", "Text", usu.empleado.comAfiEmp);
            ViewBag.tipseg = new SelectList(ListadoTipoSeguro(), "Value", "Text", usu.empleado.segSalEmp);
            ViewBag.bancocts = new SelectList(_ban.obtenerBancos(), "idBan", "nomBan", usu.empleado.idBancts);
            ViewBag.bancosuel = new SelectList(_ban.obtenerBancos(), "idBan", "nomBan", usu.empleado.idBansuel);
            ViewBag.moneda = new SelectList(_mon.obteneMonedas(), "idMon", "nomMon", usu.empleado.idMon);
            ViewBag.sede = new SelectList(_sed.obtenerSedes(), "cod_sede", "nom_sede", usu.empleado.cod_sede);

            //ESTUDIO
            ViewBag.nivelEstudio = new SelectList(_nivest.obtenerListadoNivelEstudio(), "idNivEstu", "nomNivEstu");
            ViewBag.curAct = new SelectList(ListadoEstadoED(), "Value", "Text");
            ViewBag.desAnio = new SelectList(ListadoMes(), "Value", "Text");
            ViewBag.hasAnio = new SelectList(ListadoMes(), "Value", "Text");
            //FAMILIA
            ViewBag.tipoFamilia = new SelectList(_tipfam.obtenerListadoTipoFamilia(), "idTipFam", "nomTipFa");

            //cargar detalle
            var estudios = _estemp.obtenerEstudiosEmpleado(usu.idEmp);
            ViewBag.tbEstudio = cargarTablaEstudio(estudios);

            //cargar familia
            var familia = _famemp.obtenerFamiliasEmpleado(usu.idEmp);
            ViewBag.tb_Familia = cargarTablaFamilia(familia);
            

            return View(usu);

        }
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000174")]
        public ActionResult AsignarAprobador (string id, string nom,string pes)
        {
            var usuarios = _usu.obtenerUsuarios().Where(x=>x.aprobacion.pesNapro>=int.Parse(pes));
            //----------------------------------------------------------------------------
            //modificado 29/01/2020 - se agrega otra variable "asigAproxAprob" para tener dos variables, uno por aprobador y otro general
            var asigAproxAprob = _asiapr.obtenerAprobadores(id);
            //var asigApro = _asiapr.obtenerTodosAproAsig().Select(x => new { x.idAccAsig }).Distinct();
            //modificado 29/01/2020 - se agrega otra variable "seleccionadosTotal" para contemplar los que no deberian aparecer como "No Asignados"
            var seleccionados = (from u in usuarios join aa in asigAproxAprob on u.idAcc equals aa.idAccAsig  select new {u.idAcc,nombre= u.empleado.apePatEmp + " " + u.empleado.apeMatEmp + " " + u.empleado.nom1Emp + " " + u.empleado.nom2Emp });
            //var seleccionadosTotal = (from u in usuarios join aa in asigApro on u.idAcc equals aa.idAccAsig  select new {u.idAcc,nombre= u.empleado.apePatEmp + " " + u.empleado.apeMatEmp + " " + u.empleado.nom1Emp + " " + u.empleado.nom2Emp });
            //----------------------------------------------------------------------------
            string[] selec = seleccionados.Select(x => x.idAcc.ToString()).ToArray();
            var Nseleccionados = (from u in usuarios
                                  where !selec.Contains(u.idAcc.ToString())
                                  && u.idAcc != id
                                  select new { u.idAcc, nombre = u.empleado.apePatEmp + " " + u.empleado.apeMatEmp + " " + u.empleado.nom1Emp + " " + u.empleado.nom2Emp }).ToList();

            ViewBag.usuariosS = new SelectList(seleccionados, "idAcc", "nombre");
            ViewBag.usuariosNS = new SelectList(Nseleccionados, "idAcc", "nombre");
            ViewBag.aprobador =nom;
            ViewBag.peso = pes;
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult AddAsignados(string[] idAccAsigN, string id, string nombre,string peso)
        {
            string codigo = id;
            List<AsigAproModels> lista = new List<AsigAproModels>();
            AsigAproModels item = new AsigAproModels();
            if (idAccAsigN != null)
            {
                foreach (string c in idAccAsigN)
                {
                    item.idAccApro = id;
                    item.idAccAsig = c;
                    item.aproCrea = SessionPersister.Username;
                    item.aprofchCrea = DateTime.Now;
                    lista.Add(item);
                    item = new AsigAproModels();
                }
                _asiapr.crear(lista);
            }
            return RedirectToAction("AsignarAprobador", new { id = codigo, nom = nombre,pes=peso });
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult DelAsignados(string[] idAccAsig, string id, string nombre, string peso)
        {
            string codigo = id;
            if (idAccAsig!=null)
            {
                _asiapr.eliminar(idAccAsig, codigo);
            }
           
            return RedirectToAction("AsignarAprobador", new { id = codigo, nom = nombre, pes = peso });
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "ALL")]
        public ActionResult CambiarContrasenia(string idAcc)
        {
            SessionPersister.ActiveMenu = string.Empty;
            SessionPersister.ActiveVista = string.Empty;
            return View();
        }

        [CustomAuthorizeJson(Roles = "000003,000343")]
        public JsonResult updatePassword(string idAcc, string userpass, string nomComEmp, string email)
        {
            string mensaje = "";
            string passwEn = "";
            using (MD5 md5Hash = MD5.Create())
            {
                passwEn = _usu.GetMd5Hash(md5Hash, userpass);
            }
            Boolean estado = _usu.updatePassword(idAcc, passwEn);

            if (estado)
            {
                try
                {
                    EmailHelper m = new EmailHelper();
                    string titulo = "Cambio de contraseña";
                    string mensajec = string.Format("<section> Estimado {0}<BR/> <p>Su contraseña ha sido cambiado por el usuario {1} </p> </section>", nomComEmp, SessionPersister.Username);
                    m.SendEmail(email, mensajec, titulo, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);
                    mensaje = "Se cambio la contraseña con exito!";
                }
                catch (Exception e)
                {
                    mensaje = "Se produjo un error enviar al enviar el correo ";
                }
            }
            else
            {
                mensaje = "se produjo un erro en el cambio de contraseña";
            }

            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult CambiarContrasenia(ChangePasswordModels model)
        {
            if (ModelState.IsValid)
            {
               if( verificarPass(model.userpassA))
                {

                    string passwEn = "";
                    using (MD5 md5Hash = MD5.Create())
                    {
                        passwEn = _usu.GetMd5Hash(md5Hash, model.userpassNR);
                    }
                    Boolean estado = _usu.updatePassword(SessionPersister.UserId, passwEn);

                    if (estado)
                    {
                        TempData["mensaje"] = "<div id='success' class='alert alert-success' style='text-align: center;font-weight: 600;font-size: x-large;'>La contraseña se cambio con exito.</div>";
                    }
                    else
                    {
                        TempData["mensaje"] = "<div id='danger' class='alert alert-danger' style='text-align: center;font-weight: 600;font-size: x-large;'> Se produjo un error al guardar la contraseña.</div>";
                    }

                    return View();
                }
                else
                {
                    TempData["mensaje"] = "<div id='danger' class='alert alert-danger' style='text-align: center;font-weight: 600;font-size: x-large;'> La contraseña actual no es correcta.</div>";
                    return View(model);
                }
            }

            return View(model);
        }

        //otros
        [SessionAuthorize]
        public ActionResult convertirImagen(string idAcc)
        {
                var imagen = _usu.obtenerItem(idAcc);

                if (imagen.rutaImgPer == null)
                {
                    string locacion = Server.MapPath("~/Areas/Sistemas/FotoPerfil/default.png");
                    FileStream foto = new FileStream(locacion, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    Byte[] arreglo = new Byte[foto.Length];
                    BinaryReader reader = new BinaryReader(foto);
                    arreglo = reader.ReadBytes(Convert.ToInt32(foto.Length));
                    imagen.rutaImgPer = arreglo;
                    reader.Close();
                }
                return File(imagen.rutaImgPer, "image/jpeg");
        }

        //combos
        [HttpPost]
        public JsonResult depaUsu(string cCod_Pais)
        {
            return Json(_ubic.ubicacionPersonal().Where(x => x.cCod_Pais == cCod_Pais).Select(y => new { y.cCod_Dpto, y.cDepartamento }).Distinct().OrderBy(x => x.cDepartamento), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult provUsu(string cCod_Pais, string cCod_Dpto)
        {
            return Json(_ubic.ubicacionPersonal().Where(x => x.cCod_Dpto == cCod_Dpto && x.cCod_Pais== cCod_Pais).Select(y => new { y.cCod_Provincia, y.cProvincia }).Distinct().OrderBy(x => x.cProvincia), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult distUsu(string cCod_Pais, string cCod_Dpto,string cCod_Provincia)
        {
            return Json(_ubic.ubicacionPersonal().Where(x => x.cCod_Provincia == cCod_Provincia && x.cCod_Pais == cCod_Pais && x.cCod_Dpto == cCod_Dpto).Select(y => new { y.cCod_Ubi, y.cDistrito }).Distinct().OrderBy(x=>x.cDistrito), JsonRequestBehavior.AllowGet);
        }

        //otros
        public Boolean verificarPass(string pass)
        {
            Boolean ok = false;
            UsuarioModels u = new UsuarioModels();
            u.userpass = pass;
            u.username = SessionPersister.Username;
            var login = _am.obtenerlogin(u);

            if (login != null)
            {
                ok = true;
            }

            return ok;
        }

        private List<SelectListItem> ListadoSioNo()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "SI", Value = "SI" });
            lst.Add(new SelectListItem() { Text = "NO", Value = "NO" });

            return lst;
        }
        private List<SelectListItem> ListadoTipoComision()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "FLUJO", Value = "FLUJO" });
            lst.Add(new SelectListItem() { Text = "MIXTA", Value = "MIXTA" });
            lst.Add(new SelectListItem() { Text = "NINGUNO", Value = "NINGUNO" });
            return lst;
        }
        private List<SelectListItem> ListadoTipoSeguro()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "ESSALUD", Value = "ESSALUD" });
            lst.Add(new SelectListItem() { Text = "EPS", Value = "EPS" });
            return lst;
        }

        private List<SelectListItem> ListadoEstadoED()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "COMPLETO", Value = "COMPLETO" });
            lst.Add(new SelectListItem() { Text = "INCOMPLETO", Value = "INCOMPLETO" });
            lst.Add(new SelectListItem() { Text = "ESTUDIANDO", Value = "ESTUDIANDO" });
            return lst;
        }


        private List<SelectListItem> ListadoMes()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem() { Text = "ENERO", Value = "ENERO" });
            lst.Add(new SelectListItem() { Text = "FEBRERO", Value = "FEBRERO" });
            lst.Add(new SelectListItem() { Text = "MARZO", Value = "MARZO" });
            lst.Add(new SelectListItem() { Text = "ABRIL", Value = "ABRIL" });
            lst.Add(new SelectListItem() { Text = "MAYO", Value = "MAYO" });
            lst.Add(new SelectListItem() { Text = "JUNIO", Value = "JUNIO" });
            lst.Add(new SelectListItem() { Text = "JULIO", Value = "JULIO" });
            lst.Add(new SelectListItem() { Text = "AGOSTO", Value = "AGOSTO" });
            lst.Add(new SelectListItem() { Text = "SETIEMBRE", Value = "SETIEMBRE" });
            lst.Add(new SelectListItem() { Text = "OCTUBRE", Value = "OCTUBRE" });
            lst.Add(new SelectListItem() { Text = "NOVIEMBRE", Value = "NOVIEMBRE" });
            lst.Add(new SelectListItem() { Text = "DICIEMBRE", Value = "DICIEMBRE" });
            return lst;
        }

        public string[] listaDetalle(string info)
        {
            string[] detalle = info.Split('|');
            return detalle;
        }

        public string cargarTablaEstudio(List<EstudioEmpleadoModels> models)
        {
            string rows = "";
            //armo tabla tb_familia
            if (models.Count != 0)
            {
                foreach (var i in models)
                {
                    rows += "<tr>";
                    rows += "<td class='text-center'>" + i.idEstu + "</td>";
                    rows += "<td class='text-center'>" + i.nomInsEstu + "</td>";
                    rows += "<td class='hidden'>" + i.idNivEstu + "</td>";
                    rows += "<td class='text-center'>" + i.nivelEstudio.nomNivEstu + "</td>";
                    rows += "<td class='text-center'>" + i.carProEstu + "</td>";
                    rows += "<td class='hidden'>" + i.curActEstu + "</td>";
                    rows += "<td class='hidden'>" + i.mesDesEstu + "</td>";
                    rows += "<td class='hidden'>" + i.anioDesEstu + "</td>";
                    rows += "<td class='hidden'>" + i.mesHasEstu + "</td>";
                    rows += "<td class='hidden'>" + i.anioHasEstu + "</td>";
                    rows += "<td class='update'><span class='glyphicon glyphicon-edit'></span></td>";
                    rows += "<td class='delete' onclick='ActualizarIdFila(event)'><span class='glyphicon glyphicon-remove'></span></td>";
                    rows += "</tr>";
                }
            }

           return rows;
        }

        public string cargarTablaFamilia(List<FamiliaEmpleadoModels> models)
        {
            string rows = "";
            //armo tabla tb_familia
            if (models.Count != 0)
            {
                int cant = 0;
                foreach (var i in models)
                {
                    cant++;
                    rows += "<tr>";
                    rows += "<td class='text-center'>" + cant + "</td>";
                    rows += "<td class='text-center'>" + i.dniEmpFam + "</td>";
                    rows += "<td class='text-center'>" + i.nomComEmpFam + "</td>";
                    rows += "<td class='hidden'>" + i.fchNacEmpFam.ToShortDateString() + "</td>";
                    rows += "<td class='hidden'>" + i.idTipFam + "</td>";
                    rows += "<td class='text-center'>" + i.tipoFamilia.nomTipFa + "</td>";
                    rows += "<td class='text-center'>" + i.ocuEmpFam + "</td>";
                    rows += "<td class='updateF'><span class='glyphicon glyphicon-edit'></span></td>";
                    rows += "<td class='delete' onclick='ActualizarIdFila(event)'><span class='glyphicon glyphicon-remove'></span></td>";
                    rows += "</tr>";
                }
            }

            return rows;
        }

    }
}