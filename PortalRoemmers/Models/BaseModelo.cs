using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Models
{
    public class BaseModelo
    {
        public int PaginaActual { get; set; }
        public int TotalDeRegistros { get; set; }
        public int RegistrosPorPagina { get; set; }
    }
}