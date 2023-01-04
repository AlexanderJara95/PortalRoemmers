using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Trilogia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Visitador
{
    public class LineaModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idLin { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        public string nomLin { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string desLin { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Tipo")]
        public string tipLin { get; set; }

        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

        //auditoria
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreLin { get; set; }

        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModLin { get; set; }

        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreLin { get; set; }

        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModLin { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Linea Seleccionado")]
        public string linAc { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Linea No Seleccionado")]
        public string linIn { get; set; }

        public List<Pro_LIn_Models> proLin { get; set; }
        public List<Usu_Zon_Lin_Models> UseZonLin { get; set; }

    }
}