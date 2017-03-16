using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using University.Models;
using University.Models.Tables;

namespace University.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var subjects = db.Subjects.Join(db.SubjectsToSemesterInGroup, e => e.Id, o => o.Subject.Id, (e, o) =>
                                                new {
                                                    e.NameFull,
                                                    e.NameAbridgment,
                                                    SemesterId = o.Semester.Id,
                                                    StudentGroupId = o.StudentGroup.Id
                                                }).Where(t=>t.SemesterId == 5).Where(t=>t.StudentGroupId == 2).Select(t=>t).ToList();

            //string aa = subjects[0].NameFull;
            //Console.WriteLine(aa);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}