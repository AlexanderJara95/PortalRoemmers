using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalRoemmers.Areas.Sistemas.Models.Gasto;
using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Areas.Marketing.Models.Actividad;

namespace PortalRoemmers.Areas.Marketing.Models.Estimacion
{
    public class DetEstim_FamProdModels
    {
        //Actividad Relacionada
        [Display(Name = "Actividad")]
        [StringLength(11)]
        public string idActiv { get; set; }
        [ForeignKey("idActiv")]
        public  EstimacionModels cabecera { get; set; }

        //Id de la Familia Roe Relacionada
        [Display(Name = "Familia")]
        [StringLength(10)]
        public string idFamRoe { get; set; }
        [ForeignKey("idFamRoe")]
        public  FamProdRoeModels familia { get; set; }

        //Id de la Familia Roe Relacionada
        [Display(Name = "Area Terap.")]
        [StringLength(10)]
        public string idAreaTerap { get; set; }
        [ForeignKey("idAreaTerap")]
        public AreaTerapeuticaModels areaTerap { get; set; }

        //----------------------------Auditoria--------------------------------
        //Fecha de creacion 
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreEstFam { get; set; }
        //Fecha de modificacion
        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModEstFam { get; set; }
        //Usuario creacion
        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreEstFam { get; set; }
        //Fecha de modificacion
        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModEstFam { get; set; }

    }
}