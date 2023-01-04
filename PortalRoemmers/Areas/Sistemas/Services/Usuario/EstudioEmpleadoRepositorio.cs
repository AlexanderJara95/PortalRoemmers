using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class EstudioEmpleadoRepositorio
    {
        public Boolean eliminarCmd(string idEmp)
        {
            string commandText = "delete from tb_EstuEmp where idEmp=@idEmp ";
            Boolean mensaje = false;
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idEmp", SqlDbType.VarChar).Value = idEmp;
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
        public bool crear(List<EstudioEmpleadoModels> models)
        {
            bool mensaje = true;
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    db.tb_EstuEmp.AddRange(models);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                    mensaje =false;
                }
            }
            return mensaje;
        }
        public List<EstudioEmpleadoModels> obtenerEstudiosEmpleado(string idEmp)
        {
            var db = new ApplicationDbContext();
            var model = db.tb_EstuEmp.Include(x=>x.nivelEstudio).OrderBy(x => x.idEstu).Where(x=>x.idEmp== idEmp).ToList();
            return model;
        }

    }
}