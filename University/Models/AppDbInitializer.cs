using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using University.Models.Tables;

namespace University.Models
{
        public class AppDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
        {
            protected override void Seed(ApplicationDbContext context)
            {
                // добавляем данные для факультетов, групп, специальностей, предметов
                DbInitData.Init();

                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                // создаем две роли
                var role1 = new IdentityRole { Name = "admin" };
                var role2 = new IdentityRole { Name = "student" };
                var role3 = new IdentityRole { Name = "teacher" };

                // добавляем роли в бд
                roleManager.Create(role1);
                roleManager.Create(role2);
                roleManager.Create(role3);

                // создаем пользователей
                var admin = new ApplicationUser { Email = "runec@mail.ru", UserName = "runec@mail.ru", BirthDate = DateTime.Now, Photo = "", FirstName = "", SurName = "", PatronymicName = ""};
                string password = "12!Qaz";
                var result = userManager.Create(admin, password);

                // если создание пользователя прошло успешно
                if (result.Succeeded)
                {
                    // добавляем для пользователя роль
                    userManager.AddToRole(admin.Id, role1.Name);
                }
                else
                {
                    Console.WriteLine(result.Errors);
                }

            base.Seed(context);
            }
        }

    public class DbInitData
    {
        public static readonly List<Faculty> Faculties = new List<Faculty>()
        {
            new Faculty() { NameAbridgment = "ФИТУ", NameFull = "Факультет информационных технологий и управления"},
            new Faculty() { NameAbridgment = "ФКСИС", NameFull = "Факультет компьютерных систем и сетей"},
            new Faculty() { NameAbridgment = "ФКП", NameFull = "Факультет компьютерного проектирования"}
        };

        public static readonly List<Speciality> Specialities = new List<Speciality>()
        {
            new Speciality() {FacultyId = 1, NameAbridgment = "ИИ", NameFull = "Искусственный интеллект"},
            new Speciality() {FacultyId = 1, NameAbridgment = "АСОИ", NameFull = "Автоматизированные системы обработки информации"},
            new Speciality() {FacultyId = 2, NameAbridgment = "ПОИТ", NameFull = "Программное обеспечение информационных технологий"},
            new Speciality() {FacultyId = 2, NameAbridgment = "Информатика", NameFull = "Информатика и технологии программирования"},
            new Speciality() {FacultyId = 3, NameAbridgment = "ПМС", NameFull = "Программируемые мобильные системы"},
            new Speciality() {FacultyId = 3, NameAbridgment = "ЭСБ", NameFull = "Электронные системы безопасности"}
        };

        public static readonly List<StudentGroup> Groups = new List<StudentGroup>()
        {
            new StudentGroup() { SpecialityId = 1, Name = "121701"},
            new StudentGroup() { SpecialityId = 1, Name = "121702"},
            new StudentGroup() { SpecialityId = 2, Name = "221701"},
            new StudentGroup() { SpecialityId = 2, Name = "221702"},
            new StudentGroup() { SpecialityId = 3, Name = "321701"},
            new StudentGroup() { SpecialityId = 3, Name = "321702"},
            new StudentGroup() { SpecialityId = 3, Name = "321703"},
            new StudentGroup() { SpecialityId = 4, Name = "421701"},
            new StudentGroup() { SpecialityId = 4, Name = "421702"},
            new StudentGroup() { SpecialityId = 5, Name = "521701"},
            new StudentGroup() { SpecialityId = 6, Name = "621701"},
            new StudentGroup() { SpecialityId = 6, Name = "621702"}
        };

        public static readonly List<Subject> Subjects = new List<Subject>()
        {
            new Subject() { NameAbridgment = "МОИС", NameFull = "Математические основы интеллектуальных систем"},
            new Subject() { NameAbridgment = "ОАИП", NameFull = "Основы алгоритмизации и программирования"},
            new Subject() { NameAbridgment = "ППВИС", NameFull = "Проектирование программ в интеллектуальных системах"}
        };
        public static void Init()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            db.Faculties.AddRange(Faculties);
            db.Specialities.AddRange(Specialities);
            db.StudentGroups.AddRange(Groups);
            db.Subjects.AddRange(Subjects);
            db.SaveChanges();
        }
    }
}

