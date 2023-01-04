using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Medico
{
    public class MedicoModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idCli { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        [Index(name: "IX_nomCli")]
        public string nomCli { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchNacCli { get; set; }

        [Display(Name = "Nro Documento")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string nroDocCli { get; set; }

        [Display(Name = "Matricula")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string nroMatCli { get; set; }

        [Display(Name = "Close UP")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string nroCloUPCli { get; set; }

        [Display(Name = "Teléfono")]
        [StringLength(15, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string nroTelCli { get; set; }

        [StringLength(80, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Correo electrónico")]
        public string corEleCli { get; set; }

        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Código Sigma")]
        public string codSigma { get; set; }


        [Display(Name = "Tipo de Documento ")]
        [StringLength(10)]
        public string idTipDoc { get; set; }
        [ForeignKey("idTipDoc")]
        public  TipDocIdeModels tipDoc { get; set; }

        [Display(Name = "Genero")]
        [StringLength(10)]
        public string idGen { get; set; }
        [ForeignKey("idGen")]
        public  GeneroModels genero { get; set; }

        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

        [Display(Name = "Tipo Cliente")]
        [StringLength(10)]
        public string idTipCli { get; set; }
        [ForeignKey("idTipCli")]
        public  TipoMedicoModels tipoCliente { get; set; }

        [Display(Name = "Tipo Cliente")]
        [StringLength(10)]
        public string idEsp { get; set; }
        [ForeignKey("idEsp")]
        public  EspecialidadModels especialidad { get; set; }

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