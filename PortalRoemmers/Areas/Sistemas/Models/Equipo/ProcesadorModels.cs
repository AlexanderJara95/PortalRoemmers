using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PortalRoemmers.Areas.Sistemas.Models.Equipo
{
    public class ProcesadorModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idProce { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string nomProce { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string descProce { get; set; }

        //VelocidadCpu
        [StringLength(10, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Velocidad CPU")]
        public string velCpuProce { get; set; }

        //Nro Nucleo
        [Display(Name = "Número de Nucleo")]
        public int nroNucProce { get; set; }

        //Nro Nucleo logicos
        [Display(Name = "Número de Nucleo Logico")]
        public int nroNucLogProce { get; set; }

        public List<EquipoModels> equipos { get; set; }

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