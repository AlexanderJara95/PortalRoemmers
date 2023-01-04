using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class SedeRepositorio
    {

        public List<SedeModels> obtenerSedes()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Sede.OrderBy(x => x.nom_sede).ToList();
            return model;
        }

        public SedeModels obtenerSede(string cod_sede)
        {
            SedeModels sede = new SedeModels();

                using (var db = new ApplicationDbContext())
                {
                    sede = db.tb_Sede.Find(cod_sede);
                    return sede;
                }
        }

    }
}