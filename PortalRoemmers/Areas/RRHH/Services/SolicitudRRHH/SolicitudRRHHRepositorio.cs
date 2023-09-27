using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Areas.RRHH.Models.Grupo;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Web;
using System.Linq;
using System.Data;
using System.Data.Entity; //permite usar landa
using System.Data.SqlClient;
using PortalRoemmers.Security;
using PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH;
using System.Collections.Generic;

namespace PortalRoemmers.Areas.RRHH.Services.SolicitudRRHH
{
    public class SolicitudRRHHRepositorio
    {
        Ennumerador enu = new Ennumerador();
        UsuarioRepositorio _usu = new UsuarioRepositorio();

        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search, string subtipo, string primero, string actual)
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
                //SOLICITUDES PROPIAS
                var model = db.tb_SolicitudRRHH
                    .Include(x => x.solicitante.empleado)
                    .Include(x => x.aprobador.empleado)
                    .Include(x => x.estado)
                    .Include(x => x.subtipoSolicitud)
                    .OrderByDescending(x => x.idSolicitudRrhh)
                    .Where(x => ((x.idAccSol == SessionPersister.UserId || x.idAccApro == SessionPersister.UserId) && x.idEstado != ConstantesGlobales.estadoAnulado && (x.idSubTipoSolicitudRrhh == subtipo || (x.idSubTipoSolicitudRrhh == ConstantesGlobales.subTipoVacacionesM && (x.idAccSol == SessionPersister.UserId || x.idAccApro == SessionPersister.UserId)))) && ((x.fchIniSolicitud >= p) && (x.fchIniSolicitud <= a) && (x.fchFinSolicitud >= p) && (x.fchFinSolicitud <= a)) && (x.descSolicitud.Contains(search) || (x.subtipoSolicitud.descSubtipoSolicitud.Contains(search)) || x.estado.nomEst.Contains(search)))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                //AREA DEL USUARIO
                string idAreaUser = db.tb_Usuario
                    .Where(usu => usu.idAcc == SessionPersister.UserId)
                    .Join(db.tb_Empleado,
                        usu => usu.idEmp,
                        emp => emp.idEmp,
                        (usu, emp) => emp.idAreRoe)
                    .Join(db.tb_Area,
                        idAreRoe => idAreRoe,
                        area => area.idAreRoe,
                        (idAreRoe, area) => area.idAreRoe)
                    .FirstOrDefault();

                //SOLICITUDES MASIVAS
                var modelMasiva = db.tb_SolicitudRRHH
                    .Include(x => x.solicitante.empleado)
                    .Include(x => x.aprobador.empleado)
                    .Include(x => x.estado)
                    .Include(x => x.subtipoSolicitud)
                    .OrderByDescending(x => x.idSolicitudRrhh)
                    .Where(solicitud =>
                    db.tb_GrupoSolicitudRRHH.Any(grupoSol =>
                        grupoSol.idSolicitudRrhh == solicitud.idSolicitudRrhh &&
                        db.tb_GrupoRRHH.Any(grupo =>
                            grupo.idGrupoRrhh == grupoSol.idGrupoRrhh &&
                            db.tb_AreaGrupoRRHH.Any(areaGrupo =>
                                areaGrupo.idGrupoRrhh == grupo.idGrupoRrhh &&
                                areaGrupo.idAreRoe == idAreaUser) &&
                            !db.tb_ExcluGrupoRRHH.Any(exclude =>
                                exclude.idGrupoRrhh == grupo.idGrupoRrhh &&
                                exclude.idAcc == SessionPersister.UserId))))
                    .ToList();

                var modelFinal = model.Concat(modelMasiva).ToList();
                var totalDeRegistros = modelFinal.Count;

                var modelo = new ViewModels.IndexViewModel();
                modelo.SoliRRHH = modelFinal;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }

        public List<SolicitudRRHHModels> obtenerSolicitudesUsuario(string primero, string actual, string idAccSol)
        {
            // ------------------------------------------------------------------------
            DateTime p = DateTime.Parse(primero); //desde
            DateTime a = DateTime.Parse(actual).AddHours(23).AddMinutes(59);//hasta
            //------------------------------------------------------------------------

            using (var db = new ApplicationDbContext())
            {
                //SOLICITUDES PROPIAS
                var model = db.tb_SolicitudRRHH
                    .Include(x => x.solicitante.empleado)
                    .Include(x => x.aprobador.empleado)
                    .Include(x => x.estado)
                    .Include(x => x.subtipoSolicitud)
                    .OrderByDescending(x => x.idSolicitudRrhh)
                    .Where(x => ((x.idAccSol == idAccSol) && x.idEstado != ConstantesGlobales.estadoAnulado && (x.idSubTipoSolicitudRrhh == ConstantesGlobales.subTipoVacaciones || (x.idSubTipoSolicitudRrhh == ConstantesGlobales.subTipoVacacionesM && x.idAccSol == idAccSol))) && ((x.fchIniSolicitud >= p) && (x.fchIniSolicitud <= a) && (x.fchFinSolicitud >= p) && (x.fchFinSolicitud <= a)))
                    .ToList();

                //AREA DEL USUARIO
                string idAreaUser = db.tb_Usuario
                    .Where(usu => usu.idAcc == SessionPersister.UserId)
                    .Join(db.tb_Empleado,
                        usu => usu.idEmp,
                        emp => emp.idEmp,
                        (usu, emp) => emp.idAreRoe)
                    .Join(db.tb_Area,
                        idAreRoe => idAreRoe,
                        area => area.idAreRoe,
                        (idAreRoe, area) => area.idAreRoe)
                    .FirstOrDefault();

                //SOLICITUDES MASIVAS
                var modelMasiva = db.tb_SolicitudRRHH
                    .Include(x => x.solicitante.empleado)
                    .Include(x => x.aprobador.empleado)
                    .Include(x => x.estado)
                    .Include(x => x.subtipoSolicitud)
                    .OrderByDescending(x => x.idSolicitudRrhh)
                    .Where(solicitud =>
                    db.tb_GrupoSolicitudRRHH.Any(grupoSol =>
                        grupoSol.idSolicitudRrhh == solicitud.idSolicitudRrhh &&
                        db.tb_GrupoRRHH.Any(grupo =>
                            grupo.idGrupoRrhh == grupoSol.idGrupoRrhh &&
                            db.tb_AreaGrupoRRHH.Any(areaGrupo =>
                                areaGrupo.idGrupoRrhh == grupo.idGrupoRrhh &&
                                areaGrupo.idAreRoe == idAreaUser) &&
                            !db.tb_ExcluGrupoRRHH.Any(exclude =>
                                exclude.idGrupoRrhh == grupo.idGrupoRrhh &&
                                exclude.idAcc == SessionPersister.UserId))))
                    .ToList();
                var modelFinal = model.Concat(modelMasiva).ToList();
                return modelFinal;
            }
        }

        public SolicitudRRHHModels obtenerItem(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                SolicitudRRHHModels model = db.tb_SolicitudRRHH
                    .Include(x => x.solicitante.empleado)
                    .Include(x => x.aprobador.empleado)
                    .Include(x => x.estado)
                    .Include(x => x.subtipoSolicitud)
                    .Where(x => x.idSolicitudRrhh == id).FirstOrDefault();
                return model;
            }

        }
        public Boolean modificar(SolicitudRRHHModels model)
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
        public Boolean modificarUserSoliRrhh(SolicitudRRHHModels model)
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
        public Boolean modificarGrupoSoliRrhh(GrupoSolicitudRRHHModels model)
        {
            Boolean ok = false;
            using (var db = new ApplicationDbContext())
            {
                var filaExistente = db.tb_GrupoSolicitudRRHH.FirstOrDefault(g => g.idSolicitudRrhh == model.idSolicitudRrhh );
                try
                {
                    if (filaExistente != null)
                    {
                        db.tb_GrupoSolicitudRRHH.Remove(filaExistente);
                        db.SaveChanges();
                        ok = crearGrupoSoliRrhh(model);
                    }
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }

            }
            return ok;
        }

        public Boolean crear(SolicitudRRHHModels model)
        {   
            Boolean exec = true;
            using (var db = new ApplicationDbContext())
            {
                db.tb_SolicitudRRHH.Add(model);
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

        public Boolean crearUserSoliRrhh (UserSolicitudRRHHModels model)
        {
            Boolean exec = true;
            using (var db = new ApplicationDbContext())
            {
                db.tb_UserSolicitudRRHH.Add(model);
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

        public GrupoSolicitudRRHHModels obtenerGrupoSoliRrhh(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                GrupoSolicitudRRHHModels model = db.tb_GrupoSolicitudRRHH
                    .Where(x => x.idSolicitudRrhh == id).FirstOrDefault();
                return model;
            }

        }
        public Boolean crearGrupoSoliRrhh(GrupoSolicitudRRHHModels model)
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

        public bool validarExisteEnRegistro(string id, DateTime desde, DateTime hasta)
        {
            using (var db = new ApplicationDbContext())
            {
                bool existeRegistro = db.tb_SolicitudRRHH
                    .Any(x => x.idAccSol == id && x.fchIniSolicitud == desde && x.fchFinSolicitud == hasta);

                return existeRegistro;
            }
        }

        public Boolean updateEstadoSoliRRHH(string sol, string estado)
        {
            string commandText = "UPDATE tb_SolicitudRRHH SET idEstado = @idEstado, usuMod=@usuMod , usufchMod=@usufchMod  WHERE idSolicitudRrhh = @idSolicitudRrhh ;";

            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                command.Parameters.Add("@idSolicitudRrhh", SqlDbType.VarChar);
                command.Parameters["@idSolicitudRrhh"].Value = sol;
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