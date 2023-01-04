using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Medico;

namespace PortalRoemmers.Areas.Marketing.Models.Actividad
{
    public class DetActiv_MedModels
    {
        [Display(Name = "Actividad")]
        [StringLength(11)]
        public string idActiv { get; set; }
        [ForeignKey("idActiv")]
        public ActividadModels actividad { get; set; }
        //Agregacion Simple - id de los clientes
        [Display(Name = "Médico")]
        [StringLength(10)]
        public string idCli { get; set; }
        [ForeignKey("idCli")]
        public MedicoModels cliente { get; set; }

        //Auditoria
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuCreaDetActMed { get; set; }
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCreaDetActMed { get; set; }
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuModDetActMed { get; set; }
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchModDetActMed { get; set; }

    }
}