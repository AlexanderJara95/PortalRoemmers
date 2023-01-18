using PortalRoemmers.Areas.Sistemas.Services.Usuario;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity; //permite usar landa

using System.Text;

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

                var model = db.tb_SolGastos
                    .Include(x => x.estado)
                    .Include(x => x.concepto.tipoGasto)
                    .Include(x => x.dFirma.Select(y => y.gasto))
                    .Include(x => x.dFirma.Select(y => y.estado))
                    .Include(x => x.dFirma.Select(y => y.solicitante))
                    .Include(x => x.moneda)
                    .Include(x => x.responsable.empleado)
                    .Include(x => x.concepto)
                    .Include(x => x.zona).Include(x => x.linea)
                    .Include(x => x.especialidad)
                    .Include(x => x.dFam.Select(y => y.familia))
                    .Include(x => x.dMed.Select(y => y.cliente))
                    .Include(x => x.dResp.Select(y => y.responsable))
                    .Include(x => x.dFil)
                    .Include(x => x.liquidacion).DefaultIfEmpty()
                    .Include(x => x.liquidacion.estado).DefaultIfEmpty()
                    .OrderByDescending(x => x.idSolGas).Where(x => ((x.idAccRes == SessionPersister.UserId || x.idAccSol == SessionPersister.UserId) && x.idEst != ConstantesGlobales.estadoAnulado && x.modSolGas == modulo) && ((x.usufchCrea >= p) && (x.usufchCrea <= a)) && (x.titSolGas.Contains(search) || (x.obsSolGas.Contains(search)) || x.idSolGas.Contains(search) || x.estado.nomEst.Contains(search)))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.tb_SolGastos.Where(x => ((x.idAccRes == SessionPersister.UserId || x.idAccSol == SessionPersister.UserId) && x.idEst != ConstantesGlobales.estadoAnulado && x.modSolGas == modulo) && ((x.usufchCrea >= p) && (x.usufchCrea <= a)) && (x.titSolGas.Contains(search) || (x.obsSolGas.Contains(search)) || x.idSolGas.Contains(search) || x.estado.nomEst.Contains(search))).Count();
                var modelo = new ViewModels.IndexViewModel();
                modelo.SolGasto = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }
        }
    }
}