using PortalRoemmers.Areas.RRHH.Services.Formulario;
using PortalRoemmers.Filters;
using PortalRoemmers.Helpers;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.RRHH.Controllers.Formulario
{
    //000372
    public class ForUsuController : Controller
    {

        private Form_Usu_Repositorio _det;
        private FormularioRepositorio _form;
        public ForUsuController()
        {
            _det = new Form_Usu_Repositorio();
            _form = new FormularioRepositorio();
        }


        [CustomAuthorize(Roles = "000003,000373")]
        public ActionResult Index(string menuArea, string menuVista, string idFor, string Value)
        {
            ViewBag.idFor = new SelectList(_form.obtenerFormularios().Select(x => new { x.idFor, x.nomFor }), "idFor", "nomFor");
            SessionPersister.ActiveVista = menuVista;
            SessionPersister.ActiveMenu = menuArea;

            ViewBag.completo = new SelectList(ListadoVista(), "Value", "Text", Value);

            var model = _det.obtenerFormxUsu(idFor, Value);
            return View(model);
        }

        private List<SelectListItem> ListadoVista()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "SI COMPLETO", Value = "true" });
            lst.Add(new SelectListItem() { Text = "NO COMPLETO", Value = "false" });

            return lst;
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> enviarCorreo(string correos)
        {
            
            EmailHelper em = new EmailHelper();
            string[] cors = correos.Split('|');
            Boolean correcto = true;

            foreach (var c in cors)
            {
                string cor = c.Split(';')[0];
                string nomCon = c.Split(';')[1];
                string titulo = "FICHA SINTOMATOLÓGICA";

                string mensaje = string.Format("<section> Estimado (a) <span style=\"font-weight:bold;\">{0}</span><BR/> <p> No olvidar llenar la ficha Sintomatológica  <BR/> Para completar tu ficha ingresa a  <a href=\"{1}\" title=\"Megalabs-latam\">Megalas-latam.com.pe</a> </p>  </section>", nomCon, Url.Action("Index", "Account", new { Area = "" }, Request.Url.Scheme));
                try
                {
                    correcto = em.SendEmail(cor, mensaje, titulo, ConstCorreo.CORREO, ConstCorreo.CLAVE_CORREO);
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                    correcto = false;
                }
            }

    



            return Json(correcto, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [EncryptedActionParameter]
        [CustomAuthorize(Roles = "000003,000380")]
        public ActionResult FormularioUsuarios(string id, string nom)
        {
            var usuarios = _det.obtenerUsuariosxFormularios(id);

             ViewBag.usuario = new SelectList(_det.obtenerNoUsuariosxFormularios(id).Select(x => new { x.idAcc, nombre = x.empleado.nomComEmp + " (" + x.username + ")" }), "idAcc", "nombre");
             ViewBag.usuarioA = new SelectList(usuarios.Select(x => new { x.idAcc, nombre = x.accounts.empleado.nomComEmp + " (" + x.accounts.username + ")" }), "idAcc", "nombre");
           
            ViewBag.id = id;
            ViewBag.nom = nom;

            return View();
        }

        [HttpPost]
        [CustomAuthorizeJson(Roles = "000003,000381")]
        public JsonResult obtenerReporteLlenado(string idFor, DateTime idDia)
        {
            return Json(_det.listarDatosFormularioLlenado(idFor, idDia), JsonRequestBehavior.AllowGet);
        }

        public FileResult Resultado(string ruta)
        {
            return File(ruta, "application/vnd.ms-excel");
        }


        [HttpPost]
        [SessionAuthorize]
        public ActionResult AddUsers(string[] idAcc, string id, string nombre)
        {
            string codigo = id;

            _det.agregarUsuariosFormulario(idAcc, codigo);

            return RedirectToAction("FormularioUsuarios", new { id = codigo, nom = nombre });
        }
        [HttpPost]
        [SessionAuthorize]
        public ActionResult DelUsers(string[] idAccA, string id, string nombre)
        {
            string codigo = id;
            _det.eliminarUsuariosFormulario(idAccA, codigo);

            return RedirectToAction("FormularioUsuarios", new { id = codigo, nom = nombre });
        }
        [HttpPost]
        public JsonResult cboFechaFicha(string idForR)
        {
            return Json(_det.listarFechaFormulario(idForR), JsonRequestBehavior.AllowGet);
        }

    }
}