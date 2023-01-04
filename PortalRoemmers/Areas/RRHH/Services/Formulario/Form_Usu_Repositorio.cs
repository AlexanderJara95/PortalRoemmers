using PortalRoemmers.Areas.RRHH.Models.Formulario;
using PortalRoemmers.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data.SqlClient;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using System.Web.Mvc;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PortalRoemmers.Areas.RRHH.Services.Formulario
{
    public class Form_Usu_Repositorio
    {
        public List<int> confFormulario()
        {
            //numero de campos
            List<int> values = new List<int>() {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16 };

            return values;
        }
        public List<Form_Usu_Models> obtenerFormulariosxUsuario(string idAcc)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_For_Usu.OrderBy(x => x.idFor).Include(x => x.formularios).Where(x => x.idAcc == idAcc).ToList();
                return model;
            }
        }
        public Boolean sqlUpdateEstado(string idAcc,string idFor)
        {
            
            string commandText = " update "+ ConstantesGlobales.TBFORUSU +
                " set comForm=@comForm, usuMod=@usuMod, usufchMod=@usufchMod" +
                " where idAcc=@idAcc and idFor=@idFor;";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@idAcc", idAcc);
                command.Parameters.AddWithValue("@idFor", idFor);
                command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                command.Parameters.AddWithValue("@usufchMod", DateTime.Now);
                command.Parameters.AddWithValue("@comForm", true);
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
        public List<Form_Usu_Models> obtenerFormxUsu(string idFor, string Value)
        {
            Boolean v = Convert.ToBoolean(Value);
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_For_Usu.OrderBy(x => x.idFor).Include(x => x.formularios).Include(x => x.accounts).Include(x => x.accounts.empleado).Where(x => x.idFor == idFor && x.comForm == v).ToList();
                return model;
            }
        }
        public List<Form_Usu_Models> obtenerUsuariosxFormularios(string idFor)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_For_Usu.Include(x => x.accounts).Include(x => x.accounts.empleado).Where(x => x.idFor == idFor).ToList();
                return model;
            }
        }
        public List<UsuarioModels> obtenerNoUsuariosxFormularios(string idFor)
        {
            using (var db = new ApplicationDbContext())
            {
                var tbForUsu = from fu in db.tb_For_Usu
                               where fu.idFor == idFor
                               select fu;

                var innerGroupJoinQuery =  from u in db.tb_Usuario
                                           join fu in tbForUsu on u.idAcc equals fu.idAcc into ufu
                                           from subpet in ufu.DefaultIfEmpty()
                                           where subpet.idAcc == null && u.idEst==ConstantesGlobales.estadoActivo 
                                           select u ;

                return innerGroupJoinQuery.Include(x=>x.empleado).ToList();
            }
        }
        public Boolean agregarUsuariosFormulario(string[] idAcc, string id)
        {
            Boolean resultado=true;
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {

                if (idAcc != null)
                {
                    foreach (var a in idAcc)
                    {
                        string commandText = "INSERT INTO " + ConstantesGlobales.TBFORUSU +
                            " (idAcc,idFor,comForm,usuCrea,usufchCrea)" +
                            " VALUES (@idAcc,@idFor,@comForm,@usuCrea,@usufchCrea);";
                        SqlCommand command = new SqlCommand(commandText, connection);
                        command.Parameters.AddWithValue("@idAcc", a);
                        command.Parameters.AddWithValue("@idFor", id);
                        command.Parameters.AddWithValue("@comForm", false);
                        command.Parameters.AddWithValue("@usuCrea", SessionPersister.Username);
                        command.Parameters.AddWithValue("@usufchCrea", DateTime.Now);
                        try
                        {
                            connection.Open();
                            Int32 rowsAffected = command.ExecuteNonQuery();
                            connection.Close();
                         
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            resultado = false;
                        }
                    }
                }
            }
            return resultado;
        }
        public Boolean eliminarUsuariosFormulario(string[] idAcc, string id)
        {
            Boolean resultado = true;
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {

                if (idAcc != null)
                {
                    foreach (var a in idAcc)
                    {
                        string commandText = "DELETE FROM " + ConstantesGlobales.TBFORUSU +
                            " WHERE  idAcc=@idAcc and idFor=@idFor;";
                          
                        SqlCommand command = new SqlCommand(commandText, connection);
                        command.Parameters.AddWithValue("@idAcc", a);
                        command.Parameters.AddWithValue("@idFor", id);
                        try
                        {
                            connection.Open();
                            Int32 rowsAffected = command.ExecuteNonQuery();
                            connection.Close();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            resultado= false;
                        }
                    }
                }
            }
            return resultado;
        }
        public List<SelectListItem> listarFechaFormulario(string idFor)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                connection.Open();
                string sql = "select distinct(idDia) from tb_Form" +idFor+ " order by idDia desc;";
                using (SqlCommand command = new SqlCommand(sql,connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item.Value = reader.GetValue(0).ToString();
                            item.Text = reader.GetValue(0).ToString();
                            lst.Add(item);
                            item = new SelectListItem();
                        }
                    }
                }
            }
            return lst;
        }
        public string listarDatosFormularioLlenado(string idFor, DateTime idDia)
        {
            string ruta = "";
            string path = "~/Export/Formulario";
            bool exists = Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(path));
            if (!exists) Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(path));

            string nomTB = "tb_Form" + idFor;

            string camposCol = "";
            foreach (int i in confFormulario())
            {
                camposCol += "item" + i + ",";
            }
            camposCol = camposCol.Substring(0, camposCol.Length - 1);

            string commandText = "select " + camposCol + " from " + nomTB + " where idDia=@idDia;";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {

                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText,
                connection))
                {
                    command.Parameters.AddWithValue("@idDia", idDia);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        using (SLDocument sl = new SLDocument())
                        {
                            int fil = 1;
                            int col = 0;
                            SLFont font;
                            SLRstType rst;
                            SLStyle style;

                            foreach (int i in confFormulario())
                            {
                                col = i;
                                font = new SLFont();
                                font.Bold = true;//negrita
                                font.SetFont(FontSchemeValues.Minor, 14);
                                rst = new SLRstType();
                                rst.AppendText("CAMPO " + i, font);
                                sl.SetCellValue(fil, col, rst.ToInlineString());
                                sl.SetColumnWidth(col, 14);
                            }
                            fil += 1;
                            path = path + "/Formulario" + idFor + ".xls";
                            while (reader.Read())
                            {
                                foreach (int i in confFormulario())
                                {
                                    col = i;
                                    sl.SetCellValue(fil, col, reader.GetValue(i - 1).ToString());
                                }
                                fil += 1;
                            }
                            ruta = System.Web.HttpContext.Current.Server.MapPath(path);
                            sl.SaveAs(ruta);
                        }
                    }
                }
            }

            return ruta;
        }

    }
}