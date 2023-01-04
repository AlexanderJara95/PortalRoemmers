
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Almacen.Models.Inventario
{
    public class InventarioProductoModels
    {
        [Display(Name = "Código")]
        [Index("IX_ConteoProducto", 2)]
        [StringLength(15, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string codProCon { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string desProCon { get; set; }

        [Display(Name = "Nro. Lote")]
        [Index("IX_ConteoProducto", 3)]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nroLotCon { get; set; }

        [Display(Name = "Ubicación")]
        [Index("IX_ConteoProducto", 4)]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string ubiProCon { get; set; }

        [Index("IX_ConteoProducto", 1)]
        [Display(Name = "Conteo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int nroInvCon { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int canInvCon { get; set; }

        [Display(Name = "Observación")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string obsInvCon { get; set; }

        [Display(Name = "Fabricante")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string fabInvCon { get; set; }

        [Display(Name = "Almacen")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string almInvCon { get; set; }

        [Display(Name = "Código Barra")]
        [StringLength(15, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [NotMapped]
        public string codBarCon { get; set; }

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