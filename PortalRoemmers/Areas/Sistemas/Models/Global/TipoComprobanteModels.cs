using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Global
{
    public class TipoComprobanteModels
    {
        //Id del Comprobante
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idCodComp { get; set; }

        //Nombre del Comprobante
        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomTipComp { get; set; }

        //Descripcion del Comprobante
        [Display(Name = "Descripción")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string dscTipComp { get; set; }

        //Codigo Sunat
        [Display(Name = "Codigo Sunat")]
        [StringLength(2, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string codSunat { get; set; }

        //Cantidad de Serie
        [Display(Name = "Serie")]
        public int cantSerie { get; set; }

        //Cantidad Numero
        [Display(Name = "Correlativo")]
        public int cantNumero { get; set; }

        //Estado
        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }
        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreTC { get; set; }

        //Fecha de modificacion
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModTC { get; set; }

        //Usuario creacion
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreTC { get; set; }

        //Fecha de modificacion
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModTC { get; set; }
    }
}