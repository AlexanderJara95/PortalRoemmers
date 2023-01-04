using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Medico;

namespace PortalRoemmers.Areas.Ventas.Models.SolicitudGasto
{
    public class DetSolGasto_MedModels
    {
        //Composicion Fuerte - id de la Solicitud Relacionada
        [Display(Name = "Código Sol.")]
        [StringLength(7)]
        public string idSolGas { get; set; }
        [ForeignKey("idSolGas")]
        public  SolicitudGastoModels solicitud { get; set; }
        //Agregacion Simple - id de los clientes
        [Display(Name = "Médico")]
        [StringLength(10)]
        public string idCli { get; set; }
        [ForeignKey("idCli")]
        public  MedicoModels cliente { get; set; }
        //Porcentaje de Participacion del Cliente
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