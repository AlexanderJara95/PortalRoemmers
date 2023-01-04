using PortalRoemmers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalRoemmers.Helpers
{
    public class Parametros
    {
        public List<string> Resultado(string codigo)
        {
            using (var db = new ApplicationDbContext())
            {
                var model = db.tb_DetPar.Where(x => x.idPar == codigo).Select(x => x.valDetPar).ToList();
                return model;
            }
        }
        public List<SelectedModels> selectResultado(string codigo)
        {
            using (var db = new ApplicationDbContext())
            {
                var result = db.tb_DetPar.Where(x => x.idPar == codigo).Select(x => new { x.idDetPar, x.valDetPar});
                //-----
                List<SelectedModels> cboList = new List<SelectedModels>();
                SelectedModels cbo = new SelectedModels();
                foreach (var v in result)
                {
                    cbo.value = v.valDetPar;
                    cbo.text = v.idDetPar;
                    cboList.Add(cbo);
                    cbo = new SelectedModels();
                }
                //----
                return cboList;
            }
        }
    }
}