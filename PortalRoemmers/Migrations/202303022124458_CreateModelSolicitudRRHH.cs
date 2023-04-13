namespace PortalRoemmers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModelSolicitudRRHH : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_SolicitudRRHH",
                c => new
                    {
                        idSolicitudRrhh = c.String(nullable: false, maxLength: 7),
                        idAccSol = c.String(nullable: false, maxLength: 10),
                        idAccApro = c.String(nullable: false, maxLength: 10),
                        descSolicitud = c.String(nullable: false, maxLength: 200),
                        fchIniSolicitud = c.DateTime(nullable: false),
                        fchFinSolicitud = c.DateTime(nullable: false),
                        rutaArchivoAdjunto = c.String(maxLength: 500),
                        idEstado = c.String(nullable: false, maxLength: 10),
                        idSubTipoSolicitudRrhh = c.String(maxLength: 10),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => t.idSolicitudRrhh)
                .ForeignKey("dbo.tb_Usuario", t => t.idAccApro)
                .ForeignKey("dbo.tb_Estado", t => t.idEstado)
                .ForeignKey("dbo.tb_Usuario", t => t.idAccSol)
                .ForeignKey("dbo.tb_SubtipoSolicitudRRHH", t => t.idSubTipoSolicitudRrhh)
                .Index(t => t.idAccSol)
                .Index(t => t.idAccApro)
                .Index(t => t.idEstado)
                .Index(t => t.idSubTipoSolicitudRrhh);
            
            CreateTable(
                "dbo.tb_SubtipoSolicitudRRHH",
                c => new
                    {
                        idSubTipoSolicitudRrhh = c.String(nullable: false, maxLength: 10),
                        nomSubtipoSolicitud = c.String(nullable: false, maxLength: 50),
                        descSubtipoSolicitud = c.String(nullable: false, maxLength: 200),
                        idTipoSolicitudRrhh = c.String(maxLength: 10),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => t.idSubTipoSolicitudRrhh)
                .ForeignKey("dbo.tb_TipoSolicitudRRHH", t => t.idTipoSolicitudRrhh)
                .Index(t => t.idTipoSolicitudRrhh);
            
            CreateTable(
                "dbo.tb_TipoSolicitudRRHH",
                c => new
                    {
                        idTipoSolicitudRrhh = c.String(nullable: false, maxLength: 10),
                        nomTipoSolicitud = c.String(nullable: false, maxLength: 50),
                        descTipoSolicitud = c.String(nullable: false, maxLength: 200),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => t.idTipoSolicitudRrhh);
            
            CreateTable(
                "dbo.tb_UserSolicitudRRHH",
                c => new
                    {
                        idSolicitudRrhh = c.String(nullable: false, maxLength: 7),
                        idAccRes = c.String(nullable: false, maxLength: 10),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.idSolicitudRrhh, t.idAccRes })
                .ForeignKey("dbo.tb_Usuario", t => t.idAccRes)
                .Index(t => t.idAccRes);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_UserSolicitudRRHH", "idAccRes", "dbo.tb_Usuario");
            DropForeignKey("dbo.tb_SubtipoSolicitudRRHH", "idTipoSolicitudRrhh", "dbo.tb_TipoSolicitudRRHH");
            DropForeignKey("dbo.tb_SolicitudRRHH", "idSubTipoSolicitudRrhh", "dbo.tb_SubtipoSolicitudRRHH");
            DropForeignKey("dbo.tb_SolicitudRRHH", "idAccSol", "dbo.tb_Usuario");
            DropForeignKey("dbo.tb_SolicitudRRHH", "idEstado", "dbo.tb_Estado");
            DropForeignKey("dbo.tb_SolicitudRRHH", "idAccApro", "dbo.tb_Usuario");
            DropIndex("dbo.tb_UserSolicitudRRHH", new[] { "idAccRes" });
            DropIndex("dbo.tb_SubtipoSolicitudRRHH", new[] { "idTipoSolicitudRrhh" });
            DropIndex("dbo.tb_SolicitudRRHH", new[] { "idSubTipoSolicitudRrhh" });
            DropIndex("dbo.tb_SolicitudRRHH", new[] { "idEstado" });
            DropIndex("dbo.tb_SolicitudRRHH", new[] { "idAccApro" });
            DropIndex("dbo.tb_SolicitudRRHH", new[] { "idAccSol" });
            DropTable("dbo.tb_UserSolicitudRRHH");
            DropTable("dbo.tb_TipoSolicitudRRHH");
            DropTable("dbo.tb_SubtipoSolicitudRRHH");
            DropTable("dbo.tb_SolicitudRRHH");
        }
    }
}
