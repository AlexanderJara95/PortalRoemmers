using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Contabilidad.Models.Letra;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Models.Global;

namespace PortalRoemmers.Areas.Contabilidad.Models.Letra
{
    public class FirmasLetraModels
    {
        [Display(Name = "ID Letra")]
        public string idLetra { get; set; }
        [ForeignKey("idLetra")]
        public LetraModels letra { get; set; }

        [Display(Name = "Usuario")]
        [StringLength(10)]
        public string idAcc { get; set; }
        [ForeignKey("idAcc")]
        public UsuarioModels solicitante { get; set; }

        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public EstadoModels estado { get; set; }

        //Observacion
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Observacíon")]
        public string obsFirLet { get; set; }

        //auditoria
        [Display(Name = "Usuario Creacion")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha Creacion")]
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