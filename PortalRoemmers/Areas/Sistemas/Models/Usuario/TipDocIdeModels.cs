using PortalRoemmers.Areas.Sistemas.Models.Medico;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class TipDocIdeModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idTipDoc { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        public string nomTipDoc { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string desTipDoc { get; set; }

        [Display(Name = "Longitud")]
        public int longTipDoc { get; set; }

        public List<EmpleadoModels> empleado { get; set; }
        public List<MedicoModels> clientes { get; set; }

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