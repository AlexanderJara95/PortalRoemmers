namespace PortalRoemmers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cambiando_tipo_string_a_byte_ModelSolicitudRRHH : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_SolicitudRRHH", "documentoAdjunto", c => c.Binary());
            DropColumn("dbo.tb_SolicitudRRHH", "rutaArchivoAdjunto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_SolicitudRRHH", "rutaArchivoAdjunto", c => c.String(maxLength: 500));
            DropColumn("dbo.tb_SolicitudRRHH", "documentoAdjunto");
        }
    }
}
