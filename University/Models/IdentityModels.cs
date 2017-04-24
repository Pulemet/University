using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using University.Models.Tables;


// enable-migrations

// Add-Migration "DataMigration"

// Update-Database



namespace University.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FirstName", this.FirstName));
            userIdentity.AddClaim(new Claim("SurName", this.SurName));
            userIdentity.AddClaim(new Claim("PatronymicName", this.PatronymicName));
            userIdentity.AddClaim(new Claim("Gender", this.Gender));
            userIdentity.AddClaim(new Claim("Photo", this.Photo));
            userIdentity.AddClaim(new Claim("BirthDate", this.BirthDate.ToShortDateString()));
            userIdentity.AddClaim(new Claim("GroupId", this.GroupId.ToString()));
            return userIdentity;
        }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public string PatronymicName { get; set; }

        public string Gender { get; set; }

        public string Photo { get; set; }

        public DateTime BirthDate { get; set; }

        public int GroupId { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Department> Departments { get; set; }

        public DbSet<Faculty> Faculties { get; set; }

        public DbSet<Speciality> Specialities { get; set; }

        public DbSet<StudentGroup> StudentGroups { get; set; }

        public DbSet<Subject> Subjects { get; set; }

        public DbSet<Friend> Friends { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Dialog> Dialogs { get; set; }

        public DbSet<UserToDialog> UserToDialogs { get; set; }

        public DbSet<Material> Materials { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<MaterialComment> MaterialComments { get; set; }

        public DbSet<AwaitingUser> AwaitingUsers { get; set; }

        public DbSet<TeacherToSubject> TeacherToSubjects { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}