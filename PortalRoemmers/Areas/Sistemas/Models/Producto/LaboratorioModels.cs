using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace PortalRoemmers.Areas.Sistemas.Models.Producto
{
    public class LaboratorioModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idLab { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        public string nomLab { get; set; }

        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string desLab { get; set; }

        public List<ProductoModels> productos { get; set; }

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