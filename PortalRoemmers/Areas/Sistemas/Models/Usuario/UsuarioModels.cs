using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Areas.Sistemas.Models.Menu;
using PortalRoemmers.Areas.Sistemas.Models.Roles;
using PortalRoemmers.Areas.Sistemas.Models.Trilogia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalRoemmers.Areas.Sistemas.Models.Usuario
{
    public class UsuarioModels
    {
        [Key]
        [Display(Name = "Código")]
        [StringLength(10)]
        public string idAcc { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Correo Roemmers")]
        public string email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Usuario")]
        [Index(IsUnique = true)]
        public string username { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Contraseña")]
        public string userpass { get; set; }

        [Display(Name = "Foto de Perfil")]
        public byte[] rutaImgPer { get; set; }

        [Display(Name = "Correo confirmado")]
        public bool? confirmEmail { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        [Display(Name = "Token Usuario")]
        public string tokenUsu { get; set; }

        //Estado General
        [Display(Name = "Estado")]
        [StringLength(10)]
        public string idEst { get; set; }
        [ForeignKey("idEst")]
        public  EstadoModels estado { get; set; }

        [Display(Name = "Empleado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(10)]
        public string idEmp { get; set; }
        [ForeignKey("idEmp")]
        public  EmpleadoModels empleado { get; set; }

        [Display(Name = "Nivel Aprovación")]
        [StringLength(10)]
        [Required]
        public string idNapro { get; set; }
        [ForeignKey("idNapro")]
        public  NivelAproModels aprobacion { get; set; }

        //auditoria
        [Display(Name = "Fecha creación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchCreUsu { get; set; }

        [Display(Name = "Fecha modificación")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? fchModUsu { get; set; }

        [Display(Name = "Usuario creación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userCreUsu { get; set; }

        [Display(Name = "Usuario modificación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1}  characters.")]
        public string userModUsu { get; set; }

        [Display(Name = "Menu")]
        [StringLength(10)]
        public string idMen { get; set; }
        [ForeignKey("idMen")]
        public  MenuModels menu { get; set; }


        //Relaciones muchos a muchos////////////////////////////////////////////////////////////////////
        public List<Usu_RolModels> accRoles { get; set; }
        public List<Usu_Zon_Lin_Models> UseZonLin { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Confirmar contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("userpass", ErrorMessage = "La contraseñas no concuerdan")]//compara dos atributos
        public string confirmPassword { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Nombre Completo")]
        public string nomComEmp { get; set; }

        [NotMapped]//no lo crea en la base de datos
        [Display(Name = "Usuario Asignado")]
        public string idAccA { get; set; }

    }

}