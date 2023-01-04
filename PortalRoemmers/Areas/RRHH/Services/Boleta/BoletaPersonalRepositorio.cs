using PortalRoemmers.Areas.RRHH.Models.Boleta;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.RRHH.Services.Boleta
{
    public class BoletaPersonalRepositorio
    {
        
        public IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;
            if (pagina == 0)
            {
                pagina = 1;
            }
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_BolPer.Include(x=>x.detalle)
                  .OrderByDescending(x => x.idBolPer).Where(x => x.idBolPer.Contains(search) || x.titBolPer.Contains(search) || x.mesBolPer.Contains(search) || x.anioBolPer.Contains(search))
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_BolPer.Include(x => x.detalle).Where(x => x.idBolPer.Contains(search) || x.titBolPer.Contains(search) || x.mesBolPer.Contains(search) || x.anioBolPer.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.Boleta = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public BoletaPersonalModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            BoletaPersonalModels model = db.tb_BolPer.Include(x=>x.detalle).Where(x=>x.idBolPer==id).FirstOrDefault();
            return model;
        }
        
        public Boolean crear(BoletaPersonalModels model)
        {
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.tb_BolPer.Add(model);
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            return mensaje;
        }
        public Boolean modificar(BoletaPersonalModels model)
        {
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            return mensaje;
        }
        public Boolean eliminar(BoletaPersonalModels model)
        {
            string idBolPer = model.idBolPer;

            string commandText = "delete from tb_BolDet where idBolPer=@idBolPer;";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idBolPer", SqlDbType.VarChar).Value = idBolPer;
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Deleted;
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }

            }
            return mensaje;
        }

        public List<BoletaPersonalModels> obtenerBoletas()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_BolPer.OrderByDescending(x => x.idBolPer).ToList();
                return model;
            }
        }


    }
}