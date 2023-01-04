using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Global;

namespace PortalRoemmers.Areas.Contabilidad.Models.Letra
{
    public class LetraModels
    {
        //id de la letra
        [Key]
        [Display(Name = "Id")]
        [StringLength(10)]
        public string idLetra { get; set; }

        //Codigo de la letra
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Código")]
        [Index(name: "IX_codLet")]
        public string codLetra { get; set; }

        //Referencia del Girador
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Referencia")]
        public string refLetra { get; set; }

        //Fecha de Giro
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [Display(Name = "Fecha Giro")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchGiroLet { get; set; }

        //Lugar de Giro
        [StringLength(25, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Lugar Giro")]
        public string lugGiroLet { get; set; }

        //Fecha de Vencimiento
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [Display(Name = "Fecha Venc.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchVencLet { get; set; }

        //Codigo del Aceptante
        [Display(Name = "Aceptante")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idAcep { get; set; }
        [ForeignKey("idAcep")]
        public AceptanteModels aceptante { get; set; }

        //Moneda
        [Display(Name = "Moneda")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idMon { get; set; }
        [ForeignKey("idMon")]
        public MonedaModels moneda { get; set; }

        //Importe
        [Display(Name = "Importe")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Double impLetra { get; set; }

        //Letra de Importe
        [Display(Name = "Cantidad en Letras")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string cantEnLetras { get; set; }

        //Estado
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
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

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Opcion")]
        public Boolean option { get; set; }

    }
}