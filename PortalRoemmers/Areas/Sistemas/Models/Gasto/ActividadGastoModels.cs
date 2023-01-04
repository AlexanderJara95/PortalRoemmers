using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Visitador;
namespace PortalRoemmers.Areas.Sistemas.Models.Gasto
{
    public class ActividadGastoModels
    {
        //Codigo del gasto de la actividad
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idActGas { get; set; }
        //Nombre del gasto de la actividad
        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomActGas { get; set; }
        //Descripcion de la actividad
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string descActGas { get; set; }
        //Estado del gasto de la actividad
        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }
        //Tipo de gasto actividad
        [Display(Name = "Tipo Gasto Actividad")]
        [StringLength(10)]
        public string idTipGasAct { get; set; }
        [ForeignKey("idTipGasAct")]
        public  TipGastDeActivModels tipoGastoActividad { get; set; }
        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreGastActiv { get; set; }
        //Fecha de modificacion
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModGastActiv { get; set; }
        //Usuario creacion
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreGastActiv { get; set; }
        //Fecha de modificacion
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModGastActiv { get; set; }
    }
}