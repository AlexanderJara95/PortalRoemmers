using PortalRoemmers.Areas.Sistemas.Models.Menu;
using PortalRoemmers.Models;
using System.Collections.Generic;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Menu
{
    public class TipoIconRepositorio
    {
        public List<TipoIconModels> obtenerTipoIcon()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_TipIcon.OrderBy(x => x.nomTipIco).ToList();
                return model;
            }
        }
    }
}