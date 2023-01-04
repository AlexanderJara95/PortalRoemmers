using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Marketing.Models.Actividad;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Security;

namespace PortalRoemmers.Areas.Marketing.Models.Estimacion
{
    public class EstimacionModels
    {
        //Codigo de la actividad
        [Key]
        [Display(Name = "Actividad")]
        [StringLength(11)]
        public string idActiv { get; set; }
        [ForeignKey("idActiv")]
        public  ActividadModels actividad { get; set; }

        //Codigo de la linea responsable
        [Display(Name = "Linea")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idLin { get; set; }
        [ForeignKey("idLin")]
        public  LineaModels linea { get; set; }

        //codigo de tipo de gasto (2021)
        [Display(Name = "Tipo de Gasto")]
        [StringLength(10)]
        public string idTipGas { get; set; }
        public TipoGastoModels tipogasto { get; set; }

        //codigo de concepto de gasto (2021)
        [Display(Name = "Concepto Gasto")]
        [StringLength(10)]
        public string idConGas { get; set; }
        [ForeignKey("idConGas")]
        public ConceptoGastoModels concepto { get; set; }

        //Numero de Asistentes
        [Display(Name = "Cantidad de Asistentes")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int numAsist { get; set; }

        //Numero de Staff
        [Display(Name = "Cantidad de Staff")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int numStaff { get; set; }

        //Moneda de la estimacion
        [Display(Name = "Moneda")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idMon { get; set; }
        [ForeignKey("idMon")]
        public  MonedaModels moneda { get; set; }

        //Tipo de cambio
        [Display(Name = "Tipo de Cambio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [TipoCambio(ErrorMessage = "El Tipo de cambio no puede estar en 0")]
        public Double valtipCam { get; set; }

        //Monto de la estimacion
        [Display(Name = "Inversion")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Double monEstGas { get; set; }

        //Detalle Adicional de la estimacion
        [Display(Name = "Observacíon")]
        public string obsEstGas { get; set; }

        //Estado de la Estimacion
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

        //Id del Presupuesto Estimado
        [Display(Name = "Presupuesto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(7)]
        public string idPres { get; set; }
        [ForeignKey("idPres")]
        public   PresupuestoModels presupuesto { get; set; }

        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreActiv { get; set; }
        //Fecha de modificacion
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModActiv { get; set; }
        //Usuario creacion
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreActiv { get; set; }
        //Fecha de modificacion
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModActiv { get; set; }
        //----------------------------Relaciones--------------------------------
        public  List<DetEstim_GastActModels> detalleEstim_Gas { get; set; }
        public  List<DetEstim_FamProdModels> detalleEstim_Fam { get; set; }
        //----------------------------Not Mapped--------------------------------
        //presupuesto
        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Procede")]
        public Boolean procede { get; set; }

        //Codigo del usuario responsable
        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Responsable")]
        [StringLength(10)]
        public string idAccRes { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Especialidad")]
        [StringLength(10)]
        public string idEsp { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Tipo de Gasto")]
        [StringLength(10)]
        public string idTipGasAct { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Total Gasto Medico")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "#,##0")]
        public Double totalGM { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Total Gasto Staff")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "#,##0")]
        public Double totalGS { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Total Gasto Varios")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "#,##0")]
        public Double totalGV { get; set; }
    }
}