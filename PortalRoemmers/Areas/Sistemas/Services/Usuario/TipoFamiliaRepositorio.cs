using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class TipoFamiliaRepositorio
    {
        public List<TipoFamiliaModels> obtenerListadoTipoFamilia()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_TipFam.OrderBy(x => x.nomTipFa).ToList();
            return model;
        }
    }
}