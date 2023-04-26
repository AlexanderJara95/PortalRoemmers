namespace PortalRoemmers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModelGrupo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_AreaGrupoRRHH",
                c => new
                    {
                        idAreRoe = c.String(nullable: false, maxLength: 7),
                        idGrupoRrhh = c.String(nullable: false, maxLength: 7),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.idAreRoe, t.idGrupoRrhh })
                .ForeignKey("dbo.tb_GrupoRRHH", t => t.idGrupoRrhh)
                .Index(t => t.idGrupoRrhh);
            
            CreateTable(
                "dbo.tb_GrupoRRHH",
                c => new
                    {
                        idGrupoRrhh = c.String(nullable: false, maxLength: 7),
                        descGrupo = c.String(nullable: false, maxLength: 200),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => t.idGrupoRrhh);
            
            CreateTable(
                "dbo.tb_ExcluGrupoRRHH",
                c => new
                    {
                        idAcc = c.String(nullable: false, maxLength: 10),
                        idGrupoRrhh = c.String(nullable: false, maxLength: 7),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.idAcc, t.idGrupoRrhh })
                .ForeignKey("dbo.tb_Usuario", t => t.idAcc)
                .ForeignKey("dbo.tb_GrupoRRHH", t => t.idGrupoRrhh)
                .Index(t => t.idAcc)
                .Index(t => t.idGrupoRrhh);
            
            CreateTable(
                "dbo.tb_GrupoSolicitudRRHH",
                c => new
                    {
                        idGrupoRrhh = c.String(nullable: false, maxLength: 7),
                        idSolicitudRrhh = c.String(nullable: false, maxLength: 7),
                        descGrupo = c.String(nullable: false, maxLength: 200),
                        usuCrea = c.String(),
                        usufchCrea = c.DateTime(),
                        usuMod = c.String(),
                        usufchMod = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.idGrupoRrhh, t.idSolicitudRrhh })
                .ForeignKey("dbo.tb_GrupoRRHH", t => t.idGrupoRrhh)
                .Index(t => t.idGrupoRrhh);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_GrupoSolicitudRRHH", "idGrupoRrhh", "dbo.tb_GrupoRRHH");
            DropForeignKey("dbo.tb_ExcluGrupoRRHH", "idGrupoRrhh", "dbo.tb_GrupoRRHH");
            DropForeignKey("dbo.tb_ExcluGrupoRRHH", "idAcc", "dbo.tb_Usuario");
            DropForeignKey("dbo.tb_AreaGrupoRRHH", "idGrupoRrhh", "dbo.tb_GrupoRRHH");
            DropIndex("dbo.tb_GrupoSolicitudRRHH", new[] { "idGrupoRrhh" });
            DropIndex("dbo.tb_ExcluGrupoRRHH", new[] { "idGrupoRrhh" });
            DropIndex("dbo.tb_ExcluGrupoRRHH", new[] { "idAcc" });
            DropIndex("dbo.tb_AreaGrupoRRHH", new[] { "idGrupoRrhh" });
            DropTable("dbo.tb_GrupoSolicitudRRHH");
            DropTable("dbo.tb_ExcluGrupoRRHH");
            DropTable("dbo.tb_GrupoRRHH");
            DropTable("dbo.tb_AreaGrupoRRHH");
        }
    }
}
