using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity; //permite usar landa
using System.Data.SqlClient;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.RRHH.Models.Grupo;
using PortalRoemmers.Areas.Sistemas.Models.Usuario;

namespace PortalRoemmers.Areas.RRHH.Services.Grupo
{
    public class GrupoRRHHRepositorio
    {
        Ennumerador enu = new Ennumerador();
        UsuarioRepositorio _usu = new UsuarioRepositorio();

        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search, string tipo, string primero, string actual)
        {
            //------------------------------------------------------------------------
            DateTime p = DateTime.Parse(primero); //desde
            DateTime a = DateTime.Parse(actual).AddHours(23).AddMinutes(59);//hasta
            //------------------------------------------------------------------------
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }

            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_grupoRRHH
                    .OrderByDescending(x => x.idGrupoRrhh)
                    //.Where(x => (x.idEstado != ConstantesGlobales.estadoAnulado && x.subtipoSolicitud.idTipoSolicitudRrhh == tipo) && ((x.fchIniSolicitud >= p) && (x.fchIniSolicitud <= a) && (x.fchFinSolicitud >= p) && (x.fchFinSolicitud <= a)) && (x.descSolicitud.Contains(search) || (x.subtipoSolicitud.descSubtipoSolicitud.Contains(search)) || x.estado.nomEst.Contains(search)))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina)
                    .Include(x => x.estado) // Carga el estado relacionado
                    .ToList();
                
                var totalDeRegistros = db.tb_SolicitudRRHH.Where(x => ((x.idAccSol == SessionPersister.UserId || x.idAccApro == SessionPersister.UserId) && x.idEstado != ConstantesGlobales.estadoAnulado && x.subtipoSolicitud.idTipoSolicitudRrhh == tipo) && ((x.fchIniSolicitud >= p) && (x.fchIniSolicitud <= a) && (x.fchFinSolicitud >= p) && (x.fchFinSolicitud <= a)) && (x.descSolicitud.Contains(search) || (x.subtipoSolicitud.descSubtipoSolicitud.Contains(search)) || x.estado.nomEst.Contains(search))).Count();
                var modelo = new ViewModels.IndexViewModel();
                modelo.GrupoRRHH = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
        public GrupoRRHHModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                GrupoRRHHModels model = db.tb_grupoRRHH
                    .Where(x => x.idGrupoRrhh == id).FirstOrDefault();
                return model;
            }

        }
        public Boolean modificar(GrupoRRHHModels model)
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

        public Boolean crear(GrupoRRHHModels model)
        {
            Boolean exec = true;
            using (var db = new ApplicationDbContext())
            {
                db.tb_grupoRRHH.Add(model);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    exec = false;
                }
            }
            return exec;
        }
        public Boolean crearAreaGrupoRrhh(AreaGrupoRRHHModels model)
        {
            Boolean exec = true;
            using (var db = new ApplicationDbContext())
            {
                db.tb_AreaGrupoRRHH.Add(model);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    exec = false;
                }
            }
            return exec;
        }
        public List<AreaRoeModels> obtenerAreaGrupoRrhh(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                //var modelAreaGrupoRRHH = db.tb_AreaGrupoRRHH.Where(x => x.idGrupoRrhh == id).Select(x => x.idAreRoe).ToList();
                //var modelArea = db.tb_Area.Where(x => modelAreaGrupoRRHH.Contains(x.idAreRoe)).ToList();
                var model = db.tb_Area.Where(x => db.tb_AreaGrupoRRHH.Any(y => y.idGrupoRrhh == id && y.idAreRoe == x.idAreRoe)).ToList();
                return model;
            }
        }

        public Boolean crearExcluGrupoRrhh(ExcluGrupoRRHHModels model)
        {
            Boolean exec = true;
            using (var db = new ApplicationDbContext())
            {
                db.tb_ExcluGrupoRRHH.Add(model);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    exec = false;
                }
            }
            return exec;
        }

        /*public ExcluGrupoRRHHModels obtenerExcluGrupoRrhh(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                ExcluGrupoRRHHModels model = db.tb_ExcluGrupoRRHH
                    .Include(x => x.idGrupoRrhh && x.)
                    .Where(x => x.idGrupoRrhh == id).FirstOrDefault();
                return model;
            }

        }*/
        public List<UsuarioModels> obtenerExcluGrupoRrhh(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Usuario
                    .Include(x => x.empleado)
                    .Where(x => db.tb_ExcluGrupoRRHH.Any(y => y.idGrupoRrhh == id && y.idAcc == x.idAcc))
                    .Where(x => x.idEst != ConstantesGlobales.estadoCesado)
                    .OrderBy(x => x.idAcc).ToList();

                return model;
            }
        }

        public Boolean crearGrupoSolRrhh(GrupoSolicitudRRHHModels model)
        {
            Boolean exec = true;
            using (var db = new ApplicationDbContext())
            {
                db.tb_GrupoSolicitudRRHH.Add(model);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    exec = false;
                }
            }
            return exec;
        }
        public Boolean updateGrupo(string idGrupoRrhh, string descGrupo)
        {
            string commandText = "UPDATE tb_GrupoRRHH SET descGrupo = @descGrupo, usuMod=@usuMod , usufchMod=@usufchMod  WHERE idGrupoRrhh = @idGrupoRrhh;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                command.Parameters.Add("@idGrupoRrhh", SqlDbType.VarChar);
                command.Parameters["@idGrupoRrhh"].Value = idGrupoRrhh;
                command.Parameters.AddWithValue("@descGrupo", descGrupo);
                command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                command.Parameters.AddWithValue("@usufchMod", DateTime.Now);

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
        
        public Boolean updateEstadoGrupo(string id, string estado)
        {
            string commandText = "UPDATE tb_GrupoRRHH SET idEstado = @idEstado, usuMod=@usuMod , usufchMod=@usufchMod  WHERE idGrupoRrhh = @idGrupoRrhh ;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                command.Parameters.Add("@idGrupoRrhh", SqlDbType.VarChar);
                command.Parameters["@idGrupoRrhh"].Value = id;
                command.Parameters.AddWithValue("@idEstado", estado);
                command.Parameters.AddWithValue("@usuMod", SessionPersister.Username);
                command.Parameters.AddWithValue("@usufchMod", DateTime.Now);

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