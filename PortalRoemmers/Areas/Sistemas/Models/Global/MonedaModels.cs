using System;
using System.ComponentModel.DataAnnotations;

namespace PortalRoemmers.Areas.Sistemas.Models.Global
{
    public class MonedaModels
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Código")]
        public string idMon { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomMon { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(25, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Abreviatura")]
        public string abrMon { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Simbolo")]
        public string simbMon { get; set; }

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