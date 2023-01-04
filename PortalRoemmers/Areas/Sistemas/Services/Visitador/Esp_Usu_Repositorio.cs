using PortalRoemmers.Areas.Sistemas.Models.Visitador;
using PortalRoemmers.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PortalRoemmers.Areas.Sistemas.Services.Visitador
{
    public class Esp_Usu_Repositorio
    {

        public Boolean crear(List<Esp_Usu_Models> model)
        {
            Boolean resultado = true;
            var db = new ApplicationDbContext();

            db.tb_Esp_Usu.AddRange(model);
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
        public Boolean eliminar(string[] asignados,string id)
        {

            bool respuesta = true;

            using (var db = new ApplicationDbContext())
            {
                List<Esp_Usu_Models> espUsu = (from p in db.tb_Esp_Usu
                                               where asignados.Contains(p.idAcc) && p.idEsp== id
                                               select p).ToList();
                db.tb_Esp_Usu.RemoveRange(espUsu);

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

        public List<Esp_Usu_Models> obtenerUsuarioxEspecialidad(string idEsp)
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Esp_Usu.Include(x=>x.accounts.empleado).Where(x => x.idEsp == idEsp).ToList();
            return model;
        }

        public List<Esp_Usu_Models> obtenerEspecialidadesXusuario(string idAccRes)
        {
            var db = new ApplicationDbContext();
            var model = db.tb_Esp_Usu.Include(x => x.especialidad).Where(x => x.idAcc==idAccRes).ToList();
            return model;
        }

    }
}