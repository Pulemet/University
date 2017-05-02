using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using University.Models;
using University.Models.Dto;
using University.Models.Helper;

namespace University.Controllers
{
    public class FriendsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Friends
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var listRelationsUsers = db.Friends.Where(t => t.UserOneId == userId || t.UserTwoId == userId).Select(t=>t);
            var listFriends = db.Users.Join(listRelationsUsers, u => u.Id, f => f.UserOneId == userId ? f.UserTwoId : f.UserOneId, (u, f) => u).
                                       OrderBy(u => u.SurName).ToList();

            List<UserDto> users = new List<UserDto>();

            foreach (var user in listFriends)
            {
                users.Add(GetUserDto(user));
            }

            return View(users);
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
    }
}