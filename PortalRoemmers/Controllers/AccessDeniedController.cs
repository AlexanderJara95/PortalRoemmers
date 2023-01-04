using System.Web.Mvc;

namespace PortalRoemmers.Controllers
{
    public class AccessDeniedController : Controller
    {
        // GET: AccessDenied
        public ActionResult Index()
        {
            ViewBag.tituloD = "Denegado - Mega Labs Latam";
            return View();
        }

        public ActionResult TokenCaduco()
        {
            ViewBag.tituloD = "Caduco Token - Mega Labs Latam";
            return View();
        }

        public ActionResult ErrorExec()
        {
            ViewBag.tituloD = "Error de Ejecucion - Mega Labs Latam";
            return View();
        }

        public JsonResult SinAcceso()
        {
            return Json("ACCESO DENEGO, CONTACTESE CON UN ADMINISTRADOR DEL SISTEMA", JsonRequestBehavior.AllowGet);
        }

        public JsonResult SesionTerminada()
        {
            return Json("VUELVA A LOGUEARSE AL SISTEMA, SU SESION HA EXPIRADO", JsonRequestBehavior.AllowGet);
        }
    }
}