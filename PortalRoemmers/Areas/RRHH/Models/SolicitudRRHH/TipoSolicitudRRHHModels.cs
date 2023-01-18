using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Types.Sql;
using PortalRoemmers.Areas.RRHH.Models.Documento;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH
{
    internal class TipoSolicitudRRHHModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idTipoSolicitudRrhh { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomTipoSolicitud { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string descTipoSolicitud { get; set; }


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

        public List<SubtipoSolicitudRRHHModels> SubtipoSolicitudRRHH { get; set; }

    }
}