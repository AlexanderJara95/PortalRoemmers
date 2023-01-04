using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Producto
{
    public class ProductoModels
    {
        [Key]
        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [Display(Name = "Código")]
        public string idProAX { get; set; }

        [Required(ErrorMessage = "Este campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Nombre")]
        public string nomPro { get; set; }

        //uno a muchos
        [Display(Name = "Familia")]
        [StringLength(10)]
        public string idFam { get; set; }
        [ForeignKey("idFam")]
        public  FamProdAxModels familia { get; set; }

        [Display(Name = "Familia Roemmers")]
        [StringLength(10)]
        public string idFamRoe { get; set; }
        [ForeignKey("idFamRoe")]
        public  FamProdRoeModels familiaRoe { get; set; }

        [Display(Name = "Area Terapeutica")]
        [StringLength(10)]
        public string idAreaTerap { get; set; }
        [ForeignKey("idAreaTerap")]
        public AreaTerapeuticaModels areaTerap { get; set; }

        [Display(Name = "Gerente Producto")]
        public string idEmp { get; set; }
        [ForeignKey("idEmp")]
        public  EmpleadoModels usuarioGP { get; set; }

        [Display(Name = "Laboratorio")]
        [StringLength(10)]
        public string idLab { get; set; }
        [ForeignKey("idLab")]
        public  LaboratorioModels laboratorio { get; set; }

        public List<Pro_LIn_Models> proLin { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Producto Seleccionado")]
        public string proAc { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Producto No Seleccionado")]
        public string proIn { get; set; }

        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

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