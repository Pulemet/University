using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using University.Models;
using University.Models.Dto;
using University.Models.Helper;
using University.Models.Tables;

namespace University.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Register");
            }

            var user = db.Users.Find(User.Identity.GetUserId());
            return View(GetUserDto(user));
        }

        [Authorize]
        public ActionResult UserPage(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            var userId = User.Identity.GetUserId();
            var listRelationsUsers = db.Friends.Where(t => t.UserOneId == user.Id || t.UserTwoId == user.Id).Select(t => t);
            var isFriend = db.Users.Join(listRelationsUsers, u => u.Id, f => f.UserOneId == user.Id ? f.UserTwoId : f.UserOneId, (u, f) => u).
                                       FirstOrDefault(u => u.Id == userId);
            ViewData["IsFriend"] = isFriend != null;
            return View(GetUserDto(user));
        }

        private UserDto GetUserDto(ApplicationUser user)
        {
            UserDto uDto = new UserDto(user);

            var role = db.Roles.Where(r => r.Name == ConstDictionary.ROLE_STUDENT).Select(r => r).FirstOrDefault();
            if (user.Roles.Select(r => r.RoleId).Contains(role.Id))
            {
                var group = db.StudentGroups.Find(user.GroupId);
                var speciality = db.Specialities.Find(group.SpecialityId);
                var facultity = db.Faculties.Find(speciality.FacultyId);
                uDto.Group = group.Name;
                uDto.Speciality = speciality.NameAbridgment;
                uDto.Faculty = facultity.NameAbridgment;
                uDto.UserRole = UserRoles.Student;
            }
            else
            {
                role = db.Roles.Where(r => r.Name == ConstDictionary.ROLE_ADMIN).Select(r => r).FirstOrDefault();
                uDto.UserRole = user.Roles.Select(r => r.RoleId).Contains(role.Id)
                                       ? UserRoles.Admin
                                       : UserRoles.Teacher;
                if (uDto.UserRole == UserRoles.Teacher)
                {
                    var department = db.Departments.Find(user.GroupId);
                    uDto.Department = department.NameAbridgment;
                }
            }

            return uDto;
        }

        [Authorize]
        public ActionResult ShowFriedns(string userId)
        {
            var listFriendsId = db.Friends.Where(t => t.UserOneId == userId);
            List<ApplicationUser> listFriends = new List<ApplicationUser>();

            foreach (var friedId in listFriendsId)
            {
                ApplicationUser user = db.Users.Find(friedId.UserTwoId);
                if(user != null)
                    listFriends.Add(user);
            }
            return View(listFriends);
        }

        public ActionResult CompletionRegistration()
        {
            return View();
        }

        public ActionResult PopUpSendMessage(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            return PartialView(user);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}