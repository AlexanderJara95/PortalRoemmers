using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Marketing.Models.Actividad;
using PortalRoemmers.Areas.Marketing.Models.SolicitudGastoMkt;

namespace PortalRoemmers.Areas.Marketing.Models.Estimacion
{
    public class DetEstim_GastActModels
    {
        //Actividad Relacionada
        [Display(Name = "Actividad")]
        [StringLength(11)]
        public string idActiv { get; set; }
        [ForeignKey("idActiv")]
        public  EstimacionModels cabecera { get; set; }

        //Actividad Gasto Relacionada
        [Display(Name = "Tipo de Gasto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idActGas { get; set; }
        [ForeignKey("idActGas")]
        public  ActividadGastoModels gastoActiv { get; set; }

        //Importe Total
        [Display(Name = "Importe Total")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Double monTotal { get; set; }

        //Importe Promedio
        [Display(Name = "Importe Promedio")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Double monProm { get; set; }

        //Saldo Real
        [Display(Name = "Saldo Real")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Double salReal { get; set; }

        //---------------------------------------------------------------------
        public virtual List<DetSolGasto_GasModels> detSolGas { get; set; }
        //---------------------------------------------------------------------

        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreEstGast { get; set; }
        //Fecha de modificacion
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModEstGast { get; set; }
        //Usuario creacion
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreEstGast { get; set; }
        //Fecha de modificacion
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModEstGast { get; set; }
    }
}