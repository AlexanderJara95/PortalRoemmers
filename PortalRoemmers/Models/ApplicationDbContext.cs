using PortalRoemmers.Areas.Sistemas.Models.Roles;
using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Models.Medico;
using PortalRoemmers.Areas.Sistemas.Models.Producto;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Areas.Sistemas.Models.Trilogia;
using PortalRoemmers.Areas.Sistemas.Models.Presupuesto;
using PortalRoemmers.Areas.Sistemas.Models.Proveedor;
using PortalRoemmers.Areas.Sistemas.Models.Menu;
using PortalRoemmers.Areas.Ventas.Models.SolicitudGasto;
using PortalRoemmers.Areas.Marketing.Models.Actividad;
using PortalRoemmers.Areas.Marketing.Models.Estimacion;
using PortalRoemmers.Areas.Marketing.Models.SolicitudGastoMkt;
using PortalRoemmers.Areas.RRHH.Models.Galeria;
using PortalRoemmers.Areas.RRHH.Models.Periodico;
using PortalRoemmers.Areas.RRHH.Models.Bienvenida;
using PortalRoemmers.Areas.Sistemas.Models.Solicitud;
using PortalRoemmers.Areas.Sistemas.Models.Enlace;
using PortalRoemmers.Areas.Almacen.Models.Inventario;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.RRHH.Models.Boleta;
using PortalRoemmers.Areas.Contabilidad.Models.Letra;
using System.Data.Entity.Infrastructure.Annotations;
using PortalRoemmers.Areas.RRHH.Models.Formulario;
using PortalRoemmers.Areas.Marketing.Models.FarmacoVigilancia;
using PortalRoemmers.Areas.RRHH.Models.Documento;
using PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH;

namespace PortalRoemmers.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaulConnectionRoe")
        {

        }
        //usuario
        public DbSet<EmpleadoModels> tb_Empleado { get; set; }
        public DbSet<UsuarioModels> tb_Usuario { get; set; }
        public DbSet<EstCivilModels> tb_EstCiv { get; set; }
        public DbSet<CargoModels> tb_Cargo { get; set; }
        public DbSet<Usu_RolModels> tb_Usu_Rol { get; set; }
        public DbSet<AreaRoeModels> tb_Area { get; set; }
        public DbSet<TipDocIdeModels> tb_DocIde { get; set; }
        public DbSet<UbicacionModels> tb_Ubic { get; set; }
        public DbSet<AfpModels> tb_Afp { get; set; }
        public DbSet<GeneroModels> tb_Gene { get; set; }
        public DbSet<NivelAproModels> tb_NivApr { get; set; }
        public DbSet<AsigAproModels> tb_Asig_Apro { get; set; }
        public DbSet<ConceptoGastoModels> tb_ConGas { get; set; }
        public DbSet<TipoGastoModels> tb_TipGas { get; set; }
        public DbSet<SedeModels> tb_Sede { get; set; }
        public DbSet<TipGas_Usu_Models> tb_TipGas_Usu { get; set; }
        public DbSet<BancoModels> tb_Banco { get; set; }
        public DbSet<SangreModels> tb_San { get; set; }
        public DbSet<PaisModels> tb_Pais { get; set; }
        public DbSet<FamiliaEmpleadoModels> tb_FamEmp { get; set; }
        public DbSet<EstudioEmpleadoModels> tb_EstuEmp { get; set; }
        public DbSet<NivelEstudioModels> tb_NivelEstu { get; set; }
        public DbSet<TipoFamiliaModels> tb_TipFam { get; set; }

        //Menu
        public DbSet<MenuModels> tb_Menu { get; set; }
        public DbSet<TipoMenuModels> tb_TipMenu { get; set; }
        public DbSet<TipoIconModels> tb_TipIcon { get; set; }
        public DbSet<IconModels> tb_Icon { get; set; }

        //roles
        public DbSet<RolesModels> tb_Roles { get; set; }
        public DbSet<TipoRolModels> tb_TipRol { get; set; }

        //Equipo 
        public DbSet<EquipoModels> tb_Equipo { get; set; }
        public DbSet<FabricanteEquipoModels> tb_Fabricante { get; set; }
        public DbSet<ProcesadorModels> tb_Proce { get; set; }
        public DbSet<SistemaOModels> tb_SisOp { get; set; }
        public DbSet<TipoEquipoModels> tb_TipEqui { get; set; }
        public DbSet<ModEquiModels> tb_ModEqui { get; set; }

        public DbSet<TipoDiscoModels> tb_TipDis { get; set; }
        public DbSet<TipoRamModels> tb_TipRam { get; set; }
        //Producto
        public DbSet<ProductoModels> tb_Producto { get; set; }
        public DbSet<FamProdAxModels> tb_FamProdAx { get; set; }
        public DbSet<FamProdRoeModels> tb_FamProdRoe { get; set; }
        public DbSet<LaboratorioModels> tb_LabPro { get; set; }
        public DbSet<AreaTerapeuticaModels> tb_AreaTerap { get; set; }

        //Visitador
        public DbSet<LineaModels> tb_Linea { get; set; }
        public DbSet<ZonaModels> tb_Zona { get; set; }
        public DbSet<Pro_LIn_Models> tb_Pro_Lin { get; set; }
        public DbSet<Usu_Zon_Lin_Models> tb_Usu_Zon_Lin { get; set; }
        public DbSet<EspecialidadModels> tb_Especialidad { get; set; }
        public DbSet<FirmasSoliGastoModels> tb_FirSolGas { get; set; }
        public DbSet<Esp_Usu_Models> tb_Esp_Usu { get; set; }

        //Medico(Cliente)
        public DbSet<MedicoModels> tb_Medico { get; set; }
        public DbSet<TipoMedicoModels> tb_TipMed { get; set; }

        //Proveedor
        public DbSet<TipoProveedorModels> tb_TipPro { get; set; }
        public DbSet<ProveedorModels> tb_Proveedor { get; set; }

        //Global
        public DbSet<CodigoModels> tb_IdTablas { get; set; }
        public DbSet<EstadoModels> tb_Estado { get; set; }
        public DbSet<MonedaModels> tb_Moneda { get; set; }
        public DbSet<TipoPagoModels> tb_TipPag { get; set; }
        public DbSet<TipoSolModels> tb_TipSol { get; set; }
        public DbSet<TipoComprobanteModels> tb_TipComp { get; set; }
        public DbSet<ParametroModels> tb_Parametro { get; set; }
        public DbSet<ParDetalleModels> tb_DetPar { get; set; }
        public DbSet<ByteModels> tb_Byte { get; set; }
        public DbSet<AtributoHTMLModels> tb_AtrHtml { get; set; }

        //Solicitud de Gasto
        public DbSet<SolicitudGastoModels> tb_SolGastos { get; set; }
        public DbSet<DetSolGasto_FamModels> tb_DetSolGas_Fam { get; set; }
        public DbSet<TipoCambioModels> tb_TipoCambio { get; set; }
        public DbSet<DetSolGasto_MedModels> tb_DetSolGas_Med { get; set; }
        public DbSet<DetSolGasto_RespModels> tb_DetSolGas_Res { get; set; }
        public DbSet<DetSolGasto_DocModels> tb_DetSolGas_Doc { get; set; }
        public DbSet<LiquidaGastoModels> tb_LiqGas { get; set; }
        public DbSet<DetSolGasto_GasModels> tb_DetSolGas_Gas { get; set; }
        public DbSet<DetSolGasto_FileModels> tb_DetSolGas_File { get; set; }
        public DbSet<DetSolGasto_AreaTerapModels> tb_DetSolGas_ATer { get; set; }

        //Presupuesto 
        public DbSet<TipoPresupuestoModels> tb_TipPres { get; set; }
        public DbSet<PresupuestoModels> tb_Pres { get; set; }
        public DbSet<MovimientoPresModels> tb_MovPres { get; set; }

        //Actividad - Estimacion - Participantes
        public DbSet<ActividadModels> tb_Activ { get; set; }
        public DbSet<DetActiv_MedModels> tb_DetAct_Med { get; set; }
        public DbSet<EstimacionModels> tb_Estim { get; set; }
        public DbSet<DetEstim_GastActModels> tb_DetEstim_Gas { get; set; }
        public DbSet<DetEstim_FamProdModels> tb_DetEstim_Fam { get; set; }
        public DbSet<TipGastDeActivModels> tb_TipGasAct { get; set; }
        public DbSet<ActividadGastoModels> tb_GasAct { get; set; }
        //HOME
        public DbSet<GaleriaModels> tb_Galeria { get; set; }
        public DbSet<EfectoImagenModels> tb_EfeIma { get; set; }
        public DbSet<PeriodicoSeccionModels> tb_PeriodicoSec { get; set; }
        public DbSet<ContenidoSeccionModels> tb_ContenidoSec { get; set; }
        public DbSet<TipoGaleriaModels> tb_TipGal { get; set; }
        public DbSet<BienvenidaModels> tb_Bienvenida { get; set; }
        public DbSet<FotosBienvenidaModels> tb_BienFotos { get; set; }
        //Enlace
        public DbSet<TipoEnlaceModels> tb_TipEnl{ get; set; }
        public DbSet<EnlaceModels> tb_Enlace { get; set; }
        //inventario
        public DbSet<InventarioAxModels> tb_InvAx { get; set; }
        public DbSet<InventarioProductoModels> tb_InvPro { get; set; }
        public DbSet<NumeroConteoModels> tb_NumCon { get; set; }
        public DbSet<HistoriaInventarioModels> tb_HisInv { get; set; }
        //Boleta Personal
        public DbSet<BoletaPersonalModels> tb_BolPer { get; set; }
        public DbSet<BoletaDetalleModels> tb_BolDet { get; set; }
        //Letras
        public DbSet<AceptanteModels> tb_Aceptante { get; set; }
        public DbSet<LetraModels> tb_Letra { get; set; }
        public DbSet<FirmasLetraModels> tb_FirLet { get; set; }
        //Formulario
        public DbSet<FormularioModels> tb_Formulario { get; set; }
        public DbSet<Form_Usu_Models> tb_For_Usu { get; set; }

        //Farmaco vigilancia
        public DbSet<EventoAdversoModels> tb_Eve_Adv { get; set; }

        //Documento RRHH
        public DbSet<TipoDocumentoRRHHModels> tb_TipDocRRHH { get; set; }
        public DbSet<DocumentoRRHHModels> tb_DocRRHH { get; set; }

        //Solicitud RRHH
        public DbSet<SolicitudRRHHModels> tb_SolicitudRRHH { get; set; }
        public DbSet<SubtipoSolicitudRRHHModels> tb_SubtipoSolicitudRRHH { get; set; }
        public DbSet<TipoSolicitudRRHHModels> tb_TipoSolicitudRRHH { get; set; }
        public DbSet<UserSolicitudRRHHModels> tb_UserSolicitudRRHH { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //elimina la eliminacion en casacada que esta mal
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //usuario
            modelBuilder.Entity<EmpleadoModels>().ToTable("tb_Empleado");
            modelBuilder.Entity<UsuarioModels>().ToTable("tb_Usuario");
            modelBuilder.Entity<Usu_RolModels>().ToTable("tb_Usu_Rol");
            modelBuilder.Entity<TipDocIdeModels>().ToTable("tb_TipDocIde");
            modelBuilder.Entity<EstCivilModels>().ToTable("tb_EstCiv");
            modelBuilder.Entity<CargoModels>().ToTable("tb_Cargo");
            modelBuilder.Entity<AreaRoeModels>().ToTable("tb_Area");
            modelBuilder.Entity<UbicacionModels>().ToTable("tb_Ubic");
            modelBuilder.Entity<AfpModels>().ToTable("tb_Afp");
            modelBuilder.Entity<GeneroModels>().ToTable("tb_Gene");
            modelBuilder.Entity<NivelAproModels>().ToTable("tb_NivApr");
            modelBuilder.Entity<AsigAproModels>().ToTable("tb_Asig_Apro");
            modelBuilder.Entity<AsigAproModels>().HasKey(x => new { x.idAccApro, x.idAccAsig });
            modelBuilder.Entity<ConceptoGastoModels>().ToTable("tb_ConGas");
            modelBuilder.Entity<TipoGastoModels>().ToTable("tb_TipGas");
            modelBuilder.Entity<TipGas_Usu_Models>().HasKey(x => new { x.idAcc, x.idTipGas});
            modelBuilder.Entity<TipGas_Usu_Models>().ToTable("tb_TipGas_Usu");
            modelBuilder.Entity<SedeModels>().ToTable("tb_Sede");
            modelBuilder.Entity<BancoModels>().ToTable("tb_Banco");
            modelBuilder.Entity<SangreModels>().ToTable("tb_San");
            modelBuilder.Entity<PaisModels>().ToTable("tb_Pais");

            modelBuilder.Entity<FamiliaEmpleadoModels>().ToTable("tb_FamEmp");
            modelBuilder.Entity<FamiliaEmpleadoModels>().HasKey(x => new { x.idEmp, x.dniEmpFam });
            modelBuilder.Entity<EstudioEmpleadoModels>().ToTable("tb_EstuEmp");
            modelBuilder.Entity<EstudioEmpleadoModels>().HasKey(x => new { x.idEmp, x.idEstu });
            modelBuilder.Entity<NivelEstudioModels>().ToTable("tb_NivelEstu");
            modelBuilder.Entity<TipoFamiliaModels>().ToTable("tb_TipFam");
 

            //Muchos a muchos
            modelBuilder.Entity<Usu_RolModels>().HasKey(x => new { x.idAcc, x.rolId });

            //menu
            modelBuilder.Entity<MenuModels>().ToTable("tb_Menu");
            modelBuilder.Entity<TipoMenuModels>().ToTable("tb_TipMenu");
            modelBuilder.Entity<TipoIconModels>().ToTable("tb_TipIcon");
            modelBuilder.Entity<IconModels>().ToTable("tb_Icon");

            //roles
            modelBuilder.Entity<RolesModels>().ToTable("tb_Roles");
            modelBuilder.Entity<TipoRolModels>().ToTable("tb_TipRol");

            //Equipo 
            modelBuilder.Entity<EquipoModels>().ToTable("tb_Equipo");
            modelBuilder.Entity<FabricanteEquipoModels>().ToTable("tb_Fabricante");
            modelBuilder.Entity<ProcesadorModels>().ToTable("tb_Proce");
            modelBuilder.Entity<SistemaOModels>().ToTable("tb_SisOp");
            modelBuilder.Entity<TipoEquipoModels>().ToTable("tb_TipEqui");
            modelBuilder.Entity<ModEquiModels>().ToTable("tb_ModEqui");

            modelBuilder.Entity<TipoDiscoModels>().ToTable("tb_TipDis");
            modelBuilder.Entity<TipoRamModels>().ToTable("tb_TipRam");

            //Producto
            modelBuilder.Entity<ProductoModels>().ToTable("tb_Producto");
            modelBuilder.Entity<FamProdAxModels>().ToTable("tb_FamProdAx");
            modelBuilder.Entity<FamProdRoeModels>().ToTable("tb_FamProdRoe");
            modelBuilder.Entity<LaboratorioModels>().ToTable("tb_LabPro");
            modelBuilder.Entity<AreaTerapeuticaModels>().ToTable("tb_AreaTerap");

            //Visitador
            modelBuilder.Entity<LineaModels>().ToTable("tb_Linea");
            modelBuilder.Entity<ZonaModels>().ToTable("tb_Zona");
            modelBuilder.Entity<EspecialidadModels>().ToTable("tb_Especialidad");
            modelBuilder.Entity<Pro_LIn_Models>().ToTable("tb_Pro_Lin");
            modelBuilder.Entity<Pro_LIn_Models>().HasKey(x => new { x.idProAX, x.idLin });
            modelBuilder.Entity<Usu_Zon_Lin_Models>().ToTable("tb_Usu_Zon_Lin");
            modelBuilder.Entity<Usu_Zon_Lin_Models>().HasKey(x => new { x.idAcc, x.idLin, x.idZon });
            modelBuilder.Entity<Esp_Usu_Models>().HasKey(x => new { x.idAcc, x.idEsp });
            modelBuilder.Entity<Esp_Usu_Models>().ToTable("tb_Esp_Usu");

            //Medico
            modelBuilder.Entity<MedicoModels>().ToTable("tb_Medico");
            modelBuilder.Entity<TipoMedicoModels>().ToTable("tb_TipMed");

            //Proveedor
            modelBuilder.Entity<ProveedorModels>().ToTable("tb_Proveedor");
            modelBuilder.Entity<TipoProveedorModels>().ToTable("tb_TipPro");

            //Global
            modelBuilder.Entity<ByteModels>().ToTable("tb_Byte");
            modelBuilder.Entity<CodigoModels>().ToTable("tb_IdTablas");
            modelBuilder.Entity<EstadoModels>().ToTable("tb_Estado");
            modelBuilder.Entity<MonedaModels>().ToTable("tb_Moneda");
            modelBuilder.Entity<TipoPagoModels>().ToTable("tb_TipPag");
            modelBuilder.Entity<TipoSolModels>().ToTable("tb_TipSol");
            modelBuilder.Entity<TipoCambioModels>().ToTable("tb_TipoCambio");
            modelBuilder.Entity<TipoComprobanteModels>().ToTable("tb_TipComp");
            modelBuilder.Entity<ParametroModels>().ToTable("tb_Parametro");
            modelBuilder.Entity<ParDetalleModels>().ToTable("tb_DetPar");
            modelBuilder.Entity<ParDetalleModels>().HasKey(x => new { x.idPar, x.idDetPar });
            modelBuilder.Entity<AtributoHTMLModels>().ToTable("tb_AtrHtml");
            

            //Solicitud de Gastos
            modelBuilder.Entity<SolicitudGastoModels>().ToTable("tb_SolGastos");
            modelBuilder.Entity<LiquidaGastoModels>().ToTable("tb_LiqGas");
            modelBuilder.Entity<LiquidaGastoModels>().HasRequired(s => s.solicitud).WithRequiredPrincipal(ad => ad.liquidacion);//uno a uno
            modelBuilder.Entity<FirmasSoliGastoModels>().ToTable("tb_FirSolGas");
            modelBuilder.Entity<FirmasSoliGastoModels>().HasKey(x => new { x.idSolGas, x.idAcc, x.idEst });
            modelBuilder.Entity<DetSolGasto_FamModels>().ToTable("tb_DetSolGas_Fam");
            modelBuilder.Entity<DetSolGasto_FamModels>().HasKey(x => new { x.idSolGas, x.idFamRoe });
            modelBuilder.Entity<DetSolGasto_MedModels>().ToTable("tb_DetSolGas_Med");
            modelBuilder.Entity<DetSolGasto_MedModels>().HasKey(x => new { x.idSolGas, x.idCli });
            modelBuilder.Entity<DetSolGasto_RespModels>().ToTable("tb_DetSolGas_Res");
            modelBuilder.Entity<DetSolGasto_RespModels>().HasKey(x => new { x.idSolGas, x.idEmp });
            modelBuilder.Entity<DetSolGasto_DocModels>().ToTable("tb_DetSolGas_Doc");
            modelBuilder.Entity<DetSolGasto_DocModels>().HasKey(x => new { x.idSolGas, x.idDetDoc });
            modelBuilder.Entity<DetSolGasto_GasModels>().ToTable("tb_DetSolGas_Gas");
            modelBuilder.Entity<DetSolGasto_GasModels>().HasKey(x => new { x.idSolGas, x.idActiv,x.idActGas });
            modelBuilder.Entity<DetSolGasto_FileModels>().ToTable("tb_DetSolGas_File");
            modelBuilder.Entity<DetSolGasto_FileModels>().HasKey(x => new { x.idSolGas, x.idFile });
            modelBuilder.Entity<DetSolGasto_AreaTerapModels>().ToTable("tb_DetSolGas_ATer");
            modelBuilder.Entity<DetSolGasto_AreaTerapModels>().HasKey(x => new { x.idSolGas, x.idAreaTerap });

            //Presupuesto
            modelBuilder.Entity<TipoPresupuestoModels>().ToTable("tb_TipPres");
            modelBuilder.Entity<PresupuestoModels>().ToTable("tb_Pres");
            modelBuilder.Entity<MovimientoPresModels>().ToTable("tb_MovPres");
            modelBuilder.Entity<MovimientoPresModels>().HasKey(x => new { x.idPres, x.idSolGas });

            //Actividad - Estimacion - Participantes
            modelBuilder.Entity<ActividadModels>().ToTable("tb_Activ");
            modelBuilder.Entity<DetActiv_MedModels>().ToTable("tb_DetAct_Med");
            modelBuilder.Entity<DetActiv_MedModels>().HasKey(x => new { x.idActiv, x.idCli });
            modelBuilder.Entity<EstimacionModels>().ToTable("tb_Estim");
            modelBuilder.Entity<ActividadModels>().HasRequired(s => s.estimacion).WithRequiredPrincipal(ad => ad.actividad);//uno a uno

            //---Detalles
            modelBuilder.Entity<DetEstim_GastActModels>().ToTable("tb_DetEstim_Gas");
            modelBuilder.Entity<DetEstim_GastActModels>().HasKey(x => new { x.idActiv, x.idActGas });
            modelBuilder.Entity<DetEstim_GastActModels>().HasKey(x => new { x.idActiv, x.idActGas });
            modelBuilder.Entity<DetEstim_GastActModels>().HasMany<DetSolGasto_GasModels>(g => g.detSolGas).WithRequired(s => s.detEstGas).HasForeignKey(x=>new { x.idActiv,x.idActGas});

            modelBuilder.Entity<DetEstim_FamProdModels>().ToTable("tb_DetEstim_Fam");
            modelBuilder.Entity<DetEstim_FamProdModels>().HasKey(x => new { x.idActiv, x.idFamRoe, x.idAreaTerap });
            //modelBuilder.Entity<DetEstim_FamProdModels>().HasKey(x => new { x.idActiv, x.idFamRoe });
            //---Actividad
            modelBuilder.Entity<TipGastDeActivModels>().ToTable("tb_TipGasAct");
            modelBuilder.Entity<ActividadGastoModels>().ToTable("tb_GasAct");
            //HOME
            modelBuilder.Entity<GaleriaModels>().ToTable("tb_Galeria");
            modelBuilder.Entity<TipoGaleriaModels>().ToTable("tb_TipGal");
            modelBuilder.Entity<EfectoImagenModels>().ToTable("tb_EfeIma");
            modelBuilder.Entity<PeriodicoSeccionModels>().ToTable("tb_PeriodicoSec");
            modelBuilder.Entity<ContenidoSeccionModels>().ToTable("tb_ContenidoSec");
            modelBuilder.Entity<PeriodicoSeccionModels>().HasRequired(s => s.contSec).WithRequiredPrincipal(ad => ad.perSec);//uno a uno

            modelBuilder.Entity<BienvenidaModels>().ToTable("tb_Bienvenida");
            modelBuilder.Entity<FotosBienvenidaModels>().ToTable("tb_BienFotos ");
            modelBuilder.Entity<FotosBienvenidaModels>().HasKey(x => new { x.idbien, x.idFotBie });

            //Enlace
            modelBuilder.Entity<TipoEnlaceModels>().ToTable("tb_TipEnl");
            modelBuilder.Entity<EnlaceModels>().ToTable("tb_Enlace");

            //Inventario
            modelBuilder.Entity<InventarioAxModels>().ToTable("tb_InvAx ");
            modelBuilder.Entity<InventarioAxModels>().HasKey(x => new { x.idProInv, x.nroLotInv,x.ubiProInv });
            modelBuilder.Entity<InventarioProductoModels>().ToTable("tb_InvPro ");
            modelBuilder.Entity<InventarioProductoModels>().HasKey(x => new { x.codProCon, x.nroLotCon, x.ubiProCon,x.nroInvCon });
            modelBuilder.Entity<NumeroConteoModels>().ToTable("tb_NumCon");
            modelBuilder.Entity<NumeroConteoModels>().Property(x => x.codCon).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<HistoriaInventarioModels>().ToTable("tb_HisInv");
            modelBuilder.Entity<HistoriaInventarioModels>().HasKey(x => new { x.fchConHis, x.codProConHis, x.nroLotConHis});

            //Boleta Personal
            modelBuilder.Entity<BoletaPersonalModels>().ToTable("tb_BolPer");
            modelBuilder.Entity<BoletaDetalleModels>().ToTable("tb_BolDet");
            modelBuilder.Entity<BoletaDetalleModels>().HasKey(x => new { x.idBolPer, x.nroDocBolDet });

            //Letras (Contabilidad)
            modelBuilder.Entity<AceptanteModels>().ToTable("tb_Aceptante");
            modelBuilder.Entity<LetraModels>().ToTable("tb_Letra");
            modelBuilder.Entity<LetraModels>().Property(t => t.codLetra).HasColumnAnnotation("Index",new IndexAnnotation(new[]{new IndexAttribute("X_codLet") { IsUnique = true } }));
            modelBuilder.Entity<FirmasLetraModels>().ToTable("tb_FirLetra");
            modelBuilder.Entity<FirmasLetraModels>().HasKey(x => new { x.idLetra, x.idAcc, x.idEst });

            //Formulario
            modelBuilder.Entity<FormularioModels>().ToTable("tb_Formulario");
            modelBuilder.Entity<Form_Usu_Models>().ToTable("tb_For_Usu");
            modelBuilder.Entity<Form_Usu_Models>().HasKey(x => new { x.idAcc, x.idFor });

            //Farmaco vigilancia
            modelBuilder.Entity<EventoAdversoModels>().ToTable("tb_Eve_Adv");

            //Documento RRHH
            modelBuilder.Entity<TipoDocumentoRRHHModels>().ToTable("tb_TipDocRRHH");
            modelBuilder.Entity<DocumentoRRHHModels>().ToTable("tb_DocRRHH");

            //SolicitudRRHH
            modelBuilder.Entity<SolicitudRRHHModels>().ToTable("tb_SolicitudRRHH");
            modelBuilder.Entity<SubtipoSolicitudRRHHModels>().ToTable("tb_SubtipoSolicitudRRHH");
            modelBuilder.Entity<TipoSolicitudRRHHModels>().ToTable("tb_TipoSolicitudRRHH");
            modelBuilder.Entity<UserSolicitudRRHHModels>().ToTable("tb_UserSolicitudRRHH");
            modelBuilder.Entity<UserSolicitudRRHHModels>().HasKey(x => new { x.idSolicitudRrhh, x.idAccRes });

            base.OnModelCreating(modelBuilder);
        }

    }
}