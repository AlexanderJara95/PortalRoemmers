using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace PortalRoemmers.Areas.RRHH.Models.Bienvenida
{
    public class BienvenidaModels
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Código")]
        public string idbien { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nombien { get; set; }

        [Display(Name = "Título")]
        [StringLength(1000, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string titbien { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(3000, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string desbien { get; set; }

        [Display(Name = "Activo")]
        public Boolean actbien { get; set; }

        [Display(Name = "Fotos")]
        public  List<FotosBienvenidaModels> fotos { get; set; }

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