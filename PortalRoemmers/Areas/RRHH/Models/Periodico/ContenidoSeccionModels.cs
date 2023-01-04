using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.RRHH.Models.Periodico
{
    public class ContenidoSeccionModels
    {
        [Key]
        [Display(Name = "Periódico")]
        [StringLength(10)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string idConSec { get; set; }
        [ForeignKey("idConSec")]
        public  PeriodicoSeccionModels perSec { get; set; }

        [Display(Name = "Título")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string titConSec { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string desConSec { get; set; }

        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Imagen")]
        public string rutConSec { get; set; }

        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCre { get; set; }
        //Fecha de modificacion
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }
        //Usuario creacion
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuCre { get; set; }
        //Fecha de modificacion
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuMod { get; set; }


    }
}