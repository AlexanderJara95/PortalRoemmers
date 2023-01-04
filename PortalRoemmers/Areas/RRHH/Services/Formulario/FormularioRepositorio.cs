using PortalRoemmers.Areas.RRHH.Models.Formulario;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;


namespace PortalRoemmers.Areas.RRHH.Services.Formulario
{
    public class FormularioRepositorio
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

                var model = db.tb_Formulario
                    .Include(x=>x.estado)
                    .OrderBy(x => x.idEst).Where(x => x.idFor.Contains(search) || x.nomFor.Contains(search) || x.estado.nomEst.Contains(search) || x.fchIniFor.ToString().Contains(search) || x.fchFinFor.ToString().Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Formulario.Include(x => x.estado).Where(x => x.idFor.Contains(search) || x.nomFor.Contains(search) || x.estado.nomEst.Contains(search) || x.fchIniFor.ToString().Contains(search) || x.fchFinFor.ToString().Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.Formulario = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public FormularioModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            FormularioModels model = db.tb_Formulario.Find(id);
            return model;
        }
        public List<int> confFormulario()
        {
            //numero de campos
            List<int> values = new List<int>() {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16};

            return values;
        }
        public Boolean guardarDatosFormulario(FormCollection formCollection)
        {

            string nomTB = "tb_Form0001";

            string camposCol = "";
            string camposVal = "";
            foreach (int i in confFormulario())
            {
                camposCol += "item" + i +",";
                camposVal += "@item" + i + ",";
            }
            camposCol= camposCol.Substring(0, camposCol.Length - 1);
            camposVal = camposVal.Substring(0 , camposVal.Length - 1);
            string commandText = "INSERT INTO " + nomTB + "( idAcc, idDia,usuCrea,usufchCrea," + camposCol + ")" + "VALUES ( @idAcc,@idDia,@usuCrea,@usufchCrea, " + camposVal+");";


            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@idAcc", SessionPersister.UserId);
                command.Parameters.AddWithValue("@idDia", DateTime.Today);
                command.Parameters.AddWithValue("@usuCrea", SessionPersister.Username);
                command.Parameters.AddWithValue("@usufchCrea", DateTime.Now);
                foreach (int i in confFormulario())
                {
                    command.Parameters.AddWithValue("@item"+ i, formCollection["item" + i]);
                }
                
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }

            }
            return true;
        }
        public List<FormularioModels> obtenerFormularios()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Formulario.OrderBy(x => x.idFor).ToList();
                return model;
            }
        }
        public Boolean activarFicha(string idFor, DateTime fchIniFor,DateTime fchFinFor)
        {
            string commandText = " update " + ConstantesGlobales.TBFORMU +
                " set fchIniFor=@fchIniFor,fchFinFor=@fchFinFor, usuMod=@usuMod, usufchMod=@usufchMod" +
                " where idFor=@idFor;" +
                " update " + ConstantesGlobales.TBFORUSU +
                " set comForm=@comForm, usuMod=@usuMod, usufchMod=@usufchMod" +
                " where idFor=@idFor;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@idFor", idFor);
                command.Parameters.AddWithValue("@fchIniFor", fchIniFor);
                command.Parameters.AddWithValue("@fchFinFor", fchFinFor);   
                command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                command.Parameters.AddWithValue("@usufchMod", DateTime.Now);
                command.Parameters.AddWithValue("@comForm", false);
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

      
    }
}