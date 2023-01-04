﻿using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class SangreRepositorio
    {
        public List<SangreModels> obtenerSangre()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_San.OrderBy(x => x.nomSan).ToList();
            return model;
        }
    }
}