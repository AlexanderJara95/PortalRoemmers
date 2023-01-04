using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalRoemmers.Areas.RRHH.Models.Galeria
{
    public class TipoGaleriaModels
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Código")]
        public string idTipGal { get; set; }

        [Display(Name = "Título")]
        [StringLength(25, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string titTipGal { get; set; }

        [Display(Name = "Clase compuesta")]
        [StringLength(30, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string classesTipGal { get; set; }

        [Display(Name = "Filtro")]
        [StringLength(35, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string filTipGal { get; set; }

        [Display(Name = "Clase única")]
        [StringLength(30, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string classTipGal { get; set; }

        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string desTipGal { get; set; }

        public  List<GaleriaModels> galerias { get; set; }

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