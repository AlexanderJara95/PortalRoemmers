using PortalRoemmers.Areas.RRHH.Models.Boleta;
using PortalRoemmers.Models;
using PortalRoemmers.Security;
using PortalRoemmers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace PortalRoemmers.Areas.RRHH.Services.Boleta
{
    public class BoletaDetalleRepositorio
    {
        public List<BoletaDetalleModels> obtenerBoletas(string idBolPer,string Value)
        {
            Boolean v = Convert.ToBoolean(Value);
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_BolDet.Include(x=>x.Boleta).Where(x=>x.idBolPer== idBolPer && x.visBolDet== v).ToList();
                return model;
            }
        }

        public BoletaDetalleModels obtenerItem(string id,string nro)
        {
            using (var db = new ApplicationDbContext())
            {
                BoletaDetalleModels model = db.tb_BolDet.Find(id, nro);
                return model;
            }

        }
        
        public Boolean crear(List<BoletaDetalleModels> model,string idBolPer)
        {
            string commandText = "delete from tb_BolDet where idBolPer=@idBolPer;";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idBolPer", SqlDbType.VarChar).Value = idBolPer;
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
  
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.tb_BolDet.AddRange(model);
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            return mensaje;
        }

        public Boolean modificar(BoletaDetalleModels model){
            Boolean mensaje = false;
            using (var db = new ApplicationDbContext())
            {
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    mensaje = true;
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            }
            return mensaje;
         
        }

        public Boolean mergeDetalle(BoletaDetalleModels model)
        {
            Boolean exito = true;
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    BoletaDetalleModels m = db.tb_BolDet.Find(model.idBolPer,model.nroDocBolDet);
                    if (m!=null)
                    {
                        m.usuMod = SessionPersister.Username;
                        m.usufchMod = DateTime.UtcNow;
                        m.rutBolDet = model.rutBolDet;
                        m.nomBolDet = model.nomBolDet;
                        db.Entry(m).State = EntityState.Modified;
                    }
                    else
                    {
                        model.usuCre = SessionPersister.Username;
                        model.usufchCre = DateTime.UtcNow;
                        
                        db.Entry(model).State = EntityState.Added;
                    }

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    exito = false;
                    e.Message.ToString();
                }
            }
            return exito;
        }

        public Boolean actualizarRuta(string idBolPer,string rutBolDet)
        {
            Boolean mensaje = false;
            //../Boleta/2020-20/
            string commandText = "  update tb_BolDet set rutBolDet=@rutBolDet + nomBolDet  where idBolPer=@idBolPer;";
            using (SqlConnection connection = new SqlConnection(Conexion.connetionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@idBolPer", SqlDbType.VarChar).Value = idBolPer;
                command.Parameters.Add("@rutBolDet", SqlDbType.VarChar).Value = rutBolDet;
                try
                {
                   connection.Open();
                   command.ExecuteNonQuery();
                   connection.Close();
                   mensaje = true;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return mensaje;
        }
        
        public List<BoletaDetalleModels> obtenerBoletasPersonal(string dni)
        {
            var db = new ApplicationDbContext();
            var model = db.tb_BolDet.Where(x=>x.nroDocBolDet== dni).Include(x => x.Boleta).OrderByDescending(x => x.idBolPer).ToList();
            return model;
        }
    }
}