using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Models.Formulario
{ 
    public class AtributoHTMLModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idAtrHtml { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(25, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomAtrHtml { get; set; }

        [Display(Name = "Icono")]
        [StringLength(25, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string iconAtrHtml { get; set; }
        
        [Display(Name = "Multiple")]
        public Boolean multAtrHtml { get; set; }

        [Display(Name = "Etiqueta")]
        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string tagAtrHtml { get; set; }

    }
}