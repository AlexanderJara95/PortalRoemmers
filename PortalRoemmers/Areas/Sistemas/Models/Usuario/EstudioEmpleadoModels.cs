using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class EstudioEmpleadoModels
    {
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idEstu { get; set; }

        //empleado
        [Display(Name = "Empleado")]
        [StringLength(10)]
        public string idEmp { get; set; }
        [ForeignKey("idEmp")]
        public EmpleadoModels empleado { get; set; }


        [Display(Name = "Institución Educativa")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomInsEstu { get; set; }

        //Nivel de Estudio
        [Display(Name = "Nivel de Estudio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idNivEstu { get; set; }
        [ForeignKey("idNivEstu")]
        public NivelEstudioModels nivelEstudio { get; set; }

        [Display(Name = "Carrera y/o Profesión")]
        [StringLength(90, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string carProEstu { get; set; }

        [Display(Name = "Cursando actualmente?")]
        [StringLength(15, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string curActEstu { get; set; }

        [Display(Name = "Desde Mes")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string mesDesEstu { get; set; }

        [Display(Name = "Desde Año")]
        [StringLength(4, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string anioDesEstu { get; set; }

        [Display(Name = "Hasta Mes")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string mesHasEstu { get; set; }

        [Display(Name = "Hasta Año")]
        [StringLength(4, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string anioHasEstu { get; set; }

        //Auditoria
        [Display(Name = "Usuario creación")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
    }
}