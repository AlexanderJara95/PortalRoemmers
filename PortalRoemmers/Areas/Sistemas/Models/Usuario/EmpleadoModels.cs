using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Producto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class EmpleadoModels
    {
        [Key]
        [StringLength(10)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Código AX")]
        public string idEmp { get; set; }

        //datos personales////////////////////////////////////////////////////////////
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Primer nombre")]
        public string nom1Emp { get; set; }

        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Segundo nombre")]
        public string nom2Emp { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Apellido Paterno ")]
        public string apePatEmp { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Apellido Materno ")]
        public string apeMatEmp { get; set; }

        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre Completo")]
        public string nomComEmp { get; set; }

        //Tipo de documento
        [Display(Name = "Tipo de Documento ")]
        [StringLength(10)]
        public string idTipDoc { get; set; }
        [ForeignKey("idTipDoc")]
        public TipDocIdeModels tipDoc { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(15)]
        [Display(Name = "Número Documento")]
        [Index(IsUnique = true)]
        public string nroDocEmp { get; set; }

        //estado civil
        [Display(Name = "Estado Civil")]
        [StringLength(10)]
        public string idEstCiv { get; set; }
        [ForeignKey("idEstCiv")]
        public EstCivilModels estCiv { get; set; }

        //Genero
        [Display(Name = "Genero")]
        [StringLength(10)]
        public string idGen { get; set; }
        [ForeignKey("idGen")]
        public GeneroModels gene { get; set; }

        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Lugar Nacimiento")]
        public string lugNacEmp { get; set; }

        //Nacio
        [Display(Name = "Nacionalidad")]
        [StringLength(10)]
        public string idPais { get; set; }
        [ForeignKey("idPais")]
        public PaisModels paisNac { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime nacEmp { get; set; }

        //direccion
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Dirección")]
        public string dirEmp { get; set; }

        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Referencia")]
        public string refDirEmp { get; set; }

        //Distrito
        [Display(Name = "Distrito")]
        public string cCod_Ubi { get; set; }
        [ForeignKey("cCod_Ubi")]
        public UbicacionModels ubicacion { get; set; }

        //contacto
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Número Telefónico")]
        public string numTeleEmp { get; set; }

        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Número Celular")]
        public string numCelEmp { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Correo Electronico Personal")]
        public string emailPerEmp { get; set; }

        //grupo sanguinio 
        [Display(Name = "Grupo Sanguinio")]
        public string idSan { get; set; }
        [ForeignKey("idSan")]
        public SangreModels sanguinio { get; set; }


        //datos laborales////////////////////////////////////////////////////////////
        //informacion laboral
        [Display(Name = "Fecha Ingreso")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? ingfchEmp { get; set; }

        [Display(Name = "Fecha cesado")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? cesfchEmp { get; set; }

        //AFP
        [Display(Name = "AFP")]
        [StringLength(10)]
        public string idAfp { get; set; }
        [ForeignKey("idAfp")]
        public AfpModels afp { get; set; }

        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Codigo afiliacion")]
        public string codAfiEmp { get; set; }

        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Comision")]
        public string comAfiEmp { get; set; }

        [Display(Name = "F. Ingreso Afiliado")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchIngAfiEmp { get; set; }

        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Seguro Salud")]
        public string segSalEmp { get; set; }

        [Display(Name = "Banco CTS")]
        [StringLength(10)]
        public string idBancts { get; set; }
        [ForeignKey("idBancts")]
        public BancoModels bancoCTS { get; set; }

        [Display(Name = "Moneda Cta CTS")]
        [StringLength(10)]
        public string idMon { get; set; }
        [ForeignKey("idMon")]
        public MonedaModels moneda { get; set; }

        [Display(Name = "Banco Sueldo")]
        [StringLength(10)]
        public string idBansuel { get; set; }
        [ForeignKey("idBansuel")]
        public BancoModels bancoSuel { get; set; }

        [StringLength(25, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nro. Cta. sueldo")]
        public string nroCtaSueEmp { get; set; }

        [StringLength(25, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nro. Cta. interbancario")]
        public string nroCtaInEmp { get; set; }

        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "¿Vives con tus padres?")]
        public string vivConPadEmp { get; set; }

        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "¿Tienes Hijos?")]
        public string hijTieEmp { get; set; }

        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre de Contacto")]
        public string perConEmp { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Dirección de Contacto")]
        public string dirConEmp { get; set; }

        [StringLength(12, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Celular de Contacto")]
        public string celConEmp { get; set; }

        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Correo de Contacto")]
        public string corConEmp { get; set; }

        //Area 
        [Display(Name = "Area")]
        [StringLength(10)]
        public string idAreRoe { get; set; }
        [ForeignKey("idAreRoe")]
        public AreaRoeModels area { get; set; }

        //Cargo
        [Display(Name = "Cargo")]
        [StringLength(10)]
        public string idCarg { get; set; }
        [ForeignKey("idCarg")]
        public CargoModels cargo { get; set; }

        //Sede
        [Display(Name = "¿Sede donde laboras?")]
        [StringLength(10)]
        public string cod_sede { get; set; }
        [ForeignKey("cod_sede")]
        public SedeModels sede { get; set; }

        //Jefe
        [Display(Name = "Jefe")]
        public string idEmpJ { get; set; }
        [ForeignKey("idEmpJ")]
        public  EmpleadoModels jefe { get; set; }
        public  ICollection<EmpleadoModels> Childs { get; set; }

        //Estado General
        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

        //muchos
        public List<ProductoModels> productos { get; set; }
        public List<EquipoModels> equipos { get; set; }
        public List<UsuarioModels> usuarios { get; set; }

        //No mapeados
        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "País")]
        public string cCod_Pais { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Departamento")]
        public string cCod_Dpto { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Provincia")]
        public string cCod_Provincia { get; set; }

        //auditoria
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreUsu { get; set; }

        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModUsu { get; set; }

        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreUsu { get; set; }

        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModUsu { get; set; }

    }
}