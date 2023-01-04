using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class BancoRepositorio
    {
        public List<BancoModels> obtenerBancos()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Banco.OrderBy(x => x.nomBan).ToList();
            return model;
        }
    }
}