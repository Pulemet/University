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
            ViewData["CurrentUser"] = true;
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
            ViewData["CurrentUser"] = false;
            return View(GetUserDto(user));
        }

        public ActionResult Friends()
        {
            string userId = User.Identity.GetUserId();
            var listRelationsUsers = _db.Friends.Where(t => t.UserOneId == userId || t.UserTwoId == userId).Select(t => t);
            var listFriends = _db.Users.Join(listRelationsUsers, u => u.Id, f => f.UserOneId == userId ? f.UserTwoId : f.UserOneId, (u, f) => u).
                                       OrderBy(u => u.SurName).ToList();

            List<UserInfoDto> usersInfo = GetUsersInfoDto(listFriends, true);

            return View(usersInfo);
        }

        [HttpPost]
        public void AddFriend(string id)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = _db.Users.Find(userId);
            ApplicationUser userFriend = _db.Users.Find(id);
            if (userFriend != null)
            {
                var listFriedsUser = _db.Friends.Where(f => (f.UserOneId == user.Id && f.UserTwoId == userFriend.Id)
                                                       || (f.UserOneId == userFriend.Id && f.UserTwoId == user.Id)).Select(f => f).ToList();
                if (listFriedsUser.Count == 0)
                {
                    Friend friend = new Friend()
                    {
                        UserOneId = user.Id,
                        UserTwoId = userFriend.Id
                    };
                    _db.Friends.Add(friend);
                    _db.SaveChanges();
                }
            }
        }

        public ActionResult FindUsers()
        {
            string userId = User.Identity.GetUserId();

            var users = GetUsers(userId);

            List<UserInfoDto> usersInfo = GetUsersInfoDto(users, false);

            List<string> userNames = users.Select(u => u.SurName + " " + u.FirstName).Distinct().ToList();
            ViewData["UserNames"] = userNames;

            return View(usersInfo);
        }

        [HttpGet]
        public ActionResult SearchUser(string name)
        {
            string userId = User.Identity.GetUserId();
            string surName = name.Split(' ')[0];
            string firstName = name.Split(' ')[1];
            var allUsers = GetUsers(userId);
            var users = allUsers.Where(u => u.FirstName == firstName && u.SurName == surName).Select(u => u).ToList();
            List<UserInfoDto> usersInfo = GetUsersInfoDto(users, false);
            return PartialView("PartialViewUsers", usersInfo);
        }

        private List<ApplicationUser> GetUsers(string id)
        {
            const string strRoleAdmin = "admin";
            var listRelationsUsers = _db.Friends.Where(t => t.UserOneId == id || t.UserTwoId == id).Select(t => t);
            var listFriends = _db.Users.Join(listRelationsUsers, u => u.Id, f => f.UserOneId == id ? f.UserTwoId : f.UserOneId, (u, f) => u);

            var role = _db.Roles.Where(r => r.Name == strRoleAdmin).Select(r => r).FirstOrDefault();
            var users = _db.Users.ToList().Where(t => t.Id != id).
                                          Where(m => !m.Roles.Select(r => r.RoleId).Contains(role.Id)).
                                          Select(m => m);

            // убрать из списка ожидающих активацию аккаунта и друзей
            users = (from user in users where !(from a in _db.AwaitingUsers.ToList() select a.UserId).Contains(user.Id) select user);
            List<ApplicationUser> listUsers = users.Except(listFriends).OrderBy(u => u.SurName).ToList();

            return listUsers;
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

        private List<UserInfoDto> GetUsersInfoDto(List<ApplicationUser> users, bool isFriend)
        {
            List<UserInfoDto> usersInfo = new List<UserInfoDto>();

            foreach (var user in users)
            {
                UserInfoDto uDto = new UserInfoDto(user);
                uDto.UserRole = GetUserRole(user);
                uDto.IsFriend = isFriend;
                usersInfo.Add(uDto);
            }

            return usersInfo;
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