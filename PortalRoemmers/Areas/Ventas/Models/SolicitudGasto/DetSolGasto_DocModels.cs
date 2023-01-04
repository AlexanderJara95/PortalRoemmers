
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Proveedor;
using PortalRoemmers.Areas.Sistemas.Models.Global;
using System;

namespace PortalRoemmers.Areas.Ventas.Models.SolicitudGasto
{
    public class DetSolGasto_DocModels
    {
        //Composicion Fuerte - id de la Solicitud Relacionada
        [Display(Name = "Código Sol.")]
        [StringLength(7)]
        public string idSolGas { get; set; }
        [ForeignKey("idSolGas")]
        public  SolicitudGastoModels solicitud { get; set; }

        //Agregacion Simple - id de la Detalle de Doc
        [Display(Name = "Id")]
        [StringLength(10)]
        public string idDetDoc { get; set; }

        //Proveedor
        [Display(Name = "Proveedor")]
        [StringLength(10)]
        public string idPro { get; set; }
        [ForeignKey("idPro")]
        public  ProveedorModels proveedor { get; set; }

        //Tipo de Comprobante
        [Display(Name = "Documento")]
        [StringLength(10)]
        public string idCodComp { get; set; }
        [ForeignKey("idCodComp")]
        public  TipoComprobanteModels documento { get; set; }

        //Serie
        [Display(Name = "Serie")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string numSerie { get; set; }

        //Numero
        [Display(Name = "Correlativo")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string numCorrelativo { get; set; }

        //Auditoria
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuMod { get; set; }
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchMod { get; set; }

        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string usuCrea { get; set; }
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? usufchCrea { get; set; }
    
    }
}