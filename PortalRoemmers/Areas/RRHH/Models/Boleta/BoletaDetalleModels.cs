using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Models.Boleta
{
    [JsonObject(IsReference = true)]
    public class BoletaDetalleModels
    {

        [Display(Name = "Código")]
        [StringLength(10)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string idBolPer { get; set; }
        [ForeignKey("idBolPer")]
        public BoletaPersonalModels Boleta { get; set; }

        [StringLength(15)]
        [Display(Name = "Nro. Documeto")]
        public string nroDocBolDet { get; set; }

        [StringLength(300, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Ruta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string rutBolDet { get; set; }

        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomBolDet { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool estBolDet { get; set; }

        [Display(Name = "Visualizar")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool visBolDet { get; set; }

        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCre { get; set; }
        //Fecha de modificacion
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }
        //Usuario creacion
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuCre { get; set; }
        //Fecha de modificacion
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuMod { get; set; }

        //Fecha de Visualizacion
        [Display(Name = "Usuario Visualización")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuVis{ get; set; }
        //Fecha de modificacion
        [Display(Name = "Fecha Visualización")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchVis { get; set; }

    }
}