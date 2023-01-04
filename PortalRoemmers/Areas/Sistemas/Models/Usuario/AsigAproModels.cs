using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class AsigAproModels
    {
        //aprobador
        [Display(Name = "Aprobador")]
        [StringLength(10)]
        public string idAccApro { get; set; }
        [ForeignKey("idAccApro")]
        public  UsuarioModels aprobador { get; set; }

        //Aginado
        [Display(Name = "Aginado")]
        [StringLength(10)]
        public string idAccAsig { get; set; }
        [ForeignKey("idAccAsig")]
        public  UsuarioModels asignador { get; set; }

        public string aproCrea { get; set; }
        public DateTime? aprofchCrea { get; set; }

        [NotMapped]
        [StringLength(10)]
        public string idAccAsigN { get; set; }

        //Auditoria
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha creación")]
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