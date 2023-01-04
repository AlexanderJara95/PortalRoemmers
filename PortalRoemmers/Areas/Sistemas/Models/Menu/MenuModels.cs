using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Models.Menu
{
    public class MenuModels
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Código")]
        public string idMen { get; set; }

        [Display(Name = "Título")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string tiMen { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string desMen { get; set; }

        [Display(Name = "Area/Controlador/Vista")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string urlMen { get; set; }

        [Display(Name = "Imagen")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string imgMen { get; set; }

        [Display(Name = "Orden")]
        public int? ordMen { get; set; }

        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Tipo")]
        public string idTipMen { get; set; }
        [ForeignKey("idTipMen")]
        public TipoMenuModels tipMenu { get; set; }

        //recursiva
        [Display(Name = "Menu/SubMenu")]
        [StringLength(10)]
        public string ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual MenuModels Parent { get; set; }

        public virtual ICollection<MenuModels> Childs { get; set; }

        public List<UsuarioModels> usuarios { get; set; }
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