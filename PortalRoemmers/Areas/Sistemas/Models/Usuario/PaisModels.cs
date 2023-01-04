using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class PaisModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idPais { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomPais { get; set; }

        [Display(Name = "Capital")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string capPais { get; set; }

        [Display(Name = "Continente")]
        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string conPais { get; set; }

        [Display(Name = "ISO 3")]
        [StringLength(3, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string iso3Pais { get; set; }

        [Display(Name = "ISO 2")]
        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string iso2Pais { get; set; }


        //Auditoria
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