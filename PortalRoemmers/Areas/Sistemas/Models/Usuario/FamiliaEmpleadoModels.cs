using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class FamiliaEmpleadoModels
    {
        [Display(Name = "DNI")]
        [StringLength(10)]
        public string dniEmpFam { get; set; }

        //empleado
        [Display(Name = "Empleado")]
        [StringLength(10)]
        public string idEmp { get; set; }
        [ForeignKey("idEmp")]
        public EmpleadoModels empleado { get; set; }

        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string nomComEmpFam { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime fchNacEmpFam { get; set; }

        //tipo de familia
        [Display(Name = "Tipo Familia")]
        [StringLength(10)]
        public string idTipFam { get; set; }
        [ForeignKey("idTipFam")]
        public TipoFamiliaModels tipoFamilia { get; set; }

        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Ocupación")]
        public string ocuEmpFam { get; set; }

        //Auditoria
        [Display(Name = "Usuario creación")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
    }
}