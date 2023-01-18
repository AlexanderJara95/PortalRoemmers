using PortalRoemmers.Areas.Marketing.Services.Estimacion;
using PortalRoemmers.Areas.Sistemas.Services.Gasto;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Services.Presupuesto;
using PortalRoemmers.Areas.Sistemas.Services.Producto;
using PortalRoemmers.Areas.Sistemas.Services.Trilogia;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Visitador;
using PortalRoemmers.Areas.Ventas.Services.SolicitudGasto;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

namespace PortalRoemmers.Areas.RRHH.Controllers.SolicitudRRHH
{
    public class SolicitudRRHHController : Controller
    {
        private SolicitudGastoRepositorio _sis;
        private AsigAproRepositorio _asiapr;
        private UsuarioRepositorio _usu;
        private TipoGastoRepositorio _tgas;
        private ConceptoGastoRepositorio _cgas;
        private Usu_Zon_LinRepositorio _uzl;
        private Pro_Lin_Repositorio _pl;
        private LineaRepositorio _lin;
        private ZonaRepositorio _zon;
        private MonedaRepositorio _mon;
        private EspecialidadRepositorio _esp;
        private TipoCambioRepositorio _tcam;
        private NivelAproRepositorio _niv;
        private EstadoRepositorio _est;
        private PresupuestoRepositorio _pre;
        private MovimientoPresRepositorio _mov;
        private LiquidaRepositorio _liq;
        private EstimacionRepositorio _esti;
        private ProductoRepositorio _product;
        private TipoGasto_UsuRepositorio _tipU;
        private TipoGastoRepositorio _tipGas;
        Ennumerador enu = new Ennumerador();

        public SolicitudRRHHController()
        {
            _sis = new SolicitudGastoRepositorio();
            _asiapr = new AsigAproRepositorio();
            _usu = new UsuarioRepositorio();
            _tgas = new TipoGastoRepositorio();
            _cgas = new ConceptoGastoRepositorio();
            _uzl = new Usu_Zon_LinRepositorio();
            _pl = new Pro_Lin_Repositorio();
            _lin = new LineaRepositorio();
            _zon = new ZonaRepositorio();
            _mon = new MonedaRepositorio();
            _esp = new EspecialidadRepositorio();
            _tcam = new TipoCambioRepositorio();
            _niv = new NivelAproRepositorio();
            _est = new EstadoRepositorio();
            _pre = new PresupuestoRepositorio();
            _mov = new MovimientoPresRepositorio();
            _liq = new LiquidaRepositorio();
            _esti = new EstimacionRepositorio();
            _product = new ProductoRepositorio();
            _tipGas = new TipoGastoRepositorio();
            _tipU = new TipoGasto_UsuRepositorio();
        }
        [CustomAuthorize(Roles = "000003,000192")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "", string fchIniSolicitud = "", string fchFinSolicitud = "")
        {
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();
            try
            {
                inicio = DateTime.Parse(fchIniSolicitud);
                fin = DateTime.Parse(fchFinSolicitud);
            }
            catch (Exception e)
            {
                DateTime date = DateTime.Now;
                DateTime oPrimerDiaDelMes = new DateTime(date.Year, 1, 1);
                var primero = oPrimerDiaDelMes.ToString("dd/MM/yyyy");
                var actual = DateTime.Today.ToString("dd/MM/yyyy");
                inicio = DateTime.Parse(primero);
                fin = DateTime.Parse(actual);
            }
            //-----------------------------
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            SessionPersister.FchEveSolGasI = fchIniSolicitud;
            SessionPersister.FchEveSolGasF = fchFinSolicitud;
            //-----------------------------
            ViewBag.search = search;
            ViewBag.primero = inicio.ToString("dd/MM/yyyy");
            ViewBag.actual = fin.ToString("dd/MM/yyyy");
            //-----------------------------
            var model = _sis.obtenerTodos(pagina, search, ConstantesGlobales.mod_ventas, inicio.ToString(), fin.ToString());
            //-----------------------------
            return View(model);
        }
    }
}


