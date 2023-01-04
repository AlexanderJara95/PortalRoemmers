using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Models.Global
{
    public class TipoCambioModels
    {
        //Fecha de creacion
        [Key]
        [Display(Name = "Fecha de TC")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fchTipoCbio { get; set; }
        //Tipo de Cambio de Venta
        [Display(Name = "TC Venta")]
        public float? monTCVenta { get; set; }
        //Tipo de Cambio de Compra
        [Display(Name = "TC Compra")]
        public float? monTCCompra { get; set; }
        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreTC{ get; set; }
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