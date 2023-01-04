using PortalRoemmers.Areas.Almacen.Models.Inventario;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Areas.Almacen.Services.Inventario
{
    public class HistoriaInventarioService
    {
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }

            using (var db = new ApplicationDbContext())
            {

                var model = db.tb_HisInv
                    .OrderBy(x => x.fchConHis).Where(x => x.codProConHis.Contains(search) || x.desProConHis.Contains(search) || x.nroLotConHis.Contains(search) || x.idGruConHis.Contains(search) )
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_HisInv.Where(x => x.codProConHis.Contains(search) || x.desProConHis.Contains(search) || x.nroLotConHis.Contains(search) || x.idGruConHis.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                
                modelo.HistoriaInv = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }

        public Boolean crear(HistoriaInventarioModels model)
        {
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.tb_HisInv.Add(model);
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


        public Boolean eliminarCmd()
        {
            var fchConHis = DateTime.Today;
            string commandText = "delete from tb_HisInv where fchConHis=@fchConHis ";
            Boolean mensaje = false;
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@fchConHis", SqlDbType.DateTime).Value = fchConHis;
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

    }
}