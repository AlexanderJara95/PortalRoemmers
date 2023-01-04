using System.Web;

namespace PortalRoemmers.Security
{
    public class SessionPersister
    {
        //login
        static string userId = "idAcc";
        static string usernameSessionvar = "username";
       // static string userNombre1 = "nom1";
       // static string userNombre2 = "nom2";
       // static string userApepat= "Apepat";
       // static string userApemat = "Apemat";
        static string rutaImgPer = "ImgPer";
        static string userRol = "Rol";
      //  static string userCarg = "Cargo";
       // static string empId = "Empleado";
        static string nivApr = "NivelAprobacion";
       // static string dniEmp = "DNIEMPLEADO";
        //paginacion
        static string pagina = "pagina";
        static string search = "search";
        //otros
        static string activeMenu = "activeMenu";
        static string activeVista = "activeVista";
        static string userMenu = "userMenu";
        static string rutaImgPerMod = "ImgPerMod";
        //sesion primera vez
        static string activeMenuI = "activeMenuI";
        static string activeVistaI = "activeVistaI";
        //Pagina de Inicio y Final
        static string fchEveSolGasI = "fchEveSolGasI";
        static string fchEveSolGasF = "fchEveSolGasF";


        /* public static string DniEmp
         {
             get
             {
                 if (HttpContext.Current == null)
                     return string.Empty;
                 var sessionVar = HttpContext.Current.Session[dniEmp];
                 if (sessionVar != null)
                     return sessionVar as string;
                 return null;
             }
             set
             {
                 HttpContext.Current.Session[dniEmp] = value;
             }
         }
        */

        public static string NivApr
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[nivApr];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[nivApr] = value;
            }
        }

        public static string ActiveMenu
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[activeMenu];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[activeMenu] = value;
            }
        }
        public static string ActiveVista
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[activeVista];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[activeVista] = value;
            }
        }
      /*  public static string EmpId
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[empId];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[empId] = value;
            }
        }*/
        public static string UserMenu
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[userMenu];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[userMenu] = value;
            }
        }
        public static string UserImaMod
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[rutaImgPerMod];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[rutaImgPerMod] = value;
            }
        }
        public static string Search
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[search];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[search] = value;
            }
        }
        public static string Pagina
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[pagina];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[pagina] = value;
            }
        }
        public static string UserId
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[userId];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[userId] = value;
            }
        }
        public static string Username
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[usernameSessionvar];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[usernameSessionvar] = value;
            }
        }
        /*     public static string Usernom1
             {
                 get
                 {
                     if (HttpContext.Current == null)
                         return string.Empty;
                     var sessionVar = HttpContext.Current.Session[userNombre1];
                     if (sessionVar != null)
                         return sessionVar as string;
                     return null;
                 }
                 set
                 {
                     HttpContext.Current.Session[userNombre1] = value;
                 }
             }
             public static string Usernom2
             {
                 get
                 {
                     if (HttpContext.Current == null)
                         return string.Empty;
                     var sessionVar = HttpContext.Current.Session[userNombre2];
                     if (sessionVar != null)
                         return sessionVar as string;
                     return null;
                 }
                 set
                 {
                     HttpContext.Current.Session[userNombre2] = value;
                 }
             }
             public static string Userapepat
             {
                 get
                 {
                     if (HttpContext.Current == null)
                         return string.Empty;
                     var sessionVar = HttpContext.Current.Session[userApepat];
                     if (sessionVar != null)
                         return sessionVar as string;
                     return null;
                 }
                 set
                 {
                     HttpContext.Current.Session[userApepat] = value;
                 }
             }
             public static string Userapemat
             {
                 get
                 {
                     if (HttpContext.Current == null)
                         return string.Empty;
                     var sessionVar = HttpContext.Current.Session[userApemat];
                     if (sessionVar != null)
                         return sessionVar as string;
                     return null;
                 }
                 set
                 {
                     HttpContext.Current.Session[userApemat] = value;
                 }
             }

          public static string UserCarg
             {
                 get
                 {
                     if (HttpContext.Current == null)
                         return string.Empty;
                     var sessionVar = HttpContext.Current.Session[userCarg];
                     if (sessionVar != null)
                         return sessionVar as string;
                     return null;
                 }
                 set
                 {
                     HttpContext.Current.Session[userCarg] = value;
                 }
             }


         */
        public static string UserIma
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[rutaImgPer];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[rutaImgPer] = value;
            }
        }
        public static string UserRol
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[userRol];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[userRol] = value;
            }
        }
       
        public static string ActiveMenuI
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[activeMenuI];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[activeMenuI] = value;
            }
        }
        public static string ActiveVistaI
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[activeVistaI];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[activeVistaI] = value;
            }
        }
        public static string FchEveSolGasI
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[fchEveSolGasI];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[fchEveSolGasI] = value;
            }
        }
        public static string FchEveSolGasF
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionVar = HttpContext.Current.Session[fchEveSolGasF];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[fchEveSolGasF] = value;
            }
        }
    }
}