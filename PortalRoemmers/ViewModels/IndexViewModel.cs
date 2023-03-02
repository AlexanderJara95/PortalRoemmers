using PortalRoemmers.Areas.Sistemas.Models.Roles;
using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Proveedor;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Models.Medico;
using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Models;
using System.Collections.Generic;
using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Sistemas.Models.Menu;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using PortalRoemmers.Areas.Ventas.Models.SolicitudGasto;
using PortalRoemmers.Areas.Sistemas.Models.Trilogia;
using PortalRoemmers.Areas.Marketing.Models.Actividad;
using PortalRoemmers.Areas.Marketing.Models.Estimacion;
using PortalRoemmers.Areas.Marketing.Models.SolicitudGastoMkt;
using PortalRoemmers.Areas.RRHH.Models.Bienvenida;
using PortalRoemmers.Areas.RRHH.Models.Galeria;
using PortalRoemmers.Areas.RRHH.Models.Periodico;
using PortalRoemmers.Areas.Sistemas.Models.Solicitud;
using PortalRoemmers.Areas.Sistemas.Models.Enlace;
using PortalRoemmers.Areas.Almacen.Models.Inventario;
using PortalRoemmers.Areas.RRHH.Models.Boleta;
using PortalRoemmers.Areas.Contabilidad.Models.Letra;
using PortalRoemmers.Areas.RRHH.Models.Formulario;
using PortalRoemmers.Areas.Marketing.Models.FarmacoVigilancia;
using PortalRoemmers.Areas.RRHH.Models.Documento;
using PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH;
using PortalRoemmers.Areas.RRHH.Models.SolicitudesRRHH;

namespace PortalRoemmers.ViewModels
{
    public class IndexViewModel : BaseModelo
    {
        //sistemas****************************************************

        //Clientes
        public List<TipoMedicoModels> TipCli { get; set; }
        public List<MedicoModels> Clientes { get; set; }
        //Proveedores
        public List<TipoProveedorModels> TipPro { get; set; }
        public List<ProveedorModels> Proveedores { get; set; }
        //Equipo
        public List<EquipoModels> Equipos { get; set; }
        public List<FabricanteEquipoModels> Fabricantes { get; set; }
        public List<ModEquiModels> ModelosE { get; set; }
        public List<ProcesadorModels> Procesadores { get; set; }
        public List<SistemaOModels> S_Operativos { get; set; }
        public List<TipoEquipoModels> T_Equipos { get; set; }

        //Gasto
        public List<ConceptoGastoModels> ConGasto { get; set; }
        public List<TipoGastoModels> TipGasto { get; set; }

        //Global
        public List<EstadoModels> Estado  { get; set; }
        public List<MonedaModels> Moneda { get; set; }
        public List<TipoPagoModels> TipoPagos { get; set; }
        public List<TipoSolModels> TipoSol { get; set; }
        public List<CodigoModels> Codigos { get; set; }
        public List<TipoCambioModels> TiposCambio { get; set; }
        public List<ParametroModels> Parametros { get; set; }

        //producto
        public List<ProductoModels> Productos { get; set; }
        public List<FamProdAxModels> FamiliaAX { get; set; }
        public List<FamProdRoeModels> FamiliaRoe { get; set; }
        public List<LaboratorioModels> Laboratorio { get; set; }
        public List<AreaTerapeuticaModels> AreaTerapeutica { get; set; }

        //Roles
        public List<Usu_RolModels> Usu_Rol { get; set; }
        public List<RolesModels> Roles { get; set; }
        public List<TipoRolModels> TipRoles { get; set; }

        //Menu
        public List<MenuModels> Menu { get; set; }
        public List<TipoMenuModels> TipoMenu { get; set; }


        //Usuario
        public List<AfpModels> Afp { get; set; }
        public List<AreaRoeModels> AreaRoe { get; set; }
        public List<CargoModels> Cargo { get; set; }
        public List<EstCivilModels> Estado_C { get; set; }
        public List<GeneroModels> Genero { get; set; }
        public List<NivelAproModels> Nivel { get; set; }
        public List<TipDocIdeModels> T_Doc { get; set; }
        public List<UbicacionModels> Ubicacion { get; set; }
        public List<UsuarioModels> Usuarios { get; set; }
        public List<EmpleadoModels> Empleado { get; set; }

        //Visitador
        public List<LineaModels> Linea { get; set; }
        public List<ZonaModels> Zona { get; set; }
        public List<EspecialidadModels> Especialidad { get; set; }

        //Enlace
        public List<TipoEnlaceModels> TEnlace { get; set; }
        public List<EnlaceModels> Enlace { get; set; }


        //ventas****************************************************
        //Solicitud de gasto
        public List<SolicitudGastoModels> SolGasto { get; set; }
        public List<DetSolGasto_GasModels> DetSolGasto { get; set; }
        public List<Usu_Zon_Lin_Models> UZL { get; set; }

        //Presupuesto de Gastos
        public List<TipoPresupuestoModels> TipPres { get; set; }
        public List<PresupuestoModels> Pres { get; set; }

        //Actividad
        public List<ActividadModels> Actividades { get; set; }
        public List<TipGastDeActivModels> tipoGastDeActiv { get; set; }
        public List<ActividadGastoModels> GastosDeActiv { get; set; }

        //Congresos
        public List<EstimacionModels> Estimaciones { get; set; }

        //RRHH****************************************************
        //Bienvenida
        public List<BienvenidaModels> Bienvenida { get; set; }
        //Galeria
        public List<TipoGaleriaModels> TipoGaleria { get; set; }
        public List<GaleriaModels> Galeria { get; set; }
        //Periodico
        public List<PeriodicoSeccionModels> Periodico { get; set; }
        public List<ContenidoSeccionModels> Contenido { get; set; }
        public List<BoletaPersonalModels> Boleta { get; set; }
        //Formulario
        public List<FormularioModels> Formulario { get; set; }

        //Documento RRHH
        public List<TipoDocumentoRRHHModels> TipDocRRHH { get; set; }
        public List<DocumentoRRHHModels> DocRRHH { get; set; }

        //Solicitud RRHH
        public List<SolicitudRRHHModels> SoliRRHH { get; set; }
        public List<SubtipoSolicitudRRHHModels> SubTipoSoliRRHH { get; set; }
        public List<TipoSolicitudRRHHModels> TipoSoliRRHH { get; set; }
        public List<UserSolicitudRRHHModels> userSoliRRHH { get; set; }


        //Almacen****************************************************
        //Inventario
        public List<InventarioAxModels> Inventario { get; set; }
        public List<InventarioProductoModels> Conteo { get; set; }
        public List<NumeroConteoModels> ConteoR { get; set; }
        public List<HistoriaInventarioModels> HistoriaInv { get; set; }
        //Contabilidad****************************************************
        //Letra
        public List<AceptanteModels> Aceptantes { get; set; }
        public List<LetraModels> Letras { get; set; }

        //Marketing****************************************************
        public List<EventoAdversoModels> EventoAdverso { get; set; }
    }
}