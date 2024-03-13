namespace PortalRoemmers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregaPeriodoSolicitudRRHHModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_SolicitudRRHH", "periodo", c => c.String(nullable: false, maxLength: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_SolicitudRRHH", "periodo");
        }
    }
}
