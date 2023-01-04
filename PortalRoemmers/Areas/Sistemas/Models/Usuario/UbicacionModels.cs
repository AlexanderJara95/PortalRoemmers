using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class UbicacionModels
    {
        [Key]
        [Display(Name = "Ubicación")]
        public string  cCod_Ubi { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Código Pais")]
        public string cCod_Pais { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Pais")]
        public string cPais { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Código Departamento")]
        public string cCod_Dpto { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Departamento")]
        public string cDepartamento  { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Código Provincia")]
        public string cCod_Provincia    { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Provincia")]
        public string cProvincia { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Código Distrito")]
        public string cCod_Distrito { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Distrito")]
        public string cDistrito { get; set; }

        public List<EmpleadoModels> empleado { get; set; }

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