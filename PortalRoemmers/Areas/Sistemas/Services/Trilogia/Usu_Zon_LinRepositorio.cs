using PortalRoemmers.Models;
using System;
using System.Linq;
using System.Data.Entity;
using PortalRoemmers.Security;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using PortalRoemmers.Areas.Sistemas.Models.Trilogia;

namespace PortalRoemmers.Areas.Sistemas.Services.Trilogia
{
    public class Usu_Zon_LinRepositorio
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

                var model = db.tb_Usu_Zon_Lin
                  .Include(x=>x.user.empleado).Include(y=>y.linea).Include(z=>z.zona).Include(a=>a.estado)
                  .OrderBy(x =>new { x.idAcc,x.idLin,x.idZon }).Where(x => x.user.empleado.nomComEmp.Contains(search)  || x.linea.nomLin.Contains(search) || x.zona.nomZon.Contains(search) || x.estado.nomEst.Contains(search))
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_Usu_Zon_Lin.Where(x => x.user.empleado.nomComEmp.Contains(search) || x.linea.nomLin.Contains(search) || x.zona.nomZon.Contains(search) || x.estado.nomEst.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.UZL = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public Usu_Zon_Lin_Models obtenerItem(string usu, string lin, string zon)
        {
            var db = new ApplicationDbContext();
            Usu_Zon_Lin_Models model = new Usu_Zon_Lin_Models();

            try
            {
                model = db.tb_Usu_Zon_Lin.Include(x => x.user.empleado).Include(x => x.linea).Include(x => x.zona).Where(x => x.idAcc == usu && x.idLin == lin && x.idZon == zon).SingleOrDefault();
            }
            catch (Exception e) {
            
            }
            
            return model;
        }
        public string crear(Usu_Zon_Lin_Models model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();

            db.tb_Usu_Zon_Lin.Add(model);
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
        public string updateEstUsu(string usu, string lin, string zon,string est)
        {
            string commandText = "UPDATE tb_Usu_Zon_Lin SET idEst = @estUsu,usuAnu=@usuAnu,usufchAnu=@usufchAnu  WHERE idAcc = @idAcc and idLin=@idLin and idZon=@idZon ;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idAcc", SqlDbType.Int);
                command.Parameters.Add("@idLin", SqlDbType.Int);
                command.Parameters.Add("@idZon", SqlDbType.Int);
                command.Parameters["@idAcc"].Value = usu;
                command.Parameters["@idLin"].Value = lin;
                command.Parameters["@idZon"].Value = zon;

                command.Parameters.AddWithValue("@estUsu", est);
                command.Parameters.AddWithValue("@usuAnu", SessionPersister.Username);
                command.Parameters.AddWithValue("@usufchAnu", DateTime.Now);
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return " < div id = 'warning' class='alert alert-warning'>Se produjo un error.</div>";
                }

            }
            return "<div id='success' class='alert alert-success'>Se cambio el estado correctamente.</div>";
        }
        public Boolean verificar(string usu, string lin, string zon)
        {
            Boolean rpt = false;
            var model= obtenerItem(usu, lin, zon);

            if (model!=null)
            {
                rpt = true;
            }
            return rpt;
        }
        public List<Usu_Zon_Lin_Models> obtenerTrilogia()
        {
            var db = new ApplicationDbContext();
            var tri = db.tb_Usu_Zon_Lin.Include(x => x.user).Include(x=>x.linea).Include(x => x.zona).Where(x=>x.idEst!=ConstantesGlobales.estadoInactivo).OrderBy(x => x.idAcc).ToList();
            return tri;
        }

    }
}