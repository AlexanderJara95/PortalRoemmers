using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class SangreModels
    {

        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idSan { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomSan { get; set; }

        [Display(Name = "Dar")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string darSan { get; set; }


        [Display(Name = "Recibir")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string recSan { get; set; }


        //Auditoria
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