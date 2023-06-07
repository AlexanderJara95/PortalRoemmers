using PortalRoemmers.Areas.RRHH.Models.DocSustentoRRHH;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH
{
    public class DocSustentoSolicitudRRHHModel
    {
        // Id 1
        [Display(Name = "Código Solicitud")]
        [StringLength(7)]
        public string idSolicitudRrhh { get; set; }
        [ForeignKey("idSolicitudRrhh")]
        public SolicitudRRHHModels solicitudRRHH;

        //Responsable 2
        [Display(Name = "Documento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idDocSustento { get; set; }
        [ForeignKey("idDocSustento")]
        public DocSustentoRRHHModels docSustentoRRHH { get; set; }

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