using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;

namespace PortalRoemmers.Areas.Ventas.Models.SolicitudGasto
{
    public class DetSolGasto_RespModels
    {
        //Composicion Fuerte - id de la Solicitud Relacionada
        [Display(Name = "Código Sol.")]
        [StringLength(7)]
        public string idSolGas { get; set; }
        [ForeignKey("idSolGas")]
        public  SolicitudGastoModels solicitud { get; set; }

        //Agregacion Simple - id del Empleado Relacionado
        [Display(Name = "Responsable")]
        [StringLength(10)]
        public string idEmp { get; set; }
        [ForeignKey("idEmp")]
        public  EmpleadoModels responsable { get; set; }

        //Porcentaje de Participacion del Responsable
        [Display(Name = "Porcentaje")]
        public double valPorcen { get; set; }

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