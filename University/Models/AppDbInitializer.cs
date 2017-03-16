using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace University.Models
{
        public class AppDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
        {
            protected override void Seed(ApplicationDbContext context)
            {
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
                var admin = new ApplicationUser { Email = "runec@mail.ru", UserName = "runec@mail.ru", BirthDate = DateTime.Now, Photo = "" };
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
    }