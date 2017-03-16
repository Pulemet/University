namespace University.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameFull = c.String(),
                        NameAbridgment = c.String(),
                        Faculty_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculties", t => t.Faculty_Id)
                .Index(t => t.Faculty_Id);
            
            CreateTable(
                "dbo.Faculties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameFull = c.String(),
                        NameAbridgment = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Dialogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Friends",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserOne_Id = c.String(maxLength: 128),
                        UserTwo_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserOne_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserTwo_Id)
                .Index(t => t.UserOne_Id)
                .Index(t => t.UserTwo_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        SurName = c.String(),
                        PatronymicName = c.String(),
                        Photo = c.String(),
                        BirthDate = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        StudentGroup_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudentGroups", t => t.StudentGroup_Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.StudentGroup_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
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
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.StudentGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Speciality_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Specialities", t => t.Speciality_Id)
                .Index(t => t.Speciality_Id);
            
            CreateTable(
                "dbo.Specialities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameFull = c.String(),
                        NameAbridgment = c.String(),
                        Department_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.Department_Id)
                .Index(t => t.Department_Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        DateSend = c.DateTime(nullable: false),
                        Dialog_Id = c.Int(),
                        Sender_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dialogs", t => t.Dialog_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Sender_Id)
                .Index(t => t.Dialog_Id)
                .Index(t => t.Sender_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Semesters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        StarTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameFull = c.String(),
                        NameAbridgment = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubjectsToSemesterInGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Semester_Id = c.Int(),
                        StudentGroup_Id = c.Int(),
                        Subject_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Semesters", t => t.Semester_Id)
                .ForeignKey("dbo.StudentGroups", t => t.StudentGroup_Id)
                .ForeignKey("dbo.Subjects", t => t.Subject_Id)
                .Index(t => t.Semester_Id)
                .Index(t => t.StudentGroup_Id)
                .Index(t => t.Subject_Id);
            
            CreateTable(
                "dbo.UserToDialogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Dialog_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dialogs", t => t.Dialog_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Dialog_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserToDialogs", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserToDialogs", "Dialog_Id", "dbo.Dialogs");
            DropForeignKey("dbo.SubjectsToSemesterInGroups", "Subject_Id", "dbo.Subjects");
            DropForeignKey("dbo.SubjectsToSemesterInGroups", "StudentGroup_Id", "dbo.StudentGroups");
            DropForeignKey("dbo.SubjectsToSemesterInGroups", "Semester_Id", "dbo.Semesters");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Messages", "Sender_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "Dialog_Id", "dbo.Dialogs");
            DropForeignKey("dbo.Friends", "UserTwo_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Friends", "UserOne_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "StudentGroup_Id", "dbo.StudentGroups");
            DropForeignKey("dbo.StudentGroups", "Speciality_Id", "dbo.Specialities");
            DropForeignKey("dbo.Specialities", "Department_Id", "dbo.Departments");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Departments", "Faculty_Id", "dbo.Faculties");
            DropIndex("dbo.UserToDialogs", new[] { "User_Id" });
            DropIndex("dbo.UserToDialogs", new[] { "Dialog_Id" });
            DropIndex("dbo.SubjectsToSemesterInGroups", new[] { "Subject_Id" });
            DropIndex("dbo.SubjectsToSemesterInGroups", new[] { "StudentGroup_Id" });
            DropIndex("dbo.SubjectsToSemesterInGroups", new[] { "Semester_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Messages", new[] { "Sender_Id" });
            DropIndex("dbo.Messages", new[] { "Dialog_Id" });
            DropIndex("dbo.Specialities", new[] { "Department_Id" });
            DropIndex("dbo.StudentGroups", new[] { "Speciality_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "StudentGroup_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Friends", new[] { "UserTwo_Id" });
            DropIndex("dbo.Friends", new[] { "UserOne_Id" });
            DropIndex("dbo.Departments", new[] { "Faculty_Id" });
            DropTable("dbo.UserToDialogs");
            DropTable("dbo.SubjectsToSemesterInGroups");
            DropTable("dbo.Subjects");
            DropTable("dbo.Semesters");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Messages");
            DropTable("dbo.Specialities");
            DropTable("dbo.StudentGroups");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Friends");
            DropTable("dbo.Dialogs");
            DropTable("dbo.Faculties");
            DropTable("dbo.Departments");
        }
    }
}
