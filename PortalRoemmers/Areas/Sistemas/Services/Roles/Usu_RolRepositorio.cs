using PortalRoemmers.Areas.Sistemas.Models.Roles;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Roles
{
    public class Usu_RolRepositorio
    {
        
        public void crearUsuRol(string rol, string usu)
        {
            var db = new ApplicationDbContext();

            // elimino
            /*Usu_RolModels 
            db.tb_Usu_Rol.Remove(acc);
            db.SaveChanges();*/

            ///creo
            Usu_RolModels model = new Usu_RolModels();

                model.rolId = rol;
                model.idAcc = usu;
                model.usuCrea = SessionPersister.Username;
                model.usufchCrea = DateTime.Now;

                db.tb_Usu_Rol.Add(model);
                db.SaveChanges();
            
        }

        public List<string> obtenerPermisos(string usu)
        {
            var db = new ApplicationDbContext();
            var permisos = db.tb_Usu_Rol.Where(y => y.idAcc == usu).Select(x=>x.rolId).ToList();
            return permisos;
        }

        public void eliminarDetalleUsuRol(string idAcc)
        {
            using (var db = new ApplicationDbContext()) {
                var deleteOrderDetails = (from details in db.tb_Usu_Rol where details.idAcc == idAcc select details);

                foreach (var detail in deleteOrderDetails)
                {
                    db.tb_Usu_Rol.Remove(detail);
                }
                db.SaveChanges();
            }
        }

        public void crearUsuRoles(List<Usu_RolModels> usu)
        {
            string[] selecR = usu.Select(s => s.rolId).ToArray();
            string[] selecU = usu.Select(s => s.idAcc).Distinct().ToArray();

            using (var db = new ApplicationDbContext())
            {
                List<Usu_RolModels> usuR = (from p in db.tb_Usu_Rol
                                            where selecR.Contains(p.rolId) && selecU.Contains(p.idAcc)
                                            select p).ToList();
                //elimino el que ya se encuentra en la base de datos
                if(usuR.Count()!=0)
                {
                    for (int i = 0; i < usuR.Count(); i++)
                    {
                        var idu1 = usuR[i].idAcc;
                        var idr1 = usuR[i].rolId;
                        for (int j = 0; j < usu.Count(); j++)
                        {                    
                            var idu2 = usu[j].idAcc;
                            var idr2 = usu[j].rolId;

                             if (idu1 == idu2 && idr1 == idr2)
                             {
                                 usu.Remove(usu[j]);
                             }

                        }
                    }
                }
                
                db.tb_Usu_Rol.AddRange(usu);
                db.SaveChanges();
            }
        }
        public void eliminarUsuRoles(List<Usu_RolModels> usu)
        {
            string[] selecR = usu.Select(s => s.rolId).ToArray();
            string[] selecU = usu.Select(s => s.idAcc).Distinct().ToArray();

            using (var db = new ApplicationDbContext())
            {
                List<Usu_RolModels> usuR = (from p in db.tb_Usu_Rol
                                            where selecR.Contains(p.rolId) && selecU.Contains(p.idAcc)
                                            select p).ToList();
                db.tb_Usu_Rol.RemoveRange(usuR);
                db.SaveChanges();
            }
        }

    }
}