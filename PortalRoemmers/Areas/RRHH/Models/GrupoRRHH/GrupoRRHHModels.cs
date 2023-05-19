using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
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

        ///estado 3
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idEstado { get; set; }
        [ForeignKey("idEstado")]
        public EstadoModels estado { get; set; }

        public List<AreaRoeModels> areas { get; set; }
        public List<UsuarioModels> excluidos { get; set; }

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