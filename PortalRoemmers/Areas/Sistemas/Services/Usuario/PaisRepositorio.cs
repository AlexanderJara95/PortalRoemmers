using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Models;
using System.Collections.Generic;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class PaisRepositorio
    {

        public List<PaisModels> obtenerPaises()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Pais.OrderBy(x => x.nomPais).ToList();
            return model;
        }


    }
}