using PortalRoemmers.Areas.Almacen.Models.Inventario;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace PortalRoemmers.Areas.Almacen.Services.Inventario
{
    public class InventarioProductoService
    {
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;

            if(pagina == 0)
            {
                pagina = 1;
            }

            using(var db = new ApplicationDbContext())
            {

                var model = db.tb_InvPro
                    .OrderBy(x => new { x.codProCon,x.nroLotCon,x.ubiProCon }).Where(x => x.usuCrea.ToUpper() == SessionPersister.Username.ToUpper() && (x.nroInvCon.ToString().Contains(search) || x.codProCon.Contains(search) || x.nroLotCon.Contains(search) || x.ubiProCon.Contains(search) || x.canInvCon.ToString().Contains(search) || x.desProCon.Contains(search)))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_InvPro.OrderBy(x => new { x.codProCon, x.nroLotCon, x.ubiProCon }).Where(x => x.usuCrea.ToUpper() == SessionPersister.Username.ToUpper() && (x.nroInvCon.ToString().Contains(search) || x.codProCon.Contains(search) || x.nroLotCon.Contains(search) || x.ubiProCon.Contains(search) || x.canInvCon.ToString().Contains(search) || x.desProCon.Contains(search))).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Conteo = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public InventarioProductoModels obtenerModel(string pro, string lot, string ubi,int con)
        {
            using(var db = new ApplicationDbContext())
            {
                InventarioProductoModels model = db.tb_InvPro.Find(pro, lot, ubi, con);
                return model;
            }
        }
        public Boolean crear(InventarioProductoModels model)
        {
            Boolean mensaje = false;
            using(var db = new ApplicationDbContext())
            {
                db.tb_InvPro.Add(model);
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch(Exception e)
                {
                    e.Message.ToString();
                }
            }
            return mensaje;
        }
        public Boolean modificar(InventarioProductoModels model)
        {
            Boolean mensaje = false;
            using(var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch(Exception e)
                {
                    e.Message.ToString();
                }
            }
            return mensaje;
        }
        public Boolean eliminar(InventarioProductoModels model)
        {
            Boolean mensaje = false;
            using(var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Deleted;
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch(Exception e)
                {
                    e.Message.ToString();
                }

            }
            return mensaje;
        }
        public Boolean eliminarRegistros()
        {
            Boolean mensaje = false;
            string commandText = "delete from tb_InvPro;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    mensaje = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    mensaje = false;
                }
            }
        
            return mensaje;
        }
        public List<QueryConGroupby> obtenerConteoAgrupado()
        {
            using(var db = new ApplicationDbContext())
            {
                var model = db.tb_InvPro.Select(x => new QueryConGroupby { codProCon= x.codProCon, desProCon= x.desProCon, nroLotCon=x.nroLotCon, almInvCon=x.almInvCon }).Distinct().OrderBy(x=>x.codProCon).ToList();
                return model;
            }
        }
        public List<QueryAxGroupby> obtenerConteoAgrupadoAx()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_InvAx
                    .Select(x => new QueryAxGroupby { idProInv = x.idProInv.Trim(), desProInv=x.desProInv.Trim(), nroLotInv = x.nroLotInv.Trim(), idgruInv=x.idgruInv.Trim(), almProInv = x.almProInv.Trim(), canProInv=x.canProInv, ubiProInv = 1})
                    .GroupBy(s => new { s.idProInv, s.desProInv , s.nroLotInv,s.idgruInv, s.almProInv })
                     .Select(g =>
                           new QueryAxGroupby
                           {
                               idProInv = g.Key.idProInv,
                               desProInv = g.Key.desProInv,
                               nroLotInv = g.Key.nroLotInv,
                               idgruInv = g.Key.idgruInv,
                               almProInv = g.Key.almProInv,
                               canProInv = g.Sum(x => x.canProInv),
                               ubiProInv = g.Sum(x => x.ubiProInv)
                           }
                     ).OrderBy(x=>x.idProInv).ToList();
                return model;
            }
        }
        public List<InventarioProductoModels> obtenerConteoDetalle()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_InvPro.OrderBy(x => x.codProCon).ToList();
                return model;
            }
        }
        public List<QueryConGroupby> obtenerConteoEspecifico(string codProConP, string nroLotConP,int nroInvCon,string almInvCon)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_InvPro.Where(x => x.codProCon == codProConP && x.nroLotCon == nroLotConP && x.nroInvCon== nroInvCon && x.almInvCon== almInvCon).Select(x => new QueryConGroupby { codProCon = x.codProCon, nroLotCon = x.nroLotCon, canInvCon= x.canInvCon , almInvCon=x.almInvCon}).OrderBy(x => x.codProCon).ToList();
                return model;
            }
        }
        public class QueryConGroupby
        {
            public string codProCon { get; set; }
            public string desProCon { get; set; }
            public string nroLotCon { get; set; }
            public int canInvCon { get; set; }
            public string almInvCon { get; set; }
        }
        public class QueryAxGroupby
        {
            public string idProInv { get; set; }
            public string desProInv { get; set; }
            public string nroLotInv { get; set; }
            public string idgruInv { get; set; }
            public int canProInv { get; set; }
            public string almProInv { get; set; }
            public int ubiProInv { get; set; }
    }
    }
}