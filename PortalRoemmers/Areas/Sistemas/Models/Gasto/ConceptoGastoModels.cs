using PortalRoemmers.Areas.Sistemas.Models.Global;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Gasto
{
    public class ConceptoGastoModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idConGas { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomConGas { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string descConGas { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Concepto contable")]
        public string concContable { get; set; }

        [StringLength(10)]
        [Display(Name = "Cuenta contable")]
        public string ctaContable { get; set; }

        //----------------------------Auditoria--------------------------------
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreConGas { get; set; }

        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModConGas { get; set; }

        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreConGas { get; set; }

        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModConGas { get; set; }

        //Estado
        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

        [Display(Name = "Tipo Gasto")]
        [StringLength(10)]
        public string idTipGas { get; set; }
        [ForeignKey("idTipGas")]
        public  TipoGastoModels tipoGasto { get; set; }


    }
}