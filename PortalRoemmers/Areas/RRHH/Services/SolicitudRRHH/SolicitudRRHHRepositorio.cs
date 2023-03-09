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
using PortalRoemmers.Areas.RRHH.Models.SolicitudRRHH;

namespace PortalRoemmers.Areas.RRHH.Services.SolicitudRRHH
{
    public class SolicitudRRHHRepositorio
    {
        Ennumerador enu = new Ennumerador();
        UsuarioRepositorio _usu = new UsuarioRepositorio();

        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search, string modulo, string primero, string actual)
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

                var model = db.tb_SolicitudRRHH
                    .Include(x => x.solicitante.empleado)
                    .Include(x => x.aprobador.empleado)
                    .Include(x => x.estado)
                    .Include(x => x.subtipoSolicitud)
                    .OrderByDescending(x => x.idSolicitudRrhh).Where(x => ((x.idAccSol == SessionPersister.UserId) && x.idEstado != ConstantesGlobales.estadoAnulado) && ((x.usufchCrea >= p) && (x.usufchCrea <= a)) && (x.descSolicitud.Contains(search) || (x.subtipoSolicitud.descSubtipoSolicitud.Contains(search)) || x.estado.nomEst.Contains(search)))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.tb_SolGastos.Where(x => ((x.idAccRes == SessionPersister.UserId || x.idAccSol == SessionPersister.UserId) && x.idEst != ConstantesGlobales.estadoAnulado && x.modSolGas == modulo) && ((x.usufchCrea >= p) && (x.usufchCrea <= a)) && (x.titSolGas.Contains(search) || (x.obsSolGas.Contains(search)) || x.idSolGas.Contains(search) || x.estado.nomEst.Contains(search))).Count();
                var modelo = new ViewModels.IndexViewModel();
                modelo.SoliRRHH = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
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

        public Boolean crearUserSolRrhh (UserSolicitudRRHHModels model)
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

            
                



    }
}