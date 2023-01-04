using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Models;
using System.Collections.Generic;
using System.Linq;


namespace PortalRoemmers.Areas.Sistemas.Services.Gasto
{
    public class ByteRepositorio
    { 
    
        public List<ByteModels> obtenerBytes()
        {
            var db = new ApplicationDbContext();
            var cg = db.tb_Byte.OrderBy(x => x.idByt).ToList();
            return cg;
        }

    }
}