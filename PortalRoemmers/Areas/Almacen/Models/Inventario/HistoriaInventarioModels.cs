using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Almacen.Models.Inventario
{
    public class HistoriaInventarioModels
    {
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime fchConHis { get; set; }

        [Display(Name = "Código")]
        [Index("IX_ConteoProducto", 2)]
        [StringLength(15, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string codProConHis { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string desProConHis { get; set; }

        [Display(Name = "Nro. Lote")]
        [Index("IX_ConteoProducto", 3)]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nroLotConHis { get; set; }

        [Display(Name = "Grupo")]
        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string idGruConHis { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int canInvConHis { get; set; }

        [Display(Name = "Ultimo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ultCanConHis { get; set; }

        [Display(Name = "Resultado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int resCanConHis { get; set; }

        [Display(Name = "Observación")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string obsInvConHis { get; set; }

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