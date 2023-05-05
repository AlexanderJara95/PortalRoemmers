using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;

namespace PortalRoemmers.Areas.RRHH.Models.Grupo
{
    public class AreaGrupoRRHHModels
    {
        // Id 1
        [Display(Name = "Área")]
        [StringLength(7)]
        public string idAreRoe { get; set; }
        [ForeignKey("idAreRoe")]
        public AreaRoeModels area;

        // Grupo 2
        [Display(Name = "Grupo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(7)]
        public string idGrupoRrhh { get; set; }
        [ForeignKey("idGrupoRrhh")]
        public GrupoRRHHModels grupoRRHH { get; set; }

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
