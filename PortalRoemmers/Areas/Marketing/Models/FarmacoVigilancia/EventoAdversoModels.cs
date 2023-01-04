using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Marketing.Models.FarmacoVigilancia
{
    public class EventoAdversoModels
    {
        //Codigo de la actividad
        [Key]
        [Display(Name = "Código")]
        [StringLength(11)]
        public string idEveAdv { get; set; }
        //datos del paciente
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "NOMBRES Y APELLIDOS O INICIALES")]
        public string nomApePacEveAdv { get; set; }

        [Display(Name = "EDAD")]
        public int? edaPacEveAdv { get; set; }

        [Display(Name = "SEXO")]
        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string idGen { get; set; }
        [ForeignKey("idGen")]
        public GeneroModels gene { get; set; }

        [StringLength(12, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "TELÉFONO DE CONTACTO ")]
        public string nroPacEveAdv { get; set; }


        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "EMAIL DE CONTACTO")]
        public string emailPacEveAdv { get; set; }

        //datos del medicamento
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "NOMBRE DEL MEDICAMENTO PRINCIPAL")]
        public string idProAX { get; set; }
        [ForeignKey("idProAX")]
        public ProductoModels producto { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string desProEveAdv { get; set; }


        [StringLength(15, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "LOTE")]
        public string nroLteEveAdv { get; set; }

        [Display(Name = "FECHA DE VENCIMIENTO")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fchVenEveAdv { get; set; }

        //descripcion del evento 
        [Display(Name = "DESCRIPCIÓN DEL EVENTO ADVERSO")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string desAdvEveAdv { get; set; }

        //datos del reportante
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "NOMBRES Y APELLIDOS O INICIALES")]
        public string nomApeRepEveAdv { get; set; }


        [StringLength(12, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "TELÉFONO DE CONTACTO ")]
        public string nroRepEveAdv { get; set; }

        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "EMAIL DE CONTACTO")]
        public string emailRepEveAdv { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Opcion")]
        public Boolean option { get; set; }

        //Datos adicionales

        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "NOMBRES DEL MEDICO TRATANTE")]
        public string nomApeMedEveAdv { get; set; }

        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "TELÉFONO  DE CONTACTO O EMAIL")]
        public string nroMedEveAdv { get; set; }

        [Display(Name = "NOMBRE DE LA  INSTITUCION DE SALUD")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string insSalEveAdv { get; set; }


        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre Completo")]
        public string nomComEmp { get; set; }

        //Auditoria
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuMod { get; set; }
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }

        [Display(Name = "Usuario Visualización")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuVis { get; set; }
        [Display(Name = "Fecha Visualización")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchVis { get; set; }


    }
}