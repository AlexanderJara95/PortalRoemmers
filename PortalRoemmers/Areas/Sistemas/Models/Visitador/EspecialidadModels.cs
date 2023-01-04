using PortalRoemmers.Areas.Sistemas.Models.Medico;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Marketing.Models.Actividad;

namespace PortalRoemmers.Areas.Sistemas.Models.Visitador
{
    public class EspecialidadModels
    {

        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idEsp { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        public string nomEsp { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string desEsp { get; set; }

        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public virtual EstadoModels estado { get; set; }
        //auditoria
        public string usuCrea { get; set; }
        public DateTime? usufchCrea { get; set; }
        public string usuMod { get; set; }
        public DateTime? usufchMod { get; set; }

        public List<MedicoModels> clientes { get; set; }

        public List<ActividadModels> actividades { get; set; }

    }
}