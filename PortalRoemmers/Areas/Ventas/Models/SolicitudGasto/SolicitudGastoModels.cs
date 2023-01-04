using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Marketing.Models.SolicitudGastoMkt;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using PortalRoemmers.Areas.Sistemas.Models.Solicitud;

namespace PortalRoemmers.Areas.Ventas.Models.SolicitudGasto
{
    public class SolicitudGastoModels
    {
        //codigo 1
        [Key]
        [Display(Name = "Código")]
        [StringLength(7)]
        public string idSolGas { get; set; }

        //Responsable 2
        [Display(Name = "Responsable")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idAccRes { get; set; }
        [ForeignKey("idAccRes")]
        public  UsuarioModels responsable { get; set; }

        //Solicitante 3
        [Display(Name = "Solicitante")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idAccSol { get; set; }
        [ForeignKey("idAccSol")]
        public  UsuarioModels solicitante { get; set; }

        //Aprobador 4
        [Display(Name = "Aprobador")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idAccApro{ get; set; }
        [ForeignKey("idAccApro")]
        public  UsuarioModels aprobador { get; set; }

        //Concepto gastos 5
        [Display(Name = "Concepto Gasto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idConGas { get; set; }
        [ForeignKey("idConGas")]
        public  ConceptoGastoModels concepto { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Tipo Gasto")]//tipo de gastos
        public string idTipGas { get; set; }

        //Zona 6
        [Display(Name = "Zona")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idZon { get; set; }
        [ForeignKey("idZon")]
        public  ZonaModels zona { get; set; }

        //Linea 7
        [Display(Name = "Linea")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idLin { get; set; }
        [ForeignKey("idLin")]
        public  LineaModels linea { get; set; }

        //Lugar 8
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Lugar")]
        public string lugSolGas { get; set; }

        //fecha evento 9
        [Display(Name = "Fecha Evento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchEveSolGas { get; set; }

        //Titulo 10
        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Título")]
        public string titSolGas { get; set; }

        //Observacion 11
        [Display(Name = "Observación")]
        public string obsSolGas { get; set; }

        //Monto 12
        [Display(Name = "Monto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [TipoCambio(ErrorMessage = "El Total no puede estar en 0")]
        public Double monSolGas { get; set; }

        //Moneda 13
        [Display(Name = "Moneda")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idMon { get; set; }
        [ForeignKey("idMon")]
        public  MonedaModels moneda { get; set; }

        //estado 14
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

        //especialidad 15
        [Display(Name = "Especialidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idEsp { get; set; }
        [ForeignKey("idEsp")]
        public  EspecialidadModels especialidad { get; set; }

        //Tipo de Pago 16
        [Display(Name = "Tipo de Pago")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]    
        [StringLength(10)]
        public string idTipPag { get; set; }
        [ForeignKey("idTipPag")]
        public  TipoPagoModels pago { get; set; }

        //Tipo de Solicitud 17
        [Display(Name = "Tipo de Solicitud")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idTipSol { get; set; }
        [ForeignKey("idTipSol")]
        public  TipoSolModels solicitud { get; set; }

        //Tipo de cambio 18
        [Display(Name = "Tipo de Cambio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [TipoCambio(ErrorMessage = "El Tipo de cambio no puede estar en 0")]
        public Double valtipCam { get; set; }

        //Porcentaje de impuesto
        [Display(Name = "Impuesto")]
        public Double? monImp { get; set; }

        //nivel de aprobacion
        [Display(Name = "Nivel Aprobación")]
        [StringLength(10)]
        public string idNapro { get; set; }
        [ForeignKey("idNapro")]
        public  NivelAproModels nivelA { get; set; }

        /// <summary>
        /// Detalles 
        /// </summary>
        public  List<FirmasSoliGastoModels> dFirma { get; set; }
        public  List<DetSolGasto_RespModels> dResp { get; set; }
        public  List<DetSolGasto_MedModels> dMed { get; set; }
        public  List<DetSolGasto_FamModels> dFam { get; set; }
        public  List<DetSolGasto_DocModels> dDoc { get; set; }
        public  List<DetSolGasto_GasModels> dGas { get; set; }
        public  List<DetSolGasto_FileModels> dFil { get; set; }
        public List<DetSolGasto_AreaTerapModels> dAre { get; set; }

        public virtual LiquidaGastoModels liquidacion { get; set; }

        //Valor Neto
        [Display(Name = "Neto")]
        public Double? monNeto { get; set; }

        //modulo
        [Display(Name = "Módulo")]
        [StringLength(1, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string modSolGas { get; set; }

        //auditoria
        [Display(Name = "Usuario Creación")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha Creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
        public string usuMod { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }

        public  List<MovimientoPresModels> movimiento { get; set; }


        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Monto Descontado")]
        public double? monDes { get; set; }
        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Procede")]
        public Boolean procede { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Opcion")]
        public Boolean option { get; set; }

        //presupuesto
        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Presupuesto")]
        public List<string> idPres { get; set; }

        //marketing
        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Actividad")]
        public string idActiv { get; set; }
        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Estimación")]
        public string idActivEst { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Tipo Gasto")]
        public string idTipGasAct { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Gasto")]
        public string idActGas { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Cantidad")]
        public int monCant { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Precio Unitario")]
        public double monProm { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Total")]
        public double monTotal { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Procede Actividad")]
        public Boolean procedeA { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Total")]
        public double? monDetalle { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Cantidad Documentos")]
        public int? cantDoc { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Procede Actividad")]
        public Boolean procedeMov { get; set; }
    }
}