using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using University.Models;
using University.Models.Dto;

namespace University.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PageSubmitRegistration(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            
            if (user != null)
            {
                UserDto uDto = new UserDto(user);
                uDto.Group = db.StudentGroups.Find(user.GroupId).Name;
                return View(uDto);
            }
            ModelState.AddModelError("", "User does not exist");
            return View(new UserDto());
        }  
    }
}