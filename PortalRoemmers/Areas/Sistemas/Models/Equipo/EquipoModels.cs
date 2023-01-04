using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PortalRoemmers.Areas.Sistemas.Models.Equipo
{
    public class EquipoModels
    {
        //Id
        [Key]
        [StringLength(10)]
        [Display(Name = "Código")]
        public string idEquipo { get; set; }

        //Fabricante equipo
        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Fabricante")]
        [StringLength(10)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string idFabrica { get; set; }

        //procesador
        [Display(Name = "Procesador")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idProce { get; set; }
        [ForeignKey("idProce")]
        [Display(Name = "Procesador")]
        public ProcesadorModels procesador { get; set; }

        //Memoria
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Memoria Ram")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string memEqui { get; set; }

        //Disco
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Disco Duro")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string discEqui { get; set; }

        //Nro Nucleo logicos
        [Display(Name = "Detalle")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string detEqui { get; set; }

        //Tipo Equipo
        [Display(Name = "Tipo")]
        [StringLength(10)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string idTipEqui { get; set; }
        [ForeignKey("idTipEqui")]
        public TipoEquipoModels tipEqui { get; set; }

        //Ubicacion Equipo
        [Display(Name = "Ubicación")]
        [StringLength(10)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string idAreRoe { get; set; }
        [ForeignKey("idAreRoe")]
        public AreaRoeModels area { get; set; }

        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Compilación S.O")]
        public string comEqui { get; set; }

        [Display(Name = "Instalación S.O")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchInsEqui { get; set; }

        //Sistema operativo 
        [Display(Name = "Sistema Operativo")]
        [StringLength(10)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string idSo { get; set; }
        [ForeignKey("idSo")]
        [Display(Name = "Sistema Operativo")]
        public SistemaOModels sisOp { get; set; }

        //Aruitectura Sistema
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Arquitectura")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string arqSis { get; set; }

        //usuario que tiene equipo
        [Display(Name = "Usuario")]
        [StringLength(10)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string idEmp { get; set; }
        [ForeignKey("idEmp")]
        public EmpleadoModels empleado { get; set; }

        //modelo equipo
        [Display(Name = "Modelo")]
        [StringLength(10)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string idMolEq { get; set; }
        [ForeignKey("idMolEq")]
        public ModEquiModels modelos { get; set; }

        //Nombre Pc
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre Equipo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomPcEqui { get; set; }

        //Estado
        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public EstadoModels estado { get; set; }

        //Numero de serie
        [StringLength(15, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Númeo de serie")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nroSerEqui { get; set; }

        //Numero de anydesk
        [StringLength(15, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Anydesk")]
        public string nroAnyEqui { get; set; }

        //Tipo de Disco
        [Display(Name = "Tipo Disco")]
        [StringLength(5)]
        public string idTipDis { get; set; }
        [ForeignKey("idTipDis")]
        public TipoDiscoModels tipoDisco { get; set; }

        //Tipo de Ram
        [Display(Name = "Tipo Ram")]
        [StringLength(5)]
        public string idTipRam { get; set; }
        [ForeignKey("idTipRam")]
        public TipoRamModels tipoRam { get; set; }

        //Cantidad de discos
        [Display(Name = "Cantidad Disco")]
        [StringLength(5, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string canDis { get; set; }

        //cantidad de ram
        [Display(Name = "Cantidad Ram")]
        [StringLength(5, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string canRam { get; set; }

        //Detalle de discos
        [Display(Name = "Detalle Discos")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string detDis { get; set; }

        //otros
        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [NotMapped]
        public string tamMen { get; set; }

        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [NotMapped]
        public string tamDis { get; set; }

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