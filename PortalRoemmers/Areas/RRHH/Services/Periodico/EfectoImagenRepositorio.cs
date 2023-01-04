using PortalRoemmers.Areas.RRHH.Models.Periodico;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Services.Periodico
{
    public class EfectoImagenRepositorio
    {
        public List<EfectoImagenModels> obtenerEfectosImagen()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_EfeIma.OrderBy(x => x.idEfeIma).ToList();
            return model;
        }
    }
}