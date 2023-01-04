using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Equipo
{
    public class TipoDiscoRepositorio
    {

        public List<TipoDiscoModels> obtenerTipoDisco()
        {
            var db = new ApplicationDbContext();
            var cg = db.tb_TipDis.OrderBy(x => x.nomTipDis).ToList();
            return cg;
        }
    }
}