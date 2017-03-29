using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using University.Models;

namespace University.Controllers
{
    public class SubjectsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Subjects
        public ActionResult Index()
        {
            var subjects = db.Subjects.ToList();
            return View(subjects);
        }
    }
}