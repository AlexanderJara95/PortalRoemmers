using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Models.DocSustentoRRHH
{
    public class TipoDocSustentoRRHHModels
    {

        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idTipoDocSustento { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomTipDocSustento { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string desTipDocSustento { get; set; }


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


        public List<DocSustentoRRHHModels> documentos { get; set; }

    }
}