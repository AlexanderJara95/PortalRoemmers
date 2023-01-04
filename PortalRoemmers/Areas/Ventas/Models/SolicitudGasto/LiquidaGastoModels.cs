using PortalRoemmers.Areas.Sistemas.Models.Global;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Ventas.Models.SolicitudGasto
{
    public class LiquidaGastoModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(7)]
        public string idSolGas { get; set; }
        [ForeignKey("idSolGas")]
        public  SolicitudGastoModels solicitud { get; set; }

        [Display(Name = "Valor")]
        public Double liqValRea { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchLiq { get; set; }
        //auditoria
        [Display(Name = "Observacíon")]
        public string obsLiq { get; set; }
        [Display(Name = "Usuario Creación")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha Creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
        [Display(Name = "Usuario modificación")]
        [StringLength(50)]
        public string usuMod { get; set; }
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }

    }
}