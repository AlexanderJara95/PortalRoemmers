using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class NivelEstudioRepositorio
    {
        public List<NivelEstudioModels> obtenerListadoNivelEstudio()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_NivelEstu.OrderBy(x => x.nomNivEstu).ToList();
            return model;
        }
    }
}