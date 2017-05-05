using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using University.Models;
using University.Models.Dto;
using University.Models.Tables;

namespace University.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Teachers
        public ActionResult Index(int id)
        {
            var subject = _db.Subjects.Find(id);
            ViewData["Subject"] = subject.NameFull;
            return View(GetTeachersToSubject(subject.Id));
        }

        private List<TeacherDto> GetTeachersToSubject(int id)
        {
            var listTeahers =
                _db.TeacherToSubjects.Where(t => t.SubjectId == id)
                    .Join(_db.Users, t => t.TeacherId, u => u.Id, (t, u) => u)
                    .ToList();

            return GetTeachersDto(listTeahers);
        }

        private List<TeacherDto> GetTeachersDto(List<ApplicationUser> users)
        {
            List<TeacherDto> teachersDto = new List<TeacherDto>();
            var subjectsToteacher =
                users.Join(_db.TeacherToSubjects, u => u.Id, t => t.TeacherId, (u, t) => t)
                .Join(_db.Subjects, t => t.SubjectId, s => s.Id, (t, s) => new
                    {
                        Id = t.TeacherId,
                        Subject = s
                    }).ToList();

            foreach (var user in users)
            {
                TeacherDto teacher = new TeacherDto(user);
                teacher.Subjects = subjectsToteacher.Where(s => s.Id == teacher.Id).Select(s => s.Subject).ToList();
                var department = _db.Departments.Find(user.GroupId);
                teacher.Department = department.NameAbridgment;
                teachersDto.Add(teacher);
            }

            return teachersDto;
        }

        private TeacherDto GetTeacherDto(ApplicationUser user)
        {
            TeacherDto teacher = new TeacherDto(user);
            teacher.Subjects = _db.TeacherToSubjects.Where(t => t.TeacherId == user.Id)
                .Join(_db.Subjects, t => t.SubjectId, s => s.Id, (t, s) => s).ToList();
            var department = _db.Departments.Find(user.GroupId);
            teacher.Department = department.NameAbridgment;
            teacher.Comments = CommentsToTeacherDto(user.Id);
            return teacher;
        }

        private List<CommentToTeacherDto> CommentsToTeacherDto(string id)
        {
            return _db.CommentsToTeacher.Where(c => c.TeacherId == id).
                Join(_db.Users, c => c.AuthorId, u => u.Id, (c, u) => new CommentToTeacherDto()
                {
                    Id = c.Id,
                    DateAdd = c.DateAdd,
                    Text = c.Text,
                    FirstName = u.FirstName,
                    SurName = u.SurName
                }).ToList();
        }

        public CommentToTeacherDto GetComment(CommentToTeacher comment)
        {
            CommentToTeacherDto commentDto = new CommentToTeacherDto();

            if (comment != null)
            {
                var user = _db.Users.Find(comment.AuthorId);
                commentDto.Id = comment.Id;
                commentDto.DateAdd = comment.DateAdd;
                commentDto.Text = comment.Text;
                commentDto.FirstName = user.FirstName;
                commentDto.SurName = user.SurName;
            }

            return commentDto;
        }

        public ActionResult TeacherPage(string id)
        {
            var user = _db.Users.Find(id);
            var currentUserId = User.Identity.GetUserId();

            ViewData["CurrentUser"] = currentUserId == id;

            return View(GetTeacherDto(user));
        }

        [HttpPost]
        public ActionResult AddReview(CommentToTeacher review)
        {
            var userId = User.Identity.GetUserId();
            review.DateAdd = DateTime.Now;
            review.AuthorId = userId;
            _db.CommentsToTeacher.Add(review);
            _db.SaveChanges();
            var allReviews = _db.CommentsToTeacher.ToList();
            int id = allReviews.Last().Id;
            return RedirectToAction("Review", new { commentId = id });
        }

        [HttpGet]
        public ActionResult Review(int commentId)
        {
            return PartialView(GetComment(_db.CommentsToTeacher.Find(commentId)));
        }
    }
}