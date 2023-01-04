using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Gasto
{
    public class TipGas_Usu_Models
    {
        [StringLength(10)]
        public string idAcc { get; set; }
        [StringLength(10)]
        public string idTipGas { get; set; }
        public UsuarioModels cuentaDeUsuario { get; set; }
        public TipoGastoModels tiposDeGastos { get; set; }
        //auditoria
        public string usuCrea { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
        public string usuMod { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }

        [NotMapped]
        [StringLength(10)]
        public string idAccN { get; set; }
    }
}