using PortalRoemmers.Areas.Sistemas.Models;
using PortalRoemmers.Models;
using System.Linq;
using System.Security.Principal;

namespace PortalRoemmers.Security
{
    public class CustomPrincipal : IPrincipal
    {
 
        string account = SessionPersister.Username;

        public CustomPrincipal()
        {
            this.Identity = new GenericIdentity(account);
        }

        public IIdentity Identity { get; set; }


        public bool IsInRole(string role)
        {
            //RETORNO TRU PARA TODOS
            if (role == ConstantesGlobales.rolTodos)
            {
                return true;
            }
        
          var roles = role.Split(new char[] { ',' });
          var t=SessionPersister.UserRol.Split(new char[] { ',' });

          return roles.Any(r => t.ToArray().Contains(r));
        }
    }
}