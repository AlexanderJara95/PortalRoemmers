namespace PortalRoemmers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregaAprobFinalSolicitudRRHHModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_SolicitudRRHH", "aprobFinal", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_SolicitudRRHH", "aprobFinal");
        }
    }
}
