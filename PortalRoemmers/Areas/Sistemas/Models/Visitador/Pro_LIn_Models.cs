using PortalRoemmers.Areas.Sistemas.Models.Producto;
using System;
using System.ComponentModel.DataAnnotations;

namespace PortalRoemmers.Areas.Sistemas.Models.Visitador
{
    public class Pro_LIn_Models
    {
        [StringLength(10)]
        public string idLin { get; set; }
        [StringLength(10)]
        public string idProAX { get; set; }

        public ProductoModels producto { get; set; }
        public LineaModels linea { get; set; }

        //auditoria
        public string usuCrea { get; set; }
        public DateTime? usufchCrea { get; set; }

    }
}