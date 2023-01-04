using PortalRoemmers.Areas.Sistemas.Models.Producto;
using PortalRoemmers.Helpers;
using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PortalRoemmers.Areas.Sistemas.Services.Producto
{
    public class FamiliaRoeRepositorio
    {
        Ennumerador enu = new Ennumerador();
        public ViewModels.IndexViewModel obtenerTodos(int pagina, string search)
        {
            int cantidadRegistrosPorPagina = 10;

            if (pagina == 0)
            {
                pagina = 1;
            }

            using (var db = new ApplicationDbContext())
            {

                var model = db.tb_FamProdRoe
                  .OrderBy(x => x.idFamRoe).Where(x => x.nomFamRoe.Contains(search) || x.desFamRoe.Contains(search))
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.tb_FamProdRoe.Where(x => x.nomFamRoe.Contains(search) || x.desFamRoe.Contains(search)).Count();

                var modelo = new ViewModels.IndexViewModel();
                modelo.FamiliaRoe = model;
                modelo.PaginaActual = pagina;
                modelo.TotalDeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadRegistrosPorPagina;

                return modelo;
            }

        }
        public FamProdRoeModels obtenerItem(string id)
        {
            var db = new ApplicationDbContext();
            FamProdRoeModels model = db.tb_FamProdRoe.Find(id);
            return model;
        }
        public string crear(FamProdRoeModels model)
        {
            string mensaje = "";
            var db = new ApplicationDbContext();
            //creo su ID
            string tabla = "tb_FamProdRoe";
            int idc = enu.buscarTabla(tabla);
            model.idFamRoe = idc.ToString();

            db.tb_FamProdRoe.Add(model);
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
        public string modificar(FamProdRoeModels model)
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

            FamProdRoeModels model = db.tb_FamProdRoe.Find(id);
            db.tb_FamProdRoe.Remove(model);
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

        //listados
        public List<FamProdRoeModels> obtenerFamiliaRoe()
        {
            var db = new ApplicationDbContext();
            var model = db.tb_FamProdRoe.OrderBy(x => x.nomFamRoe).ToList();
            return model;
        }
        public List<QueryFamATGroup> obtenerFamiliaRoeAT()
        {
            //List<QueryFamATGroup> model0 = new List<QueryFamATGroup>();
            //List<QueryFamATGroup> model1 = new List<QueryFamATGroup>();
            List<QueryFamATGroup> model2 = new List<QueryFamATGroup>();

            using (var db = new ApplicationDbContext())
            {
                //PRUEBAS PARA OBTENER LA MEJOR CONSULTA
                 /*model1 = db.tb_FamProdRoe
                .Include(x => x.productos)
                .Include(x => x.productos.Select(y => y.areaTerap))
                .Select(m => new QueryFamATGroup { cod1 = m.idFamRoe, nom1 = m.nomFamRoe, nom2 = m.productos.Select(x => x.areaTerap.numAreaTerap).FirstOrDefault(), cod2 = m.productos.Select(x => x.areaTerap.idAreaTerap).FirstOrDefault() })
                .Where(x=>x.nom1.Contains("HIDROLAGENO"))
                .OrderBy(x => x.nom1)
                .ToList();*/

                //Esta no funciona
                /*model = db.tb_FamProdRoe
                .Include(x => x.productos)
                .Include(x => x.productos.Select(y => y.areaTerap))
                .Select(m => new { parametro1 = m.idFamRoe , parametro2 = m.nomFamRoe , parametro3 = m.productos.Select(x=>x.areaTerap.numAreaTerap), parametro4 = m.productos.Select(x=>x.areaTerap.idAreaTerap) })
                .GroupBy(z => new QueryFamATGroup { cod1 = z.parametro1, nom1 = z.parametro2, nom2 = z.parametro3.FirstOrDefault(), cod2 = z.parametro4.FirstOrDefault() })
                .Select(m => new QueryFamATGroup { cod1 = m.Select(x=>x.parametro1).FirstOrDefault() , nom1 = m.Select(x => x.parametro2).FirstOrDefault(), nom2 = m.Select(x => x.parametro3).Select(y=>y.FirstOrDefault()).FirstOrDefault() , cod2 = m.Select(x => x.parametro4).Select(y => y.FirstOrDefault()).FirstOrDefault() })
                .OrderBy(x => x.nom1)
                .ToList();*/

                /*model0 = db.tb_FamProdRoe
                .Include(x => x.productos)
                .Include(x => x.productos.Select(y => y.areaTerap))
                .GroupBy(z => new QueryFamATGroup { cod1 = z.idFamRoe, nom1 = z.nomFamRoe , nom2 = z.productos.Select(y=>y.areaTerap.numAreaTerap).FirstOrDefault(), cod2 = z.productos.Select(x=>x.areaTerap.idAreaTerap).FirstOrDefault()})
                .Select(m => new QueryFamATGroup { cod1 = m.Select(x=>x.idFamRoe).FirstOrDefault() , nom1 = m.Select(x => x.nomFamRoe).FirstOrDefault(), nom2 = m.Select(x=>x.productos.Select(y=>y.areaTerap.numAreaTerap).FirstOrDefault()).FirstOrDefault(), cod2 = m.Select(x => x.productos.Select(y => y.areaTerap.idAreaTerap).FirstOrDefault()).FirstOrDefault() })
                .OrderBy(x => x.nom1)
                .ToList();*/

                model2 = db.tb_FamProdRoe
                    .Join(db.tb_Producto,
                    fam => fam.idFamRoe,
                    pro => pro.idFamRoe,
                    (fam, pro) => new { FamProdRoe = fam, Producto = pro })
                    .Join(db.tb_AreaTerap,
                    roe => roe.Producto.idAreaTerap,
                    art => art.idAreaTerap,
                    (roe, art) => new { roe.Producto, roe.FamProdRoe, AreaTerap = art })
                    .Select(c => new QueryFamATGroup
                    {
                        cod1 = c.FamProdRoe.idFamRoe,
                        nom1 = c.FamProdRoe.nomFamRoe,
                        nom2 = c.AreaTerap.numAreaTerap,
                        cod2 = c.AreaTerap.idAreaTerap
                    }).Distinct().ToList();

            }
            return model2;
        }
        public class QueryFamATGroup
        {
            public string cod1 { get; set; }
            public string nom1 { get; set; }
            public string nom2 { get; set; }
            public string cod2 { get; set; }
        }    
    }
}