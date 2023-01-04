using System;
using System.ComponentModel.DataAnnotations;

namespace PortalRoemmers.Areas.Sistemas.Models.Global
{
    public class ByteModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(5)]
        public string idByt { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomByt { get; set; }

        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string desByt { get; set; }

        //Auditoria
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuCre { get; set; }
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCre { get; set; }
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuMod { get; set; }
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }
    }
}