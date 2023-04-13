using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH
{
    public class UserSolicitudRRHHModels
    {
        // Id 1
        [Display(Name = "Código Solicitud")]
        [StringLength(7)]
        public string idSolicitudRrhh { get; set; }
        [ForeignKey("idSolicitudRrhh")]
        public SolicitudRRHHModels solicitudRRHH;

        //Responsable 2
        [Display(Name = "Responsable")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idAccRes { get; set; }
        [ForeignKey("idAccRes")]
        public UsuarioModels responsable { get; set; }

        //Auditóría
        [Display(Name = "Usuario creación")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
        [Display(Name = "Usuario modificación")]
        public string usuMod { get; set; }
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }
    }
}