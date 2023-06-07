namespace PortalRoemmers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Agregando_Model_Doc_Sustento : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_DocSustentoRRHH",
                c => new
                    {
                        idDocSustento = c.String(nullable: false, maxLength: 10),
                        desDocSustento = c.String(nullable: false, maxLength: 200),
                        documentoAdjunto = c.Binary(nullable: false),
                        idTipoDocSustento = c.String(maxLength: 10),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => t.idDocSustento)
                .ForeignKey("dbo.tb_TipoDocSustentoRRHH", t => t.idTipoDocSustento)
                .Index(t => t.idTipoDocSustento);
            
            CreateTable(
                "dbo.tb_TipoDocSustentoRRHH",
                c => new
                    {
                        idTipoDocSustento = c.String(nullable: false, maxLength: 10),
                        nomTipDocSustento = c.String(nullable: false, maxLength: 50),
                        desTipDocSustento = c.String(nullable: false, maxLength: 200),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => t.idTipoDocSustento);
            
            CreateTable(
                "dbo.tb_DocSustentoSolicitudRRHH",
                c => new
                    {
                        idSolicitudRrhh = c.String(nullable: false, maxLength: 7),
                        idDocSustento = c.String(nullable: false, maxLength: 10),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.idSolicitudRrhh, t.idDocSustento })
                .ForeignKey("dbo.tb_DocSustentoRRHH", t => t.idDocSustento)
                .Index(t => t.idDocSustento);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_DocSustentoSolicitudRRHH", "idDocSustento", "dbo.tb_DocSustentoRRHH");
            DropForeignKey("dbo.tb_DocSustentoRRHH", "idTipoDocSustento", "dbo.tb_TipoDocSustentoRRHH");
            DropIndex("dbo.tb_DocSustentoSolicitudRRHH", new[] { "idDocSustento" });
            DropIndex("dbo.tb_DocSustentoRRHH", new[] { "idTipoDocSustento" });
            DropTable("dbo.tb_DocSustentoSolicitudRRHH");
            DropTable("dbo.tb_TipoDocSustentoRRHH");
            DropTable("dbo.tb_DocSustentoRRHH");
        }
    }
}
