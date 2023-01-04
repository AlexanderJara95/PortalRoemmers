using PortalRoemmers.Areas.Sistemas.Models.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Models.Formulario
{
    public class FormularioModels
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Código")]
        public string idFor { get; set; }

        //Nombre
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomFor { get; set; }

        //Titulo
        [StringLength(60, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Título")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string titFor { get; set; }

        //Titulo
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string desFor { get; set; }


        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public EstadoModels estado { get; set; }


        [Display(Name = "Fecha Inicio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy hh:mm tt}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime fchIniFor { get; set; }

        [Display(Name = "Fecha Fin")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy hh:mm tt}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime fchFinFor { get; set; }


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