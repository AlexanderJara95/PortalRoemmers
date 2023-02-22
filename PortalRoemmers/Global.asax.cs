using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MvcPdfWriter.Core;

namespace PortalRoemmers
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

			ViewEngines.Engines.Add(new PdfViewEngine());
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
