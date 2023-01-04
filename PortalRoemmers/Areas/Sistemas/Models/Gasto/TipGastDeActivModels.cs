using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PortalRoemmers.Areas.Sistemas.Models.Gasto
{
    public class TipGastDeActivModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idTipGasAct { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomTipGasAct { get; set; }


        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(25, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Abreviatura")]
        public string abrTipGasAct { get; set; }
        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreTipGastActiv { get; set; }
        //Fecha de modificacion
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModTipGastActiv { get; set; }
        //Usuario creacion
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreTipGastActiv { get; set; }
        //Fecha de modificacion
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModTipGastActiv { get; set; }
    }
}