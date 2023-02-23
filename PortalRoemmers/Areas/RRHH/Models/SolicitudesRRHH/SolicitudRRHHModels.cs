using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Models.Global;

namespace PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH
{
    public class SolicitudRRHHModels
    {
        // Id 1
        [Key]
        [Display(Name = "Código Solicitud")]
        [StringLength(7)]
        public string idSolicitudRrhh { get; set; }

        //Solicitante 3
        [Display(Name = "Solicitante")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idAccSol { get; set; }
        [ForeignKey("idAccSol")]
        public UsuarioModels solicitante { get; set; }

        //Aprobador 4
        [Display(Name = "Aprobador")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idAccApro { get; set; }
        [ForeignKey("idAccApro")]
        public UsuarioModels aprobador { get; set; }

        //Descripción 5
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        public string descSolicitud { get; set; }

        //fecha Inicio 6
        [Display(Name = "Fecha Inicio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchIniSolicitud { get; set; }

        //fecha Fin 7
        [Display(Name = "Fecha Final")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime fchFinSolicitud { get; set; }

        //Ruta Adjunto 8
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Ruta Adjunto")]
        public string rutaArchivoAdjunto { get; set; }

        //estado 9
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idEstado { get; set; }
        [ForeignKey("idEstado")]
        public EstadoModels estado { get; set; }

        //Subtipo
        [Display(Name = "Subtipo")]
        [StringLength(10)]
        public string idSubTipoSolicitudRrhh { get; set; }
        [ForeignKey("idSubTipoSolicitudRrhh")]
        public SubtipoSolicitudRRHHModels subtipoSolicitud { get; set; }

        //Auditóría
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
    }
}