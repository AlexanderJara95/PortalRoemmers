using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Ventas.Models.SolicitudGasto
{
    public class FirmasSoliGastoModels
    {
        [Display(Name = "Solicitud Gasto")]
        public string idSolGas { get; set; }
        [ForeignKey("idSolGas")]
        public  SolicitudGastoModels gasto { get; set; }
    
        [Display(Name = "Usuario")]
        [StringLength(10)]
        public string idAcc { get; set; }
        [ForeignKey("idAcc")]
        public  UsuarioModels solicitante { get; set; }

        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

        //nivel de aprobacion
        [Display(Name = "Nivel Aprobación")]
        [StringLength(10)]
        public string idNapro { get; set; }
        [ForeignKey("idNapro")]
        public  NivelAproModels nivelA { get; set; }

        //Observacion
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Observacíon")]
        public string obsFirSol { get; set; }

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