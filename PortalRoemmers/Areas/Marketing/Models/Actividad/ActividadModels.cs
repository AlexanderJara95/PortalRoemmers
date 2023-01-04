using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Marketing.Models.Estimacion;

namespace PortalRoemmers.Areas.Marketing.Models.Actividad
{
    public class ActividadModels
    {
        //Codigo de la actividad
        [Key]
        [Display(Name = "Código")]
        [StringLength(11)]
        public string idActiv { get; set; }
        //Nombre de la actividad
        [Display(Name = "Nombre")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomActiv { get; set; }
        //Descripcion de la actividad
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [Display(Name = "Descripción")]
        public string desActiv { get; set; }

        //Codigo del usuario responsable
        [Display(Name = "Responsable")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idAccRes { get; set; }
        [ForeignKey("idAccRes")]
        public UsuarioModels responsable { get; set; }

        //Especialidad de la actividad
        [Display(Name = "Especialidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idEsp { get; set; }
        [ForeignKey("idEsp")]
        public EspecialidadModels especialidad { get; set; }
        //Fecha Inicio de la Actividad
        [Display(Name = "Fecha Inicial de la Actividad")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchIniActiv { get; set; }
        //Fecha Fin de la Actividad
        [Display(Name = "Fecha Final de la Actividad")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchFinActiv { get; set; }
        //Fecha Inicio de Vigencia
        [Display(Name = "Fecha Inicial de Vigencia")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchIniVig { get; set; }
        //Fecha Fin de Vigencia
        [Display(Name = "Fecha Final de Vigencia")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchFinVig { get; set; }
        //Estado
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public EstadoModels estado { get; set; }
        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreActiv { get; set; }
        //Fecha de modificacion
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModActiv { get; set; }
        //Usuario creacion
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreActiv { get; set; }
        //Fecha de modificacion
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModActiv { get; set; }

        //Relaciones
        public  EstimacionModels estimacion { get; set; }
        public List<DetActiv_MedModels> dMed { get; set; }
    }
}