using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Provider;
using University.Models;
using University.Models.Dto;
using University.Models.Helper;
using University.Models.Tables;

namespace University.Controllers
{
    public class SubjectsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Subjects
        public ActionResult Index()
        {
            return View(db.Subjects.ToList());
        }

        [HttpGet]
        public ActionResult MaterialsSubject(int id)
        {
            ViewBag.SubjectName = db.Subjects.Find(id) != null ? db.Subjects.Find(id).NameFull : "Undefined";
            return View(SelectMaterials(db.Materials.Where(m => m.SubjectId == id).Select(m => m).ToList()));
        }

        [HttpPost]
        public ActionResult AddMaterial()
        {
            Material material = new Material();
            material.DateLoad = DateTime.Now;
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
                var saveFile = Server.MapPath(ConstDictionary.MATERIALS_FOLDER + fileName);
                loadFile.SaveAs(saveFile);
                material.FileLink = ConstDictionary.MATERIALS_FOLDER + fileName;
            }

            db.Materials.Add(material);
            db.SaveChanges();
            var newMaterial = db.Materials.ToList().Last();

            MaterialDto mDto = new MaterialDto(newMaterial);
            ApplicationUser user = db.Users.Find(newMaterial.AuthorId);
            mDto.FirstName = user.FirstName;
            mDto.SurName = user.SurName;

            return PartialView("PartialViewMaterial", mDto);
        }

        public ActionResult Material(int id)
        {
            Material material = db.Materials.Find(id);
            MaterialDto mDto = new MaterialDto(material);
            if (material != null)
            {
                ApplicationUser user = db.Users.Find(material.AuthorId);
                mDto.FirstName = user.FirstName;
                mDto.SurName = user.SurName;
            }
            var comments = db.MaterialComments.Where(c => c.MaterialId == material.Id).Select(c => c).ToList();
            foreach (var comment in comments)
            {
                mDto.Comments.Add(GetComment(comment));
            }
            return View(mDto);
        }

        [HttpPost]
        public ActionResult AddComment(MaterialComment comment)
        {
            var userId = User.Identity.GetUserId();
            comment.DateAdd = DateTime.Now;
            comment.AuthorId = userId;
            db.MaterialComments.Add(comment);
            db.SaveChanges();
            var allComments = db.MaterialComments.ToList();
            int id = allComments.Last().Id;
            return RedirectToAction("Comment", new { commentId = id });
        }

        [HttpGet]
        public ActionResult Comment(int commentId)
        {
            return PartialView(GetComment(db.MaterialComments.Find(commentId)));
        }

        public List<MaterialDto> SelectMaterials(List<Material> listMaterials)
        {
            List<MaterialDto> materialsDto = new List<MaterialDto>();
            foreach (var material in listMaterials)
            {
                MaterialDto mDto = new MaterialDto(material);
                ApplicationUser user = db.Users.Find(material.AuthorId);
                mDto.FirstName = user.FirstName;
                mDto.SurName = user.SurName;
                materialsDto.Add(mDto);
            }
            return materialsDto;
        }

        public CommentDto GetComment(MaterialComment comment)
        {
            CommentDto commentDto = new CommentDto();

            if (comment != null)
            {
                var user = db.Users.Find(comment.AuthorId);
                commentDto.Id = comment.Id;
                commentDto.DateAdd = comment.DateAdd;
                commentDto.Text = comment.Text;
                commentDto.FirstName = user.FirstName;
                commentDto.SurName = user.SurName;
            }

            return commentDto;
        }
    }    
}