using PortalRoemmers.Areas.RRHH.Models.Bienvenida;
using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace PortalRoemmers.Areas.RRHH.Services
{
    public class BienvenidaRepositorio
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
                var model = db.tb_Bienvenida
                .Include(x=>x.fotos)
                .OrderBy(x => x.idbien)
                .Where(x => x.titbien.Contains(search) || x.desbien.Contains(search))
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.tb_Bienvenida.Where(x => x.titbien.Contains(search) || x.desbien.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Bienvenida = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public Boolean crear(BienvenidaModels model)
        {
            Boolean mensaje = false;
            //creo su ID
            using (var db = new ApplicationDbContext())
            {
                db.tb_Bienvenida.Add(model);
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
        public BienvenidaModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                BienvenidaModels model = db.tb_Bienvenida.Include(x=>x.fotos).Where(x => x.idbien == id).FirstOrDefault();
                return model;
            }

        }
        public Boolean modificar(BienvenidaModels model)
        {
            Boolean ok = false;
            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    ok = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            return ok;
        }
        public Boolean eliminar(string id)
        {
            string commandText = "delete from tb_BienFotos where idbien=@idbien ; delete from tb_Bienvenida where idbien=@idbien";
            Boolean mensaje = false;
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idbien", SqlDbType.VarChar).Value = id;
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
        public Boolean crearDetalle(List<FotosBienvenidaModels> model)
        {
            Boolean mensaje = false;
            //creo su ID
            using (var db = new ApplicationDbContext())
            {
                db.tb_BienFotos.AddRange(model);
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
        public Boolean eliminarDetalle(string id)
        {
            Boolean mensaje = false;

            string commandText = "delete from tb_BienFotos WHERE idbien = @idbien";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idbien", SqlDbType.VarChar).Value = id;
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
        public Boolean actualizarBienvenida(string id)
        {
            string commandText = "update  tb_Bienvenida set actbien= 0  where idbien<>@idbien ; update tb_Bienvenida set actbien=1  where idbien=@idbien";
            Boolean mensaje = false;
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idbien", SqlDbType.VarChar).Value = id;
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
        public BienvenidaModels obtenerBienvenida()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Bienvenida.Include(x => x.fotos).OrderBy(x => x.idbien).Where(x => x.actbien == true).FirstOrDefault();
            return model;
        }
        public List<FotosBienvenidaModels>  obtenerFotosBien(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_BienFotos.Include(x => x.bienvenida).Where(x=>x.idbien==id).OrderBy(x => x.idFotBie).ToList();
                return model;
            }
        }
        public Boolean actualizarFotoBienvenida(string idBien, string idFotoBien, string link)
        {
            string commandText = "update tb_BienFotos set linkFotBie =@link where idbien=@idbie and idFotBie=@idFotBie";
            Boolean mensaje = false;
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@link", link);
                command.Parameters.AddWithValue("@idbie", idBien);
                command.Parameters.AddWithValue("@idFotBie", idFotoBien);
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