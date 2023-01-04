using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Models.Formulario
{
    public class Form_Usu_Models
    {
        [StringLength(10)]
        public string idAcc { get; set; }
        [StringLength(10)]
        public string idFor { get; set; }

        [ForeignKey("idAcc")]
        public UsuarioModels accounts { get; set; }
        [ForeignKey("idFor")]
        public FormularioModels formularios { get; set; }

        [Display(Name = "Completo")]
        public bool comForm { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Opcion")]
        public Boolean option { get; set; }

        public string usuCrea { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
        public string usuMod { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }


    }
}