namespace PortalRoemmers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModelGrupoRRHH : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Area", "GrupoRRHHModels_idGrupoRrhh", c => c.String(maxLength: 7));
            AddColumn("dbo.tb_Usuario", "GrupoRRHHModels_idGrupoRrhh", c => c.String(maxLength: 7));
            CreateIndex("dbo.tb_Area", "GrupoRRHHModels_idGrupoRrhh");
            CreateIndex("dbo.tb_Usuario", "GrupoRRHHModels_idGrupoRrhh");
            AddForeignKey("dbo.tb_Area", "GrupoRRHHModels_idGrupoRrhh", "dbo.tb_GrupoRRHH", "idGrupoRrhh");
            AddForeignKey("dbo.tb_Usuario", "GrupoRRHHModels_idGrupoRrhh", "dbo.tb_GrupoRRHH", "idGrupoRrhh");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Usuario", "GrupoRRHHModels_idGrupoRrhh", "dbo.tb_GrupoRRHH");
            DropForeignKey("dbo.tb_Area", "GrupoRRHHModels_idGrupoRrhh", "dbo.tb_GrupoRRHH");
            DropIndex("dbo.tb_Usuario", new[] { "GrupoRRHHModels_idGrupoRrhh" });
            DropIndex("dbo.tb_Area", new[] { "GrupoRRHHModels_idGrupoRrhh" });
            DropColumn("dbo.tb_Usuario", "GrupoRRHHModels_idGrupoRrhh");
            DropColumn("dbo.tb_Area", "GrupoRRHHModels_idGrupoRrhh");
        }
    }
}
