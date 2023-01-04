using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Roles
{
    public class RolesModels
    {

        [Key]
        [StringLength(10)]
        [Display(Name = "Código")]
        public string rolId { get; set; }

        [Display(Name = "Título")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string roltitu { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string rolDes { get; set; }

        [StringLength(2)]
        [Display(Name = "Tipo")]
        public string rolTip { get; set; }
        [ForeignKey("rolTip")]
        public TipoRolModels TipRol{ get; set; }

        //recursiva
        [Display(Name = "Area/Controlador")]
        [StringLength(10)]
        public string ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual RolesModels Parent { get; set; }
        public virtual ICollection<RolesModels> Childs { get; set; }

        public List<Usu_RolModels> accRoles { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Opcion")]
        public Boolean option { get; set; }


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

