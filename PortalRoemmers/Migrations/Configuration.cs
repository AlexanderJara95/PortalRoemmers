namespace PortalRoemmers.Migrations
{
    using PortalRoemmers.Areas.Sistemas.Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<PortalRoemmers.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PortalRoemmers.Models.ApplicationDbContext context)
        {
         
        }
    }
}
