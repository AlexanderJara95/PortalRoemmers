using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Global;

namespace PortalRoemmers.Areas.Contabilidad.Models.Letra
{
    public class AceptanteModels
    {
        //id del cliente
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idAcep { get; set; }

        //Razon Social del Cliente
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Razón Social")]
        [Index(name: "IX_nomAcep")]
        public string nomAceptante { get; set; }

        //NIIF (RUC) del Cliente
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(30, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "RUC")]
        public string niffAceptante { get; set; }

        //Domicilio del Cliente
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Domicilio")]
        public string domAceptante { get; set; }


        //Localidad del Cliente
        //Domicilio del Cliente
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(25, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Localidad")]
        public string locAceptante { get; set; }

        //Correo electronico del Cliente
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Correo")]
        public string correoAceptante { get; set; }

        //Estado del Cliente
        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public EstadoModels estado { get; set; }

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