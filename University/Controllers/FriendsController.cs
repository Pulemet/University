using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using University.Models;

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
                                       OrderBy(u => u.SurName);

            return View(listFriends);
        }
    }
}