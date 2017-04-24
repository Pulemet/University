using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using University.Models;
using University.Models.Dto;
using University.Models.Helper;

namespace University.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult PageSubmitRegistration(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            
            if (user != null)
            {
                UserDto uDto = new UserDto(user);
                var role = db.Roles.Where(r => r.Name == ConstDictionary.ROLE_STUDENT).Select(r => r).FirstOrDefault();
                if (user.Roles.Select(r => r.RoleId).Contains(role.Id))
                {
                    uDto.UserRole = UserRoles.Student;
                    var group = db.StudentGroups.Find(user.GroupId);
                    uDto.Group = group.Name;
                }
                else
                {
                    uDto.UserRole = UserRoles.Teacher;
                    var department = db.Departments.Find(user.GroupId);
                    uDto.Department = department.NameAbridgment;
                }
                return View(uDto);
            }
            ModelState.AddModelError("", "User does not exist");
            return View(new UserDto());
        }  
    }
}