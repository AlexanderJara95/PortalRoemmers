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
    public class BoletaPersonalModels
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Código")]
        public string idBolPer { get; set; }

        [Display(Name = "Título")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string titBolPer { get; set; }

        [StringLength(300, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Ruta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string rutBolPer { get; set; }

        [Display(Name = "Mes")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string mesBolPer { get; set; }

        [Display(Name = "Año")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string anioBolPer { get; set; }
        
        [Display(Name = "Boletas")]
        public List<BoletaDetalleModels> detalle { get; set; }

        [NotMapped]
        public string rutBolPerMod { get; set; }

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


    }
}