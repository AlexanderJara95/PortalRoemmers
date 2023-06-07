using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Models.DocSustentoRRHH
{
    public class DocSustentoRRHHModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idDocSustento { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string desDocSustento { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [Display(Name = "Documento Adjunto")]
        public byte[] documentoAdjunto { get; set; }

        [Display(Name = "Tipo")]
        [StringLength(10)]
        public string idTipoDocSustento { get; set; }
        [ForeignKey("idTipoDocSustento")]
        public TipoDocSustentoRRHHModels TipoDocSustento { get; set; }

        [Display(Name = "Usuario creación")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
        [Display(Name = "Usuario modificación")]
        public string usuMod { get; set; }
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }
    }
}