using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Ventas.Models.SolicitudGasto;
using PortalRoemmers.Security;

namespace PortalRoemmers.Areas.Sistemas.Models.Presupuesto
{
    public class MovimientoPresModels
    {
        //Id Llave Conjunta para el ingreso de un movimiento
        [Display(Name = "Id Presupuesto")]
        [StringLength(7)]
        public string idPres { get; set; }
        //Id Llave Conjunta para el ingreso de un movimiento
        [Display(Name = "Id Solicitud")]
        [StringLength(7)]
        public string idSolGas { get; set; }

        public SolicitudGastoModels solGasto { get; set; }
        public PresupuestoModels presupuesto { get; set; }

        //Monto de la Solicitud
        [Display(Name = "Monto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Double monSolGas { get; set; }
        //Moneda Original de la Solicitud
        [Display(Name = "Moneda")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idMon { get; set; }
        [ForeignKey("idMon")]
        public  MonedaModels moneda { get; set; }
        
        //Tipo de cambio utilizado en la solicitud
        [Display(Name = "Tipo de Cambio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [TipoCambio(ErrorMessage = "El Tipo de cambio no puede estar en 0")]
        public Double valtipCam { get; set; }
        
        //Estado de la solicitud
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }
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

        //Diferencia de Modificacion de Saldo
        [NotMapped]
        [Display(Name = "Dif")]//tipo de gastos
        public double diferencia { get; set; }
    }
}