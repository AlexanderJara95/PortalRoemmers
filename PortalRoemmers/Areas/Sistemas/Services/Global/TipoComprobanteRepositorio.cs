using PortalRoemmers.Areas.Sistemas.Models.Global;
using PortalRoemmers.Models;
using System.Collections.Generic;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Global
{
    public class TipoComprobanteRepositorio
    {
        public List<TipoComprobanteModels> obtenerTComprobante()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_TipComp.Where(x => x.idEst == ConstantesGlobales.estadoActivo).OrderBy(x => x.nomTipComp).ToList();
            return model;
        }
    }
}