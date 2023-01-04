using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace PortalRoemmers.Areas.Sistemas.Models.Proveedor
{
    public class TipoProveedorModels
    {
        //id Tipo de Proveedor segun AX
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idTipPro { get; set; }

        //Nombre del tipo de proveedor
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        public string nomTipPro { get; set; }

        //Descripcion del tipo de proveedor
        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string desTipPro { get; set; }

        public List<ProveedorModels> proveedores { get; set; }

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