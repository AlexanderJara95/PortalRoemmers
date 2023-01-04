using PortalRoemmers.Areas.RRHH.Services.Formulario;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Filters;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.Formulario
{
    //000374
    public class FormularioController : Controller
    {
        FormularioRepositorio _form;
        Form_Usu_Repositorio _forUsu;
        AtributoHtmlRepositorio _atrHtml;

        public FormularioController()
        {
            _form = new FormularioRepositorio();
            _forUsu = new Form_Usu_Repositorio();
            _atrHtml = new AtributoHtmlRepositorio();
        }

        [CustomAuthorize(Roles = "000003,000375")]
        public ActionResult Index(string menuArea, string menuVista, int pagina = 1, string search = "")
        {
           
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;
            SessionPersister.Search = search;
            SessionPersister.Pagina = pagina.ToString();
            var model = _form.obtenerTodos(pagina, search);
            ViewBag.search = search;
            return View(model);
        }

        [HttpGet]
        [CustomAuthorize(Roles = "000003,000376")]
        public ActionResult Registrar()
        {
            ViewBag.AtrHtml = new SelectList(_atrHtml.obtenerAtributosHtml().Select(x=>new { idAtrHtml= x.idAtrHtml+"-"+x.multAtrHtml, nomAtrHtml= x.nomAtrHtml }), "idAtrHtml", "nomAtrHtml");
            ViewBag.opcion = new SelectList(ListadoOpcion(), "Value", "Text");

            return View();
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000377")]
        public ActionResult Modificar(string id)
        {
            var model = _form.obtenerItem(id);
            return View(model);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000378")]
        public ActionResult Eliminar(string id)
        {
            var model = _form.obtenerItem(id);
            return View(model);
        }

        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000379")]
        public JsonResult activarFicha(string idFor, DateTime fchIniFor, DateTime fchFinFo)
        {
            return Json(_form.activarFicha(idFor, fchIniFor, fchFinFo), JsonRequestBehavior.AllowGet);
        }

        //formularios de usuarios
        [HttpGet]
        [CustomAuthorize(Roles = "ALL")]
        public ActionResult FichaFormulario(string form)
        {
            SedeRepositorio se = new SedeRepositorio();
            EmpleadoModels emple = (EmpleadoModels)System.Web.HttpContext.Current.Session[Sessiones.empleado];

            var s = se.obtenerSede(emple.cod_sede);

            ViewBag.sede = s==null ?"":s.nom_sede ;
            ViewBag.form = form;
            return View(emple);
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult FichaFormulario(FormCollection formCollection)
        {
            string form=formCollection["form"];

            TempData["mensaje"] = "<div id='danger' class='alert alert-danger' style='font-size: 25px;text-align: center;font-weight: 500;'>" + "Se genero error al guardar formulario, comunicarse con el administrador del sistema" + "</div>";


            if (_form.guardarDatosFormulario(formCollection))
            {
               if( _forUsu.sqlUpdateEstado(SessionPersister.UserId, form))
                {
                    System.Web.HttpContext.Current.Session[Sessiones.formulario] = _forUsu.obtenerFormulariosxUsuario(SessionPersister.UserId);
                   
                }
                TempData["mensaje"] = "<div id='success' class='alert alert-success' style='font-size: 25px;text-align: center;font-weight: 500;'>Se registro exitosamente el formulario, gracias por su participación.</div>";
            }
            return RedirectToAction("Index", "Home", new { Area = "", menuArea = SessionPersister.ActiveMenuI, menuVista = SessionPersister.ActiveVistaI, });
        }

        private List<SelectListItem> ListadoOpcion()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "SI", Value = "true" });
            lst.Add(new SelectListItem() { Text = "NO", Value = "false" });

            return lst;
        }


    }
}