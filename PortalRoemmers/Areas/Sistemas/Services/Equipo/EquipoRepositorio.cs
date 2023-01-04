using PortalRoemmers.Models;
using System.Linq;
using System.Data.Entity; //permite usar landa
using System;
using System.Web.UI.WebControls;
using PortalRoemmers.Areas.Sistemas.Models.Equipo;
using PortalRoemmers.Helpers;
using System.Collections.Generic;
using System.Data.SqlClient;
using PortalRoemmers.Security;

namespace PortalRoemmers.Areas.Sistemas.Services.Equipo
{
    public class EquipoRepositorio
    {
        Ennumerador enu = new Ennumerador();
        public ViewModels.IndexViewModel obtenerTodos(int pagina,string search)
        {
            int cantidadRegistrosPorPagina = 10;

            if (pagina==0)
            {
                pagina = 1;
            }

            using (var db=new ApplicationDbContext())
            {

                var model = db.tb_Equipo.Include(y => y.procesador)
                  .Include(c => c.empleado)
                  .OrderBy(x => x.nomPcEqui).Where(x => x.procesador.nomProce.Contains(search) || x.memEqui.Contains(search) || x.discEqui.Contains(search) || x.empleado.apePatEmp.Contains(search) || x.nomPcEqui.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                    var totalDeRegistros = db.tb_Equipo.Where(x => x.procesador.nomProce.Contains(search) || x.memEqui.Contains(search) || x.discEqui.Contains(search) || x.empleado.apePatEmp.Contains(search) || x.nomPcEqui.Contains(search)).Count();

                    var modelo = new ViewModels.IndexViewModel();
                    modelo.Equipos = model;
                    modelo.PaginaActual = pagina;
                    modelo.TotalDeRegistros = totalDeRegistros;
                    modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public EquipoModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            EquipoModels model = db.tb_Equipo.Include(x => x.sisOp).Include(x => x.procesador).Include(x=>x.modelos).Where(x=>x.idEquipo== id).SingleOrDefault();
            return model;

        }
        public string crear(EquipoModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_Equipo";
            int idc = enu.buscarTabla(tabla);
            model.idEquipo = idc.ToString();
            db.tb_Equipo.Add(model);
            try
            {
                db.SaveChanges();
                enu.actualizarTabla(tabla, idc);
                mensaje = "<div id='success' class='alert alert-success'>Se creó un nuevo registro.</div>";
            }
            catch (Exception e)
            {
                mensaje = "<div id='warning' class='alert alert-warning'>" + e.Message + "</div>";
            }
            return mensaje;
        }
        public string modificar(EquipoModels model)
        {
            string mensaje = "";
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

            EquipoModels model = db.tb_Equipo.Find(id);
            db.tb_Equipo.Remove(model);

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
        public List<EquipoModels> obtenerEquipos()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Equipo
                  .Include(y => y.procesador)
                  .Include(y => y.modelos)
                  .Include(y => y.estado)
                  .Include(y => y.tipEqui)
                  .Include(y => y.empleado).OrderBy(x => x.nomPcEqui).ToList();
                return model;
            }
        }
        public List<EquipoModels> obtenerEquiposDetallado()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Equipo
                    
                  .Include(x => x.modelos.fabrica)
                  .Include(x => x.modelos)
                  .Include(x => x.procesador)
                  .Include(x => x.tipEqui)
                  .Include(x => x.area)
                  .Include(x => x.sisOp)
                  .Include(x => x.estado)
                  .Include(x => x.empleado)
                  .Include(x => x.tipoRam)
                  .Include(x => x.tipoDisco)
                  .OrderBy(x => x.nomPcEqui).ToList();
                return model;
            }
        }
        //sin ninguna relacion(include)
        public List<EquipoModels> obtenerOnlyEquipos(string parametro)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Equipo.OrderBy(x => x.nomPcEqui).Where(x => x.nomPcEqui.Contains(parametro)).ToList();
                return model;
            }
        }
        public List<EquipoChart> consultaEquipo()
        {
            EquipoChart ec = new EquipoChart();
            List<EquipoChart> ecs = new List<EquipoChart>();
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(
                "select t.nomTipEqui as label, COUNT(*) as data,t.colTipEqui as color  from tb_Equipo e join tb_TipEqui t on e.idTipEqui =t.idTipEqui group by t.nomTipEqui,t.colTipEqui;  ",
                connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ec.label =  reader.GetValue(0).ToString() +" : "+ reader.GetValue(1).ToString();
                            ec.data =  int.Parse(reader.GetValue(1).ToString());
                            ec.color = reader.GetValue(2).ToString();
                            ecs.Add(ec);
                            ec = new EquipoChart();
                        }
                    }
                }
            }
            return ecs;
        }

        public Boolean sqlUpdateBaja(string detEqui,string idEquipo)
        {
            string tabla = "TB_BAJA";
            int idc = enu.buscarTabla(tabla);
            string nombreE = "BAJA" + idc.ToString("D6");
            string commandText = " update tb_Equipo set idEst=@idEst,detEqui=@detEqui,nomPcEqui=@nomPcEqui,usufchMod=@usufchMod,usuMod=@usuMod where idEquipo=@idEquipo;";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@idEst", ConstantesGlobales.estadoInactivo);
                command.Parameters.AddWithValue("@nomPcEqui", nombreE);
                command.Parameters.AddWithValue("@detEqui", detEqui);
                command.Parameters.AddWithValue("@idEquipo", idEquipo);
                command.Parameters.AddWithValue("@usufchMod",DateTime.Now );
                command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    enu.actualizarTabla(tabla, idc);
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

    public class EquipoChart
    {
        public string label { get; set; }
        public int data { get; set; }
        public string color { get; set; }
    }
}