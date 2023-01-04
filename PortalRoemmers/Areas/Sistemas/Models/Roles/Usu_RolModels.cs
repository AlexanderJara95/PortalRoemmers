using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using System;
using System.ComponentModel.DataAnnotations;

namespace PortalRoemmers.Areas.Sistemas.Models.Roles
{
    public class Usu_RolModels
    {
        [StringLength(10)]
        public string idAcc { get; set; }
        [StringLength(10)]
        public string rolId { get; set; }
        public UsuarioModels accounts { get; set; }
        public RolesModels roles { get; set; }
        public string usuCrea { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
        public string usuMod { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }

    }
}