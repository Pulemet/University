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
            //Если файл не прикреплен, то массив будет больше на 1 элемент
            //и первым элементом будет файл = null
            int isAttachedFileCounter = Request.Form.Count == 3 ? 0 : 1;
            Material material = new Material();
            material.DateLoad = DateTime.Now;
            material.AuthorId = User.Identity.GetUserId();
            material.SubjectId = Int32.Parse(Request.Form[isAttachedFileCounter]);
            material.Name = Request.Form[++isAttachedFileCounter];
            material.TypeLesson = Request.Form[++isAttachedFileCounter];
            
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

            //RedirectToAction("Material", GetMaterialDto(newMaterial));
            return PartialView("PartialViewMaterial", GetMaterialDto(newMaterial));
        }

        public ActionResult Material(int id)
        {
            Material material = db.Materials.Find(id);
            MaterialDto mDto = GetMaterialDto(material);
            mDto.Comments = GetComments(mDto.Id);
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

        private List<MaterialDto> SelectMaterials(List<Material> listMaterials)
        {
            List<MaterialDto> materialsDto = new List<MaterialDto>();
            foreach (var material in listMaterials)
            {
                materialsDto.Add(GetMaterialDto(material));
            }
            return materialsDto;
        }

        private CommentDto GetComment(MaterialComment comment)
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

        public List<CommentDto> GetComments(int id)
        {
            return db.MaterialComments.Where(c => c.MaterialId == id).Join(db.Users,
                c => c.AuthorId, u => u.Id, (c, u) => new CommentDto()
                {
                    Id = c.Id,
                    DateAdd = c.DateAdd,
                    Text = c.Text,
                    FirstName = u.FirstName,
                    SurName = u.SurName
                }).ToList();
        }

        private MaterialDto GetMaterialDto(Material material)
        {
            MaterialDto mDto = new MaterialDto(material);
            if (material != null)
            {
                ApplicationUser user = db.Users.Find(material.AuthorId);
                mDto.FirstName = user.FirstName;
                mDto.SurName = user.SurName;
            }
            return mDto;
        }
    }    
}