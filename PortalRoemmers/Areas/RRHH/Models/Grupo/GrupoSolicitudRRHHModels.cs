using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH;

namespace PortalRoemmers.Areas.RRHH.Models.Grupo
{
    public class GrupoSolicitudRRHHModels
    {
        //Id 1
        [Display(Name = "Código Solicitud")]
        [StringLength(7)]
        public string idSolicitudRrhh { get; set; }
        [ForeignKey("idSolicitudRrhh")]
        public SolicitudRRHHModels solicitudRRHH;

        //Grupo 2
        [Display(Name = "Grupo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(7)]
        public string idGrupoRrhh { get; set; }
        [ForeignKey("idGrupoRrhh")]
        public GrupoRRHHModels grupoRRHH { get; set; }

        //Descripción 3
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción Grupo")]
        public string descGrupo { get; set; }

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