using PortalRoemmers.Areas.Almacen.Models.Inventario;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace PortalRoemmers.Areas.Almacen.Services.Inventario
{
    public class NumeroConteoService
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

                var model = db.tb_NumCon
                    .Include(x=>x.estado)
                    .OrderBy(x => x.codCon).Where(x => (x.desCon.ToString().Contains(search) || x.usuCrea.Contains(search) || x.estado.desEst.Contains(search) ))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_NumCon.Where(x => (x.desCon.ToString().Contains(search) || x.usuCrea.Contains(search) || x.estado.desEst.Contains(search))).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.ConteoR = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public NumeroConteoModels obtenerModel(int codCon)
        {
            using(var db = new ApplicationDbContext())
            {
                NumeroConteoModels model = db.tb_NumCon.Find(codCon);
                return model;
            }
        }
        public Boolean crear(NumeroConteoModels model)
        {
            Boolean mensaje = false;
            using(var db = new ApplicationDbContext())
            {
                db.tb_NumCon.Add(model);
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
        public Boolean modificar(NumeroConteoModels model)
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
        public Boolean eliminar(NumeroConteoModels model)
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
        
        /*public Boolean sqlUpdateCont(int cod)
        {
            string commandText = " update tb_NumCon set idEst=@idEstA; update tb_NumCon set idEst=@idEst where codCon=@codCon;";
            string commandText = " update tb_NumCon set idEst=@idEstA; update tb_NumCon set idEst=@idEst where codCon=@codCon;";
            using(SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@idEst", ConstantesGlobales.estadoActivo);
                command.Parameters.AddWithValue("@codCon", cod);
                command.Parameters.AddWithValue("@idEstA", ConstantesGlobales.estadoInactivo);

                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }*/

        public Boolean sqlUpdateCont(int cod, string est)
        {
            //string commandText = " update tb_NumCon set idEst=@idEstA; update tb_NumCon set idEst=@idEst where codCon=@codCon;";
            string commandText = "update tb_NumCon set idEst=@idEst where codCon=@codCon;";
            string estado = ConstantesGlobales.estadoActivo;
            if (est == "ACTIVO")
            {
                estado = ConstantesGlobales.estadoInactivo;
            }
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@idEst", estado);
                command.Parameters.AddWithValue("@codCon", cod);
                //command.Parameters.AddWithValue("@idEstA", ConstantesGlobales.estadoInactivo);

                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public List<NumeroConteoModels> obtenerConteoActivo()
        {
            using(var db = new ApplicationDbContext())
            {
                var model = db.tb_NumCon.Where(x => x.idEst == ConstantesGlobales.estadoActivo).ToList();
                return model;
            }
        }

        public List<NumeroConteoModels> obtenerConteos()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_NumCon.OrderBy(x=>x.codCon).Where(x=>x.idEst==ConstantesGlobales.estadoActivo).ToList();
                return model;
            }
        }
    }
}