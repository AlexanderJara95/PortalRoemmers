using PortalRoemmers.Areas.Sistemas.Models.Menu;
using PortalRoemmers.Models;
using System.Collections.Generic;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Menu
{
    public class IconRepositorio
    {
        public List<IconModels> obtenerIcon()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Icon.OrderBy(x=>x.nomIco).ToList();
                return model;
            }
        }
    }
}