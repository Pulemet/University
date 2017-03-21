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
            var listRelationsUsers = db.Friends.Where(t => t.UserOneId == userId || t.UserTwoId == userId).Select(t=>t).ToList();
            List<ApplicationUser> listFriends = new List<ApplicationUser>();

            foreach (var relationsUsers in listRelationsUsers)
            {
                listFriends.Add(relationsUsers.UserOneId == userId ?
                                db.Users.Find(relationsUsers.UserTwoId) :
                                db.Users.Find(relationsUsers.UserOneId));
            }

            return View(listFriends);
        }
    }
}