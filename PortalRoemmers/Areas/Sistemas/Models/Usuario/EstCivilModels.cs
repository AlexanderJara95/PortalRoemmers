using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class EstCivilModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idEstCiv { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomEstCiv { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string desEstCiv { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [Display(Name = "Estado")]
        public int estEstCiv { get; set; }

        public List<EmpleadoModels> empleado { get; set; }
        //Auditoria
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuMod { get; set; }
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }

    }
}