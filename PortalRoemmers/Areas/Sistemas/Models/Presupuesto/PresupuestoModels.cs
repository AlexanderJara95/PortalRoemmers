using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Marketing.Models.Estimacion;

namespace PortalRoemmers.Areas.Sistemas.Models.Presupuesto
{
    public class PresupuestoModels
    {
        //codigo 
        [Key]
        [Display(Name = "Id Presupuesto")]
        [StringLength(7)]
        public string idPres { get; set; }

        //Ref idPres
        [Display(Name = "Ref. Id Pres")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string refIdPres { get; set; }

        //Tipo de Presupuesto
        [Display(Name = "Tipo de Presupuesto")]
        [StringLength(3)]
        public string idTipoPres { get; set; }
        [ForeignKey("idTipoPres")]
        public  TipoPresupuestoModels tipospres { get; set; }

        //Fecha Inicio de Vigencia
        [Display(Name = "Fecha Inicio Vig.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fchIniVigencia { get; set; }

        //Fecha Final de Vigencia
        [Display(Name = "Fecha Fin Vig.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchFinVigencia { get; set; }

        //codigo User
        //[Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Responsable")]
        [StringLength(10)]
        public string idAccRes { get; set; }
        [ForeignKey("idAccRes")]
        public  UsuarioModels responsable { get; set; }

        //codigo UserJefe--CORRECTO esta como User
        //[Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Aprobador")]
        [StringLength(10)]
        public string idAccJ { get; set; }
        [ForeignKey("idAccJ")]
        public  UsuarioModels aprobador { get; set; }

        //codigo de tipo de concepto de gasto
        [Display(Name = "Tipo de Gasto")]
        [StringLength(10)]
        public string idTipGas { get; set; }
        public TipoGastoModels tipogasto { get; set; }

        //codigo de concepto de gasto
        //[Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Concepto Gasto")]
        [StringLength(10)]
        public string idConGas { get; set; }
        [ForeignKey("idConGas")]
        public  ConceptoGastoModels concepto { get; set; }

        //codigo Especialidad
        //[Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Especialidad")]
        [StringLength(10)]
        public string idEsp { get; set; }
        [ForeignKey("idEsp")]
        public  EspecialidadModels especialidad { get; set; }

        //codigo Zona
        //[Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Zona")]
        [StringLength(10)]
        public string idZon { get; set; }
        [ForeignKey("idZon")]
        public  ZonaModels zona { get; set; }

        //codigo Linea
        //[Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Linea")]
        [StringLength(10)]
        public string idLin { get; set; }
        [ForeignKey("idLin")]
        public  LineaModels linea { get; set; }

        //codigo Moneda
        //[Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Moneda")]
        [StringLength(10)]
        public string idMon { get; set; }
        [ForeignKey("idMon")]
        public  MonedaModels moneda { get; set; }

        //Monto del Presupuesto
        [DisplayFormat(DataFormatString = "{0:#,0.00}")]
        [Display(Name = "Monto")]
        public double Monto { get; set; }

        //Saldo Real del Presupuesto
        [DisplayFormat(DataFormatString = "{0:#,0.00}")]
        [Display(Name = "Saldo")]
        public double Saldo { get; set; }

        //Saldo Est del Presupuesto
        [DisplayFormat(DataFormatString = "{0:#,0.00}")]
        [Display(Name = "Estim")]
        public double Estim { get; set; }

        //Diferencia de Modificacion de Saldo
        [NotMapped]
        [Display(Name = "Dif")]//tipo de gastos
        public double diferencia { get; set; }

        //Estado
        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

        //Observacion 
        //Observacion 11
        [Display(Name = "Observación")]
        public string obsPtoGas { get; set; }

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

        //Relaciones
        public List<EstimacionModels> estimaciones { get; set; }//de uno a muchos

        public  List<MovimientoPresModels> movimiento { get; set; }
    }
}