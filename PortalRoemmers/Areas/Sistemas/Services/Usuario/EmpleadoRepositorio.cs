using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Models;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using PortalRoemmers.Helpers;
using System.Data.SqlClient;
using System.Data;
using PortalRoemmers.Security;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class EmpleadoRepositorio
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

                var empleado = db.tb_Empleado.Include(x=>x.estado).Include(x => x.cargo).Include(y => y.area).OrderBy(x => new { x.apePatEmp, x.apeMatEmp }).Where(z => z.nom1Emp.Contains(search) || z.nom2Emp.Contains(search) || z.apePatEmp.Contains(search) || z.apeMatEmp.Contains(search) || z.nroDocEmp.Contains(search) || z.cargo.desCarg.Contains(search) || z.area.desAreRoe.Contains(search) || z.estado.nomEst.Contains(search) || z.nomComEmp.Contains(search))
                                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Empleado.Include(x => x.estado).Include(x => x.cargo).Include(y => y.area).Where(z => z.nom1Emp.Contains(search) || z.nom2Emp.Contains(search) || z.apePatEmp.Contains(search) || z.apeMatEmp.Contains(search)  || z.nroDocEmp.Contains(search) || z.cargo.desCarg.Contains(search) || z.area.desAreRoe.Contains(search) || z.estado.nomEst.Contains(search) || z.nomComEmp.Contains(search)).Count();

                var modelo = new IndexViewModel();
                modelo.Empleado = empleado;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public EmpleadoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            EmpleadoModels model = db.tb_Empleado.Include(x=>x.jefe).Where(x=>x.idEmp==id).FirstOrDefault();
            return model;
        }

        public EmpleadoModels obtenerItemXcargo(string id)
        {
            var db = new ApplicationDbContext();
            EmpleadoModels model = db.tb_Empleado.Include(x => x.jefe).Where(x => x.idCarg == id && x.idEst == ConstantesGlobales.estadoActivo).FirstOrDefault();
            return model;
        }

        public EmpleadoModels obtenerItemDetalle(string id)
        {
            EmpleadoModels model=new EmpleadoModels();
            using (var db = new ApplicationDbContext())
            {
                model = db.tb_Empleado
                    .Include(x=>x.tipDoc)
                    .Include(x => x.estCiv)
                    .Include(x => x.gene)
                    .Include(x => x.ubicacion)
                    .Include(x => x.area)
                    .Include(x => x.cargo)
                    .Include(x => x.jefe)
                    .Include(x => x.estado)
                    .Include(x => x.afp)
                    .Where(x=>x.idEmp== id).FirstOrDefault();
            }
            return model;
        }

        public string crear(EmpleadoModels model)
        {
            //creo su ID
            string mensaje = "";
            

            var db = new ApplicationDbContext();
            db.tb_Empleado.Add(model);
            try
            {
                db.SaveChanges();
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(EmpleadoModels model)
        {
            string mensaje = "";
            model.fchModUsu = DateTime.Now;
            model.userModUsu = SessionPersister.Username;
            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    mensaje = "<div id='success' class='alert alert-success'>Se modificó el registro.</div>";
                }
                catch (Exception e)
                {
                    mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
                }
            }
            return mensaje;
        }
        public string eliminar(string id)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            EmpleadoModels model = db.tb_Empleado.Find(id);
            db.tb_Empleado.Remove(model);
            try
            {
                db.SaveChanges();
                mensaje = "<div id='success' class='alert alert-success'>Se eliminó un registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public Boolean cesadoEmp(string idEmp)
        {
            string commandText = "UPDATE tb_Empleado SET idEst = @estUsu, cesfchEmp = @cesfchEmp, fchModUsu = @fchModUsu, userModUsu = @userModUsu  WHERE idEmp = @idEmp;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idEmp", SqlDbType.NVarChar);
                command.Parameters["@idEmp"].Value = idEmp;
                command.Parameters.AddWithValue("@estUsu", ConstantesGlobales.estadoCesado);
                command.Parameters.AddWithValue("@cesfchEmp", DateTime.Now);
                command.Parameters.AddWithValue("@fchModUsu", DateTime.Now);
                command.Parameters.AddWithValue("@userModUsu", SessionPersister.Username);

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
        public void agregarJefe(string[] idEmp, string id)
        {
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {

                if (idEmp != null)
                {
                    foreach (var a in idEmp)
                    {
                        string commandText = "UPDATE tb_Empleado SET idEmpJ = @idEmpJ  WHERE idEmp = @idEmp;";
                        SqlCommand command = new SqlCommand(commandText, connection);
                        command.Parameters.Add("@idEmp", SqlDbType.NVarChar);
                        command.Parameters["@idEmp"].Value = a;
                        command.Parameters.AddWithValue("@idEmpJ", id);
                        try
                        {
                            connection.Open();
                            Int32 rowsAffected = command.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
        public void eliminarJefe(string[] idEmp)
        {
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {

                if (idEmp != null)
                {
                    foreach (var a in idEmp)
                    {

                        string commandText = "UPDATE tb_Empleado SET idEmpJ = @idEmpJ  WHERE idEmp = @idEmp;";
                        SqlCommand command = new SqlCommand(commandText, connection);
                        command.Parameters.Add("@idEmp", SqlDbType.NVarChar);
                        command.Parameters["@idEmp"].Value = a;
                        command.Parameters.AddWithValue("@idEmpJ", DBNull.Value);
                        try
                        {
                            connection.Open();
                            Int32 rowsAffected = command.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
        public List<EmpleadoModels> obtenerEmpleados()
        {
            var db = new ApplicationDbContext();
            var emp = db.tb_Empleado.Include(x => x.ubicacion).Include(x => x.usuarios).OrderBy(x => x.nomComEmp).Include(x=>x.area).Include(x => x.cargo).Where(x=>x.idEst!= ConstantesGlobales.estadoCesado).ToList();
            return emp;
        }
        public List<EmpleadoModels> obtenerCumpleanio()
        {
            using (var db = new ApplicationDbContext())
            {
                var emp = db.tb_Empleado.Include(x=>x.usuarios).Include(x => x.cargo).Include(x => x.area).OrderBy(x => x.nomComEmp).Where(x => x.idEst != ConstantesGlobales.estadoCesado && (x.nacEmp.Month==DateTime.Today.Month && x.nacEmp.Day == DateTime.Today.Day)).ToList();
                return emp;
            }
        }
        public List<EmpleadoModels> obtenerEmpleadosExport()
        {
            var db = new ApplicationDbContext();
            var emp = db.tb_Empleado.Include(x => x.usuarios).Include(x => x.ubicacion).Include(x => x.afp).Include(x => x.gene).Include(x => x.estCiv).Include(x=>x.area).Include(x => x.usuarios.Select(y => y.UseZonLin.Select(z => z.linea))).Include(x => x.usuarios.Select(y => y.UseZonLin.Select(z => z.zona))).Include(x => x.estado).Include(x => x.cargo).OrderBy(x => x.nomComEmp).Include(x => x.usuarios).ToList();
            return emp;
        }

        public EmpleadoModels obtenerxDniEmpleado(string dni)
        {
            using (var db = new ApplicationDbContext())
            {
                var emp = db.tb_Empleado.Include(x=>x.usuarios).Where(x=>x.nroDocEmp== dni).FirstOrDefault();
                return emp;
            }
        }
        public EmpleadoModels obtenerXnomCompEmp(string nomComp)
        {
            using (var db = new ApplicationDbContext())
            {
                var emp = db.tb_Empleado.Include(x => x.usuarios).Where(x => x.nomComEmp == nomComp).FirstOrDefault();
                return emp;
            }
        }

    }
}