using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using University.Models;
using University.Models.Tables;

namespace University.Controllers
{
    public class FindUserController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: FindUser
        public ActionResult Index()
        {  
            string userId = User.Identity.GetUserId();

            var users = GetUsers(userId);
            List<string> userNames = (from u in users select u.SurName + " " + u.FirstName).Distinct().ToList();
            ViewData["UserNames"] = userNames;
            return View(users);
        }

        [HttpPost]
        public void AddFriend(string id)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(userId);
            ApplicationUser userFriend = db.Users.Find(id);
            if (userFriend != null)
            {
                var listFriedsUser = db.Friends.Where(f=>(f.UserOneId == user.Id && f.UserTwoId == userFriend.Id)
                                                       || (f.UserOneId == userFriend.Id && f.UserTwoId == user.Id)).Select(f=>f).ToList();
                if (listFriedsUser.Count == 0)
                {
                    Friend friend = new Friend()
                    {
                        UserOneId = user.Id,
                        UserTwoId = userFriend.Id
                    };
                    db.Friends.Add(friend);
                    db.SaveChanges();
                } 
            }
        }

        [HttpGet]
        public ActionResult SearchUser(string name)
        {
            string userId = User.Identity.GetUserId();
            string surName = name.Split(' ')[0];
            string firstName = name.Split(' ')[1];
            var allUsers = GetUsers(userId);
            var users = allUsers.Where(u => u.FirstName == firstName && u.SurName == surName).Select(u => u);
            return PartialView("PartialViewUsers", users);
        }

        private IEnumerable<ApplicationUser> GetUsers(string id)
        {
            const string strRoleAdmin = "admin";
            var listRelationsUsers = db.Friends.Where(t => t.UserOneId == id || t.UserTwoId == id).Select(t => t);
            var listFriends = db.Users.Join(listRelationsUsers, u => u.Id, f => f.UserOneId == id ? f.UserTwoId : f.UserOneId, (u, f) => u);

            var role = db.Roles.Where(r => r.Name == strRoleAdmin).Select(r => r).FirstOrDefault();
            var users = db.Users.ToList().Where(t => t.Id != id).
                                          Where(m => !m.Roles.Select(r => r.RoleId).Contains(role.Id)).
                                          Select(m => m);

            // убрать из списка ожидающих активацию аккаунта и друзей
            users = (from user in users where !(from a in db.AwaitingUsers.ToList() select a.UserId).Contains(user.Id) select user);
            users = users.Except(listFriends).OrderBy(u => u.SurName);
            return users;
        }
    }
}