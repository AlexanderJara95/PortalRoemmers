using System;
using System.ComponentModel.DataAnnotations;

namespace PortalRoemmers.Areas.RRHH.Models.Grupo
{
    public class GrupoRRHHModels
    {
        //Id 1
        [Key]
        [Display(Name = "Código Grupo")]
        [StringLength(7)]
        public string idGrupoRrhh { get; set; }

        //Descripción 2
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción Grupo")]
        public string descGrupo { get; set; }

        //Auditoría 3
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