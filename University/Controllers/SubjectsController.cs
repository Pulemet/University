using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Provider;
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

        [HttpPost]
        public ActionResult MaterialsSubject(int id)
        {
            var materials = db.Materials.Where(m => m.SubjectId == id).Select(m=>m).ToList();
            return View(materials);
        }
    }    
}