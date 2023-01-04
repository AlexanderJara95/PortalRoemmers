using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Global;

namespace PortalRoemmers.Areas.Sistemas.Models.Proveedor
{
    public class ProveedorModels
    {
        //id del proveedor
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idPro { get; set; }

        //Cuenta del proveedor AX
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [Display(Name = "Cuenta Proveedor")]
        
        public int cuentaAX { get; set; }

        //Razon Social del proveedor
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        [Index(name: "IX_nomprov")]
        public string nomProv { get; set; }

        //NIIF (RUC) del Proveedor
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(30, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        public string niffPro { get; set; }

        //Grupo (Tipo) de Proveedor
        [Display(Name = "Tipo Proveedor")]
        [StringLength(10)]
        public string idTipPro { get; set; }
        [ForeignKey("idTipPro")]
        public  TipoProveedorModels tipoProveedor { get; set; }

        //Estado del Proveedor
        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

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