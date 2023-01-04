using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PortalRoemmers.Areas.RRHH.Models.Bienvenida
{
    public class FotosBienvenidaModels
    {

        [Display(Name = "Código")]
        [StringLength(10)]
        public string idbien { get; set; }
        [ForeignKey("idbien")]
        public  BienvenidaModels bienvenida { get; set; }

        [StringLength(10)]
        [Display(Name = "Código")]
        public string idFotBie { get; set; }

        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Ruta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string rutaFotBie { get; set; }

        [StringLength(800)]
        [Display(Name = "Link")]
        public string linkFotBie { get; set; }

        [Display(Name = "Activo")]
        public Boolean actFotBie { get; set; }

        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCre { get; set; }
        //Fecha de modificacion
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }
        //Usuario creacion
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuCre { get; set; }
        //Fecha de modificacion
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuMod { get; set; }

    }
}