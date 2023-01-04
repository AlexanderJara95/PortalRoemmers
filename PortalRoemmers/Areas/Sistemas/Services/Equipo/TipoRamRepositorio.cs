using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Equipo
{
    public class TipoRamRepositorio
    {
        public List<TipoRamModels> obtenerTipoRam()
        {
            var db = new ApplicationDbContext();
            var cg = db.tb_TipRam.OrderBy(x => x.nomTipRam).ToList();
            return cg;
        }
    }
}