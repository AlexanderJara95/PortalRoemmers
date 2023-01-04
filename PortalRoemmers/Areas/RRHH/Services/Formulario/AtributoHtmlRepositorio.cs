using PortalRoemmers.Areas.RRHH.Models.Formulario;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;

namespace PortalRoemmers.Areas.RRHH.Services.Formulario
{
    public class AtributoHtmlRepositorio
    {


        public List<AtributoHTMLModels> obtenerAtributosHtml()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_AtrHtml.OrderBy(x => x.idAtrHtml).ToList();
                return model;
            }
        }

    }
}