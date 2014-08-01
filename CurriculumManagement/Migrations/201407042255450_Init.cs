namespace CurriculumManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EAForms",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AcademicYear = c.String(nullable: false, maxLength: 11),
                        Course = c.String(nullable: false, maxLength: 60),
                        BlockWeekTitle = c.String(),
                        ActivityTitle = c.String(),
                        ActivityType = c.String(),
                        ActivityFacilitatorType = c.String(),
                        OtherActivityFacilitatorType = c.String(),
                        ActivityFacilitatorNames = c.String(),
                        ActivityFacilitatorDepartments = c.String(),
                        Abstract = c.String(),
                        LearningObjectives = c.String(),
                        Keywords = c.String(),
                        Formulary = c.String(),
                        Themes = c.String(),
                        StartDate = c.DateTime(),
                        LastSubmitted = c.DateTime(),
                        LastUpdated = c.DateTime(nullable: false),
                        InstructorSignature = c.String(),
                        ParentForm_ID = c.Int(),
                        Status_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.EAForms", t => t.ParentForm_ID)
                .ForeignKey("dbo.EAFormStatus", t => t.Status_ID)
                .Index(t => t.ParentForm_ID)
                .Index(t => t.Status_ID);
            
            CreateTable(
                "dbo.EAFormSaves",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                        Status_ID = c.Int(),
                        EAForm_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.EAFormStatus", t => t.Status_ID)
                .ForeignKey("dbo.EAForms", t => t.EAForm_ID)
                .Index(t => t.Status_ID)
                .Index(t => t.EAForm_ID);
            
            CreateTable(
                "dbo.EAFormStatus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Keywords",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.EAForms", "Status_ID", "dbo.EAFormStatus");
            DropForeignKey("dbo.EAFormSaves", "EAForm_ID", "dbo.EAForms");
            DropForeignKey("dbo.EAFormSaves", "Status_ID", "dbo.EAFormStatus");
            DropForeignKey("dbo.EAForms", "ParentForm_ID", "dbo.EAForms");
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.EAForms", new[] { "Status_ID" });
            DropIndex("dbo.EAFormSaves", new[] { "EAForm_ID" });
            DropIndex("dbo.EAFormSaves", new[] { "Status_ID" });
            DropIndex("dbo.EAForms", new[] { "ParentForm_ID" });
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Keywords");
            DropTable("dbo.EAFormStatus");
            DropTable("dbo.EAFormSaves");
            DropTable("dbo.EAForms");
        }
    }
}
