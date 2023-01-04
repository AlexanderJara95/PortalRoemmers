using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Models.Medico;
using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Areas.Ventas.Models.SolicitudGasto;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PortalRoemmers.Areas.Sistemas.Models.Proveedor;
using PortalRoemmers.Areas.Sistemas.Models.Trilogia;
using PortalRoemmers.Areas.Marketing.Models.Actividad;
using System;
using PortalRoemmers.Areas.Sistemas.Models.Solicitud;

namespace PortalRoemmers.Areas.Sistemas.Models.Global
{
    public class EstadoModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idEst { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomEst { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string desEst { get; set; }

        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Modulos")]
        public string modEst { get; set; }

        //ya estan
        public List<TipoSolModels> tipSol { get; set; }//listo
        public List<MedicoModels> clientes { get; set; }//listo
        public List<ProveedorModels> proveedores { get; set; }//no tiene mantenimiento
        public List<EquipoModels> equipos { get; set; }//listo
        public List<LineaModels> lineas { get; set; }//listo
        public List<ZonaModels> zonas { get; set; }//listo
        public List<PresupuestoModels> presupuesto { get; set; }//listo
        public List<EmpleadoModels> empleado { get; set; }//listo
        public List<ConceptoGastoModels> cnpGastos { get; set; }//listo
        public List<MovimientoPresModels> movPres { get; set; }//no tiene mantenimiento
        public List<FirmasSoliGastoModels> firmas { get; set; }//no tiene mantenimiento
        public List<TipoPagoModels> tipPagos { get; set; }//listo
        public List<UsuarioModels> usuarios { get; set; }//listo
        public List<TipoPresupuestoModels> tipPres { get; set; }//listo
        public List<TipoComprobanteModels> tipComp { get; set; }//no tiene mantenimiento
        public List<ProductoModels> productos { get; set; }//listo
        public List<EspecialidadModels> especialidades { get; set; }//listo
        public List<Usu_Zon_Lin_Models> u_z_m { get; set; }//listo
        public List<SolicitudGastoModels> solgas { get; set; }
        public List<ActividadModels> actividades { get; set; }

        //Auditoria
        [Display(Name = "Usuario creación")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
        [Display(Name = "Usuario modificación")]
        public string usuMod { get; set; }
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }
    }
}