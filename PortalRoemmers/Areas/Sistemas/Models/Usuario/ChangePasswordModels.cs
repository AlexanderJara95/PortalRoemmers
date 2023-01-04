using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class ChangePasswordModels
    {
        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Contraseña actual")]
        public string userpassA { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Contraseña nueva")]
        public string userpassN { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Repetir contraseña nueva")]
        [Compare("userpassN", ErrorMessage = "La contraseñas no concuerdan")]
        public string userpassNR { get; set; }
    }
}