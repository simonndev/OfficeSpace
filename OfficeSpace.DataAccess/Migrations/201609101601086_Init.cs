namespace OfficeSpace.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Buildings",
                c => new
                    {
                        BuildingId = c.Int(nullable: false, identity: true),
                        BuildingName = c.String(nullable: false, maxLength: 255),
                        BuildingCode = c.String(nullable: false, maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                        LocationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BuildingId)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .Index(t => t.BuildingCode, unique: true, name: "Idx_Building_Code")
                .Index(t => t.LocationId);
            
            CreateTable(
                "dbo.Floors",
                c => new
                    {
                        FloorId = c.Int(nullable: false, identity: true),
                        FloorName = c.String(nullable: false, maxLength: 255),
                        FloorCode = c.String(nullable: false, maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                        BuildingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FloorId)
                .ForeignKey("dbo.Buildings", t => t.BuildingId)
                .Index(t => t.FloorCode, unique: true, name: "Idx_Floor_Code")
                .Index(t => t.BuildingId);
            
            CreateTable(
                "dbo.Units",
                c => new
                    {
                        UnitId = c.Int(nullable: false, identity: true),
                        UnitName = c.String(nullable: false, maxLength: 255),
                        UnitCode = c.String(nullable: false, maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                        FloorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UnitId)
                .ForeignKey("dbo.Floors", t => t.FloorId)
                .Index(t => t.UnitCode, unique: true, name: "Idx_Unit_Code")
                .Index(t => t.FloorId);
            
            CreateTable(
                "dbo.Workspaces",
                c => new
                    {
                        WorkspaceId = c.Int(nullable: false, identity: true),
                        WorkspaceName = c.String(nullable: false, maxLength: 255),
                        WorkspaceCode = c.String(nullable: false, maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                        UnitId = c.Int(nullable: false),
                        WorkspaceTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkspaceId)
                .ForeignKey("dbo.WorkspaceTypes", t => t.WorkspaceTypeId)
                .ForeignKey("dbo.Units", t => t.UnitId)
                .Index(t => t.WorkspaceCode, unique: true, name: "Idx_Workspace_Code")
                .Index(t => t.UnitId)
                .Index(t => t.WorkspaceTypeId);
            
            CreateTable(
                "dbo.WorkspaceTypes",
                c => new
                    {
                        TypeId = c.Int(nullable: false, identity: true),
                        TypeName = c.String(nullable: false, maxLength: 255),
                        Description = c.String(nullable: true),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => t.TypeId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationId = c.Int(nullable: false, identity: true),
                        LocationName = c.String(nullable: false, maxLength: 255),
                        LocationCode = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: true),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                        ParentId = c.Int(nullable: true),
                    })
                .PrimaryKey(t => t.LocationId)
                .ForeignKey("dbo.Locations", t => t.ParentId)
                .Index(t => t.LocationCode, unique: true, name: "Idx_Location_Code")
                .Index(t => t.ParentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Locations", "ParentId", "dbo.Locations");
            DropForeignKey("dbo.Buildings", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Floors", "BuildingId", "dbo.Buildings");
            DropForeignKey("dbo.Units", "FloorId", "dbo.Floors");
            DropForeignKey("dbo.Workspaces", "UnitId", "dbo.Units");
            DropForeignKey("dbo.Workspaces", "WorkspaceTypeId", "dbo.WorkspaceTypes");
            DropIndex("dbo.Locations", new[] { "ParentId" });
            DropIndex("dbo.Locations", "Idx_Location_Code");
            DropIndex("dbo.Workspaces", new[] { "WorkspaceTypeId" });
            DropIndex("dbo.Workspaces", new[] { "UnitId" });
            DropIndex("dbo.Workspaces", "Idx_Workspace_Code");
            DropIndex("dbo.Units", new[] { "FloorId" });
            DropIndex("dbo.Units", "Idx_Unit_Code");
            DropIndex("dbo.Floors", new[] { "BuildingId" });
            DropIndex("dbo.Floors", "Idx_Floor_Code");
            DropIndex("dbo.Buildings", new[] { "LocationId" });
            DropIndex("dbo.Buildings", "Idx_Building_Code");
            DropTable("dbo.Locations");
            DropTable("dbo.WorkspaceTypes");
            DropTable("dbo.Workspaces");
            DropTable("dbo.Units");
            DropTable("dbo.Floors");
            DropTable("dbo.Buildings");
        }
    }
}
