using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PortalRoemmers.Areas.Sistemas.Models.Producto;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Ventas.Models.SolicitudGasto
{
    public class DetSolGasto_AreaTerapModels
    {
        //Composicion Fuerte - id de la Solicitud Relacionada
        [Display(Name = "Código Sol.")]
        [StringLength(7)]
        public string idSolGas { get; set; }
        [ForeignKey("idSolGas")]
        public SolicitudGastoModels solicitud { get; set; }

        //Agregacion Simple - id del Area Terap Relacionada
        [Display(Name = "Area Terap.")]
        [StringLength(10)]
        public string idAreaTerap { get; set; }
        [ForeignKey("idAreaTerap")]
        public AreaTerapeuticaModels areaTerap { get; set; }

        //Porcentaje de Participacion del Area Terapeutica
        [Display(Name = "Porcentaje")]
        public double valPorcen { get; set; }

        //Importe de afectacion al Area Terapeutica
        [Display(Name = "Importe")]
        public double valor { get; set; }

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