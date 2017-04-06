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
            return View(SelectMaterials(db.Materials.Where(m => m.SubjectId == id).Select(m => m).ToList()));
        }

        [HttpPost]
        public ActionResult PartialViewMaterials()
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
                var saveFile = Server.MapPath(_materialsFolder + fileName);
                loadFile.SaveAs(saveFile);
                material.FileLink = _materialsFolder + fileName;
            }

            db.Materials.Add(material);
            db.SaveChanges();
            return PartialView(SelectMaterials(db.Materials.Where(m => m.SubjectId == material.SubjectId).Select(m => m).ToList()));
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
               CommentDto commentDto = new CommentDto();

                if (comment != null)
                {
                    commentDto.Id = comment.Id;
                    commentDto.DateAdd = comment.DateAdd;
                    commentDto.Text = comment.Text;
                    var author = db.Users.Find(comment.AuthorId);
                    commentDto.FirstName = author.FirstName;
                    commentDto.SurName = author.SurName;
                }

                mDto.Comments.Add(commentDto);
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
            var comment = db.MaterialComments.Find(commentId);
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

            return PartialView(commentDto);
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
    }    
}