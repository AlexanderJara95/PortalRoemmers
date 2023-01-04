using PortalRoemmers.Security;
using System;
using System.Text;
using System.Web.Mvc;
using System.Linq;
using PortalRoemmers.Areas.RRHH.Services;
using PortalRoemmers.Areas.RRHH.Services.Boleta;
using PortalRoemmers.Areas.RRHH.Models.Boleta;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Enlace;

namespace PortalRoemmers.Controllers
{
    
    public class HomeController : Controller
    {
        GaleriaRepositorio _gal;
        PeriodicoMuralRepositorio _per;
        TipoGaleriaRepositorio _tipGal;
        BienvenidaRepositorio _bie;
        BoletaDetalleRepositorio _bolDet;
        

        public HomeController()
        {
            _bie = new BienvenidaRepositorio();
            _tipGal = new TipoGaleriaRepositorio();
            _gal = new GaleriaRepositorio();
            _per = new PeriodicoMuralRepositorio();
            _bolDet = new BoletaDetalleRepositorio();
            
        }

        [CustomAuthorize(Roles = "000003,000006")]
        public ActionResult Index(string menuArea, string menuVista)
        {

            if (SessionPersister.Username != null)
            {
                SessionPersister.ActiveVista = menuVista;
                SessionPersister.ActiveMenu = menuArea;
                var model=_bie.obtenerBienvenida();
                ViewBag.titulo = "Página Inicio - Mega Labs";
                return View(model);
             }
            else {
                return RedirectToAction("Index", "Account", new { Area = "" });
            }
        }

        [CustomAuthorize(Roles = "000003,000252")]
        public ActionResult Galeria(string menuArea, string menuVista)
        {
            var galeria = _gal.obtenerGaleria();
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(galeria);
        }

        [CustomAuthorize(Roles = "000003,000259")]
        public ActionResult PeriodicoMural(string menuArea, string menuVista)
        {
            var periodico = _per.obtenerPeriodicoSeccion();
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(periodico);
        }

        [CustomAuthorize(Roles = "000003,000332")]
        public ActionResult BoletaEmpleado(string menuArea, string menuVista)
        {
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];
            var detalle = _bolDet.obtenerBoletasPersonal(emple.nroDocEmp);
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            return View(detalle);
        }

        public ActionResult convertirImagenPerfil()
        {
            byte[] final = new byte[] { 1, 2, 3, 4, 5, 6 };
            try
            {
                final = Encoding.Default.GetBytes(SessionPersister.UserIma);
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return File(final, "image/jpeg");
        }

        [HttpPost]
        public JsonResult verificarVisualizacion(BoletaDetalleModels model)
        {
            bool ver = false;
            if (!_bolDet.obtenerItem(model.idBolPer,model.nroDocBolDet).visBolDet)
            {
                model.visBolDet = true;
                model.usufchVis = DateTime.UtcNow;
                model.usuVis = SessionPersister.Username;
                model.Boleta = null;
                ver=_bolDet.modificar(model);
            }

            return Json(ver, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult obtenerTipGal()
        {
            var tipgal = _tipGal.obtenerTipoGalerias().Select(x=>new {x.idTipGal,x.titTipGal,x.classTipGal,x.filTipGal,x.classesTipGal });
            return Json(tipgal, JsonRequestBehavior.AllowGet);
        }
    
        
    
    }
}