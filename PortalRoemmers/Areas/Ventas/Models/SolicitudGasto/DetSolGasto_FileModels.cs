using System;
using System.ComponentModel.DataAnnotations;
using PortalRoemmers.Areas.Sistemas.Models.Producto;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace PortalRoemmers.Areas.Ventas.Models.SolicitudGasto
{
    public class DetSolGasto_FileModels
    {

        //Composicion Fuerte - id de la Solicitud Relacionada
        [Display(Name = "Código Sol.")]
        [StringLength(7)]
        public string idSolGas { get; set; }
        [ForeignKey("idSolGas")]
        public  SolicitudGastoModels solicitud { get; set; }

        //Codigo del file
        [Key]
        [Display(Name = "Código File")]
        public int idFile { get; set; }

        //Nombre del file
        [Display(Name = "Nombre File")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomFile { get; set; }

        //Path del file
        [Display(Name = "Path File")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string pathFile { get; set; }

        //Archivo clase para el almacenar el file
        [NotMapped]
        [Display(Name = "Image File")]
        public HttpPostedFileBase ImageFile { get; set; }

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