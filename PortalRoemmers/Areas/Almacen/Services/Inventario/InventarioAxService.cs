using PortalRoemmers.Areas.Almacen.Models.Inventario;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Almacen.Services.Inventario
{
    public class InventarioAxService
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

                var model = db.tb_InvAx
                    .OrderBy(x => x.desProInv).Where(x => x.idProInv.Contains(search) || x.desProInv.Contains(search) || x.nroLotInv.Contains(search) || x.codBarInv.Contains(search) || x.ubiProInv.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_InvAx.Where(x => x.idProInv.Contains(search) || x.desProInv.Contains(search) || x.nroLotInv.Contains(search) || x.codBarInv.Contains(search) || x.ubiProInv.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Inventario = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }

        public InventarioAxModels obtenerModel(string pro, string lot, string ubi)
        {
            using(var db = new ApplicationDbContext())
            {
                InventarioAxModels model = db.tb_InvAx.Find(pro,lot,ubi);
                return model;
            }
        }
        public Boolean crear(InventarioAxModels model)
        {
            Boolean mensaje = false;
            using(var db = new ApplicationDbContext())
            {
                db.tb_InvAx.Add(model);
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
        public Boolean modificar(InventarioAxModels model)
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
        public Boolean eliminar(InventarioAxModels model)
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

        public List<string> obtenerLotexCBar(string codProCon, string almInvCon)
        {
            using(var db = new ApplicationDbContext())
            {
                var model = db.tb_InvAx.OrderBy(x => x.idProInv).Where(x => x.idProInv == codProCon && x.almProInv == almInvCon).Select(x => x.nroLotInv).Distinct().ToList();
                return model;
            }
        }

        public List<string> obtenerUbixLotyCBar(string codProCon, string nroLotInv, string almInvCon)
        {
            using(var db = new ApplicationDbContext())
            {
                var model = db.tb_InvAx.OrderBy(x => x.idProInv).Where(x => x.idProInv == codProCon && x.nroLotInv== nroLotInv && x.almProInv == almInvCon).Select(x=>x.ubiProInv).Distinct().ToList();
                return model;
            }
        }

        public List<string> obtenerAlmxCBar(string codProCon)
        {
            using(var db = new ApplicationDbContext())
            {
                var model = db.tb_InvAx.OrderBy(x => x.idProInv).Where(x => x.idProInv == codProCon).Select(x => x.almProInv).Distinct().ToList();
                return model;
            }
        }

        public QueryAutoCom obtenerDescripcion(string codBarPro)
        {
            using(var db = new ApplicationDbContext())
            {
                var model = db.tb_InvAx.Where(x => x.codBarInv == codBarPro).Select(x => new QueryAutoCom { value = x.idProInv, text = x.desProInv }).FirstOrDefault();
                if(model==null) { model = new QueryAutoCom { value = "", text = "No Existe" };  }
                return model;
            }
        }

        public List<QueryAutoCom> obtenerDesUnica()
        {
            using(var db = new ApplicationDbContext())
            {
                var model = db.tb_InvAx.Select(x=>new QueryAutoCom { value = x.idProInv, text = x.desProInv }).Distinct().ToList();
                return model;
            }
        }

        public List<QueryAutoCom> obtenerUbiUnica(string codProCon,string nroLotCon)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_InvAx.Where(x=>x.idProInv== codProCon && x.nroLotInv == nroLotCon).Select(x => new QueryAutoCom { value = x.ubiProInv, text = x.ubiProInv }).Distinct().ToList();
                return model;
            }
        }


        public List<QueryInvGroup> obtenerInvAgrupado(string idProInv, string nroLotInv,string almProInv)
        {
            using(var db = new ApplicationDbContext())
            {
                var model = db.tb_InvAx.Select(x => new QueryInvGroup { idProInv=x.idProInv,nroLotInv = x.nroLotInv, idgruInv = x.idgruInv, canProInv =x.canProInv, almProInv=x.almProInv }).Where(x=>x.idProInv == idProInv && x.nroLotInv == nroLotInv && x.almProInv== almProInv).ToList();
                return model;
            }
        }

        public Boolean eliminarRegistros()
        {
            Boolean mensaje = false;
            string commandText = "delete from tb_InvAx;";

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

        public class QueryInvGroup
        {
            public string idProInv { get; set; }
            public string nroLotInv { get; set; }
            public string idgruInv { get; set; }
            public int canProInv { get; set; }

            public string almProInv { get; set; }
        }

        public class QueryAutoCom
        {
            public string value { get; set; }
            public string text { get; set; }

        }
        
    }
}