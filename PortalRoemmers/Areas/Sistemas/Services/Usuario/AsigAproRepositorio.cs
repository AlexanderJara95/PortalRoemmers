using PortalRoemmers.Areas.Sistemas.Models.Usuario;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace PortalRoemmers.Areas.Sistemas.Services.Usuario
{
    public class AsigAproRepositorio
    {

        public Boolean crear(List<AsigAproModels> model)
        {
            Boolean resultado = true;
            var db = new ApplicationDbContext();

            db.tb_Asig_Apro.AddRange(model);
            try
            {
                db.SaveChanges();

            }
            catch (Exception e)
            {
                resultado = false;
            }
            return resultado;
        }
        public Boolean eliminar(string[] asignados, string aprobador)
        {

            bool respuesta = true;

            using (var db = new ApplicationDbContext())
            {
                List<AsigAproModels> proLin = (from p in db.tb_Asig_Apro
                                               where asignados.Contains(p.idAccAsig.ToString()) && p.idAccApro == aprobador
                                               select p).ToList();


                db.tb_Asig_Apro.RemoveRange(proLin);
                try
                {
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }
        public  List<AsigAproModels> obtenerAprobadores(string id)//obtengo los aprobadores desde un aprobador
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Asig_Apro.Where(x => x.idAccApro == id).ToList();
            return model;
        }
        public List<AsigAproModels> obtenerAprobadoresAsignados(string asignado)//obtengo los aprobadores desde un asignado
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Asig_Apro.Include(x=>x.aprobador).Include(x => x.aprobador.empleado).Where(x => x.idAccAsig == asignado).ToList();
            return model;
        }

        public List<AsigAproModels> obtenerTodosAproAsig()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_Asig_Apro.Include(x => x.aprobador.empleado).ToList();
                return model;
            }
        }

        public List<UsuarioModels> obtenerAproYou(string idAccRes)
        {
            using (var db = new ApplicationDbContext())
            {
                var apro = db.tb_Asig_Apro.Where(x => x.idAccAsig == idAccRes).Select(x => x.aprobador.idAcc ).ToList();
                var model = db.tb_Usuario.Include(x=>x.empleado).Where(x => apro.Contains(x.idAcc) || x.idAcc== idAccRes).ToList();
                return model;
            }
        }
        public string obtenerNivAprob(string idAccRes)
        {
            using (var db = new ApplicationDbContext())
            {
                var nivel = db.tb_Usuario.Where(x => x.idAcc == idAccRes).FirstOrDefault();
                return nivel.idNapro;
            }
        }

    }
}