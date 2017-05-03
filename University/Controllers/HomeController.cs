using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using University.Models;
using University.Models.Dto;
using University.Models.Helper;
using University.Models.Tables;

namespace University.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private Dictionary<string, string> _roles = new Dictionary<string, string>();

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            var roles = _db.Roles.ToList();

            foreach (var role in roles)
            {
                _roles.Add(role.Name, role.Id);
            }
            
            base.Initialize(requestContext);
        }

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Register");
            }

            var user = _db.Users.Find(User.Identity.GetUserId());
            return View(GetUserDto(user));
        }

        [Authorize]
        public ActionResult UserPage(string id)
        {
            ApplicationUser user = _db.Users.Find(id);
            var userId = User.Identity.GetUserId();
            var listRelationsUsers = _db.Friends.Where(t => t.UserOneId == user.Id || t.UserTwoId == user.Id).Select(t => t);
            var isFriend = _db.Users.Join(listRelationsUsers, u => u.Id, f => f.UserOneId == user.Id ? f.UserTwoId : f.UserOneId, (u, f) => u).
                                       FirstOrDefault(u => u.Id == userId);
            ViewData["IsFriend"] = isFriend != null;
            return View(GetUserDto(user));
        }

        private UserDto GetUserDto(ApplicationUser user)
        {
            UserDto uDto = new UserDto(user);

            uDto.UserRole = GetUserRole(user);

            if (uDto.UserRole == UserRoles.Student)
            {
                var group = _db.StudentGroups.Find(user.GroupId);
                var speciality = _db.Specialities.Find(group.SpecialityId);
                var facultity = _db.Faculties.Find(speciality.FacultyId);
                uDto.Group = group.Name;
                uDto.Speciality = speciality.NameAbridgment;
                uDto.Faculty = facultity.NameAbridgment;
                uDto.UserRole = UserRoles.Student;
            }
            else
            {
                if (uDto.UserRole == UserRoles.Teacher)
                {
                    var department = _db.Departments.Find(user.GroupId);
                    uDto.Department = department.NameAbridgment;
                }
            }

            return uDto;
        }

        [Authorize(Roles = "admin")]
        public ActionResult CompletionRegistration()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private string GetUserRole(ApplicationUser user)
        {
            var roles = user.Roles.Select(r => r.RoleId).ToList();

            return roles.Contains(_roles[UserRoles.Admin])
                ? UserRoles.Admin
                : roles.Contains(_roles[UserRoles.Teacher])
                ? UserRoles.Teacher
                : UserRoles.Student;
        }
    }
}