using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PortalRoemmers.Areas.Sistemas.Services.Global;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using PortalRoemmers.Filters;

namespace PortalRoemmers.Areas.Sistemas.Controllers.Global
{
    public class ParametroController : Controller
    {//PARAMETROCONTROLLER  000207
        Ennumerador enu = new Ennumerador();
        private ParametroRepositorio _par;
        // GET: Sistemas/Parametro
        public ParametroController()
        {
            _par = new ParametroRepositorio();
        }
        [CustomAuthorize(Roles = "000003,000208")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _par.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }
        //registrar
        [CustomAuthorize(Roles = "000003,000209")]
        [HttpGet]
        public ActionResult Registrar()
        {
            string parametroD = "";
            ParametroModels model = new ParametroModels();
            cargarCombos(parametroD);
            return View();
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Registrar(ParametroModels model, string ParametroD)
        {
            if (ModelState.IsValid)
            {
                model.userCrePar = SessionPersister.Username;
                model.fchCrePar = DateTime.Now;
                var resultado = _par.crear(model);
                if (resultado != "" && resultado != "error")
                {
                    var f = listaDetalle(ParametroD);
                    //familia
                    List<ParDetalleModels> listPar = new List<ParDetalleModels>();
                    ParDetalleModels detpar = new ParDetalleModels();

                    foreach (var i in f)
                    {
                        string[] item = i.Split(';');
                        if (item.Length>3)
                        { 
                            detpar.idPar = resultado;
                            detpar.idDetPar = item[0];
                            detpar.nomDetPar = item[2];
                            detpar.valDetPar = item[3];
                            listPar.Add(detpar);
                            detpar = new ParDetalleModels();
                        }
                    }
                    TempData["mensaje"] = _par.crearDetallePar(listPar, resultado);
                }
                else
                {
                    TempData["mensaje"] = resultado;
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            cargarCombos(ParametroD);
            return View(model);
        }
        //modificar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000210")]
        public ActionResult Modificar(string id)
        {
            var model = _par.obtenerItem(id);
            var lista = model.detPar.ToList();
            string Parametro = ObtenerParametroD(lista);
            cargarCombos(Parametro);
            return View(model);
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult Modificar(ParametroModels model, string ParametroD)
        {
            if (ModelState.IsValid)
            {
                model.fchModPar = DateTime.Now;
                model.userModPar = SessionPersister.Username;
                var resultado = _par.modificar(model);
                if (resultado != "" && resultado != "error")
                {
                    var f = listaDetalle(ParametroD);
                    //Modificar detalle
                    List<ParDetalleModels> listPar = new List<ParDetalleModels>();
                    ParDetalleModels detpar = new ParDetalleModels();
                    foreach (var i in f)
                    {
                        string[] item = i.Split(';');
                        if (item.Length > 3)
                        {
                            detpar.idPar = resultado;
                            detpar.idDetPar = item[0];
                            detpar.nomDetPar = item[2];
                            detpar.valDetPar = item[3];
                            listPar.Add(detpar);
                            detpar = new ParDetalleModels();
                        }
                    }
                    TempData["mensaje"] = _par.modificaDetallePar(listPar, resultado);
                    //Modificar detalle
                }
                else
                {
                    TempData["mensaje"] = resultado;
                }
                return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
            }
            cargarCombos(ParametroD);
            return View(model);
        }
        //Eliminar
        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000211")]
        public ActionResult Eliminar(string id)
        {
            var model = _par.obtenerItem(id);

            return View(model);
        }
        [HttpPost, ActionName("Eliminar")]
        [SessionAuthorize]
        public ActionResult EliminarID(ParametroModels model)
        {
            TempData["mensaje"] = _par.eliminar(model.idPar);

            return RedirectToAction("Index", new { menuArea = SessionPersister.ActiveMenu, menuVista = SessionPersister.ActiveVista, pagina = SessionPersister.Pagina, search = SessionPersister.Search });
        }

        //otros
        public string ObtenerParametroD(List<ParDetalleModels> model)
        {
            int k = model.Count();
            string parametroD = "";
            foreach (var i in model)
            {
                parametroD += i.idDetPar + ";" + i.nomDetPar + ";" + i.valDetPar;
                k--;
                if (k != 0)
                {
                    parametroD += "|";
                }
            }
            return parametroD;
        }
        public string[] listaDetalle(string info)
        {
            string[] detalle = info.Split('|');
            return detalle;
        }
        public void cargarCombos(string ParametroD)
        {
            string rowsP = "";
            //armo tabla tb_familia
            if (ParametroD != "")
            {
                var f = listaDetalle(ParametroD);
                int k = 0;
                foreach (var i in f)
                {
                    k = k + 1;
                    string[] item = i.Split(';');
                    rowsP += "<tr>";
                    rowsP += "<td class='hidden'>" + item[0] + "</td>";
                    rowsP += "<td class='text-center'>" + k + "</td>";
                    rowsP += "<td class='text-center'>" + item[1] + "</td>";
                    rowsP += "<td class='text-center'>" + item[2] + "</td>";
                    rowsP += "<td class='delete' onclick='ActualizarIdFila(event)'><span class='glyphicon glyphicon-remove'></span></td>";
                    rowsP += "</tr>";
                }
            }
            //envio los combos ya cargados
            try
            {
                ViewBag.tb_ParDetalle = rowsP;
            }
            catch { }

        }

    }
}
