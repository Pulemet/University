using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Provider;
using University.Models;
using University.Models.Tables;

namespace University.Controllers
{
    public class SubjectsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private string _materialsFolder = "/Files/Materials/";
        // GET: Subjects
        public ActionResult Index()
        {
            var subjects = db.Subjects.ToList();
            return View(subjects);
        }

        [HttpGet]
        public ActionResult MaterialsSubject(int id)
        {
            var materials = db.Materials.Where(m => m.SubjectId == id).Select(m=>m).ToList();
            return View(materials);
        }

        [HttpPost]
        public ActionResult PartialViewMaterials()
        {
            Material material = new Material();
            material.AuthorId = User.Identity.GetUserId();
            material.SubjectId = Int32.Parse(Request.Form[0]);
            material.Name = Request.Form[1];
            material.TypeLesson = Request.Form[2];
            
            if (Request.Files.Count != 0 && Request.Files[0] != null)
            {
                var loadFile = Request.Files[0];
                string fileName = material.Name.GetHashCode() + "-" +
                           material.AuthorId.GetHashCode() + "-" +
                           material.GetHashCode() + "." + loadFile.FileName.Split('.').LastOrDefault(); 
                var saveFile = Server.MapPath(_materialsFolder + fileName);
                loadFile.SaveAs(saveFile);
                material.FileLink = _materialsFolder + fileName;
            }

            db.Materials.Add(material);
            db.SaveChanges();
            return PartialView(db.Materials.Where(m => m.SubjectId == material.SubjectId));
        }
    }    
}