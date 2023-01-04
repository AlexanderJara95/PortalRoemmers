using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PortalRoemmers.Areas.Marketing.Models.Actividad;
using PortalRoemmers.Areas.Marketing.Models.Estimacion;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Ventas.Models.SolicitudGasto;
using System.ComponentModel.DataAnnotations.Schema;


namespace PortalRoemmers.Areas.Marketing.Models.SolicitudGastoMkt
{
    public class DetSolGasto_GasModels
    {
        //Composicion Fuerte - id de la Solicitud Relacionada
        [Display(Name = "Código Sol.")]
        [StringLength(7)]
        public string idSolGas { get; set; }
        [ForeignKey("idSolGas")]
        public  SolicitudGastoModels solicitud { get; set; }

        //Agregacion Simple - id de la Actividad  Relacionada
        [Display(Name = "Actividad")]
        [StringLength(11)]
        public string idActiv { get; set; }
        [ForeignKey("idActiv")]
        public virtual ActividadModels actividad { get; set; }


        //Agregacion Simple - id del Gasto  Relacionada
        [Display(Name = "Gasto")]
        [StringLength(10)]
        public string idActGas { get; set; }
        [ForeignKey("idActGas")]
        public  ActividadGastoModels gasto { get; set; }

        //------------------------------------------------------------
        public virtual DetEstim_GastActModels detEstGas { get; set; }
        //------------------------------------------------------------

        //Importe Total
        [Display(Name = "Importe Total")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Double monTotal { get; set; }

        //Importe Promedio
        [Display(Name = "Importe Promedio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Double monProm { get; set; }

        //Observacion de la actividad
        [Display(Name = "Observacion")]
        [StringLength(50)]
        public string obsAct { get; set; }

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