using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.RRHH.Models.Periodico
{
    public class PeriodicoSeccionModels
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Código")]
        public string idPerSec { get; set; }

        [Display(Name = "Título")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string titPerSec { get; set; }

        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Imagen")]
        public string rutPerSec { get; set; }

        [Display(Name = "Target")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string tarPerSec { get; set; }

        [Display(Name = "Caída")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string caiPerSec { get; set; }

        [Display(Name = "Efecto Imagen")]
        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string idEfeIma { get; set; }

        [ForeignKey("idEfeIma")]
        [Display(Name = "Efecto Imagen")]
        public  EfectoImagenModels efecIma { get; set; }

        //Relaciones
        public  ContenidoSeccionModels contSec { get; set; }

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