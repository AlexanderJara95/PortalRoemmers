using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using PortalRoemmers.Services;
using System;
using System.Web.Mvc;

namespace PortalRoemmers.Controllers
{
    public class AccountController : Controller
    {
        AccountRepositorio am = new AccountRepositorio();

        // GET: Account
        [AllowAnonymous]
        [NoLogin]
        public ActionResult Index()
        {
            ViewBag.Titulo = "Página Logueo - Mega Labs";
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UsuarioModels avm)
        {
            //si esta vacio
            if (string.IsNullOrEmpty(avm.username) || string.IsNullOrEmpty(avm.userpass))
            {
                ViewBag.Error = "Los campos no pueden esta vacios";
            }
            else
            {
                var usu = am.obtenerlogin(avm);

                if (usu==null)
                {
                    ViewBag.Error = "LA CUENTA ES INVALIDA";
                }
                else
                {
                    if (usu.confirmEmail==true)
                    {
                        if (usu.idEst== ConstantesGlobales.estadoActivo)
                        {
                            am.sessionUser(usu);

                            return RedirectToAction("Index", "Home", new { menuArea = SessionPersister.ActiveMenuI, menuVista = SessionPersister.ActiveVistaI, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
                        }
                        ViewBag.Error = "LA CUENTA ESTA DESACTIVADA";
                    }
                    else
                    {
                        try {
                        //generar token
                        string token = am.generarToken(usu.idAcc);

                        //envio mensaje al usuario
                        EmailHelper m = new EmailHelper();
                        string mensaje = string.Format("<section> Estimado (a) {0}<BR/> <p>Gracias por su registro, por favor haga clic en el siguiente enlace para completar su registro:</p><BR/>  <a href=\"{1}\" title=\"Confirmar correo electrónico del usuario\">{1}</a> </section>", usu.empleado.nomComEmp, Url.Action("ConfirmEmail", "Account", new { Token = token, Area = "" }, Request.Url.Scheme));
                        string titulo = "Confirmación de correo electrónico";
                         m.SendEmail(usu.email, mensaje, titulo,ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);
                        return RedirectToAction("Confirm", "Account", new { Email = usu.email });
                        }
                        catch (Exception e)
                        {
                            e.Message.ToString();
                            ViewBag.Error = "Se produjo un error al enviar el correo";
                            return View("Index");
                        }
                    }
                }
            }
            return View("Index");
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            SessionPersister.UserId = string.Empty;
            SessionPersister.Username = string.Empty;
            //SessionPersister.Usernom1 = string.Empty;
            //SessionPersister.Usernom2 = string.Empty;
            //SessionPersister.Userapepat = string.Empty;
            //SessionPersister.Userapemat = string.Empty;
            SessionPersister.UserIma = string.Empty;
            SessionPersister.UserRol = string.Empty;
            //SessionPersister.UserCarg = string.Empty;
            //SessionPersister.EmpId = string.Empty;
            SessionPersister.NivApr = string.Empty;
            // SessionPersister.DniEmp = string.Empty;
            SessionPersister.Pagina = string.Empty;
            SessionPersister.Search = string.Empty;

            SessionPersister.ActiveMenu = string.Empty;
            SessionPersister.ActiveVista = string.Empty;
            SessionPersister.UserMenu = string.Empty;
            SessionPersister.UserImaMod = string.Empty;
            
            SessionPersister.ActiveMenuI = string.Empty;
            SessionPersister.ActiveVistaI = string.Empty;

            System.Web.HttpContext.Current.Session.Remove(Sessiones.menu);
            System.Web.HttpContext.Current.Session.Remove(Sessiones.cumple);
            System.Web.HttpContext.Current.Session.Remove(Sessiones.enlace);
            System.Web.HttpContext.Current.Session.Remove(Sessiones.empleado);
            System.Web.HttpContext.Current.Session.Remove(Sessiones.formulario);
            return RedirectToAction("Index","Home",new {Area="" });
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(string Token)
        {
            //obtengo valor
            var valores = am.valorRealToken(Token);
            //obtengo los dos valores
            string tiempo = valores[0];
            string codigo = valores[1];

            //obtengo info del usuario 
            var us = am.obtenerUsu(codigo);

            //valido si es el mismo token que se envio
            if (us.tokenUsu == Token)
            {
                //Valido que aun este vigente
                DateTime caduca = Convert.ToDateTime(tiempo);
                var actualT = DateTime.Now; //UtcNow
                if (actualT <= caduca)
                {
                    string id = codigo;
                    var user = am.obtenerUsu(id);

                    if (user != null)
                    {

                      bool rpt=am.updateConfirEmail(user.idAcc);

                        if (rpt)
                        {
                            am.sessionUser(user);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("ErrorExec", "AccessDenied", new { Area = "" });
                        }
                      
                    }
                    else
                    {
                        return RedirectToAction("Confirm", "Account", new { Email = "" });
                    }

                }
                else
                {
                    return RedirectToAction("TokenCaduco", "AccessDenied", new { Area = "" });
                }
            }
            else
            {
                return RedirectToAction("TokenCaduco", "AccessDenied", new { Area = "" });
            }
        }

        [AllowAnonymous]
        public ActionResult Confirm(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        public JsonResult ForgotPassword(string email)
        {
            string respuesta = "";
            var user = am.obUsuEmail(email);
            if (user==null)
            {
                respuesta = "El correo electrónico ingresado no es válido  para restablecer la contraseña.";
            }
            else
            {
                //generar token
                string token=am.generarToken(user.idAcc);
                //enviar correo al usuario
                EmailHelper m = new EmailHelper();
                string mensaje = string.Format("<section> Estimado (a) {0}<BR/> <p>Para restablecer la contraseña , haga clic </p><BR/>  <a href=\"{1}\" title=\"Aquí\">{1}</a> </section>", user.empleado.nomComEmp , Url.Action("ResetPassword", "Account", new { Token = token, Area = "" }, Request.Url.Scheme));
                string titulo = "Restablecimiento de contraseña";
                m.SendEmail(user.email, mensaje, titulo, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

                respuesta = "Compruebe su correo electrónico para restablecer la contraseña.";
            }
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string Token)
        {
            //obtengo valor
            var valores = am.valorRealToken(Token);
            //obtengo los dos valores
            string tiempo = valores[0];
            string codigo = valores[1];

            //obtengo info del usuario 
            var user = am.obtenerUsu(codigo);

            //valido si es el mismo token que se envio
            if (user.tokenUsu == Token)
            {
                //Valido que aun este vigente
                DateTime caduca = Convert.ToDateTime(tiempo);
                var actualT = DateTime.Now; //UtcNow

                if (actualT <= caduca)
                {
                    UsuarioModels u = new UsuarioModels();
                    u.idAcc = codigo;
                    u.email = user.email;
                    u.nomComEmp = user.empleado.nomComEmp;

                    return View(u);
                }
                else
                {
                    return RedirectToAction("TokenCaduco", "AccessDenied", new { Area = "" });
                }
            }
            else
            {
                return RedirectToAction("TokenCaduco", "AccessDenied", new { Area = "" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(UsuarioModels user)
        {

            bool rpt = am.resetearPassword(user.userpass, user.idAcc);

            if (rpt) {

                //envio mensaje al usuario
                EmailHelper m = new EmailHelper();
                string mensaje = string.Format("<section> Estimado (a) {0}<BR/> <p>La contraseña de su cuenta  en el Portal Roemmers ha cambiado exitosamente.</p><BR/>   </section>", user.nomComEmp);
                string titulo = "Cambio contraseña-Portal Roemmers";
                m.SendEmail(user.email, mensaje, titulo, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);

                return RedirectToAction("Index", "Home", new { Area = "" });
            }
            else
            {
                return RedirectToAction("ErrorExec", "AccessDenied", new { Area = "" });
            }


        
        }

    }
}