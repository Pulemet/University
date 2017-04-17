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
    public class QuestionsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private string _materialsForQuestionsFolder = "/Files/MaterialsForQuestions/";
        // GET: Questions
        public ActionResult Index()
        {
            return View(db.Questions.ToList());
        }

        [HttpGet]
        public ActionResult Questions(int id)
        {
            ViewBag.SubjectName = db.Subjects.Find(id) != null ? db.Subjects.Find(id).NameFull : "Undefined";
            return View(SelectQuestions(db.Questions.Where(q => q.SubjectId == id).Select(q => q).ToList()));
        }

        public ActionResult NewQuestion()
        {
            return View(db.Subjects.ToList());
        }

        public List<QuestionDto> SelectQuestions(List<Question> listQuestions)
        {
            List<QuestionDto> questionsDto = new List<QuestionDto>();
            foreach (var question in listQuestions)
            {
                QuestionDto qDto = new QuestionDto(question);
                ApplicationUser user = db.Users.Find(question.AuthorId);
                qDto.FirstName = user.FirstName;
                qDto.SurName = user.SurName;
                questionsDto.Add(qDto);
            }
            return questionsDto;
        }

        public ActionResult Question(int id)
        {
            Question question = db.Questions.Find(id);
            QuestionDto qDto = new QuestionDto(question);
            if (question != null)
            {
                ApplicationUser user = db.Users.Find(question.AuthorId);
                qDto.FirstName = user.FirstName;
                qDto.SurName = user.SurName;
            }
            var answers = db.Answers.Where(a => a.QuestionId == question.Id).Select(a => a).ToList();
            foreach (var answer in answers)
            {
                qDto.Answers.Add(GetAnswer(answer));
            }

            return View(qDto);
        }

        [HttpPost]
        public ActionResult AddQuestion(Question question, HttpPostedFileBase file = null)
        {
            question.AuthorId = User.Identity.GetUserId();
            question.CreateDate = DateTime.Now;

            if (file != null)
            {
                string fileName = question.Topic.GetHashCode() + "-" +
                           question.AuthorId.GetHashCode() + "-" +
                           question.GetHashCode() + "." + file.FileName.Split('.').LastOrDefault();
                var saveFile = Server.MapPath(_materialsForQuestionsFolder + fileName);
                file.SaveAs(saveFile);
                question.FileLink = _materialsForQuestionsFolder + fileName;
            }

            db.SaveChanges();
            return RedirectToAction("Questions", new { id = question.SubjectId });
        }

        [HttpPost]
        public ActionResult AddAnswer(Answer answer)
        {
            var userId = User.Identity.GetUserId();
            answer.CreateDate = DateTime.Now;
            answer.AuthorId = userId;
            db.Answers.Add(answer);
            db.SaveChanges();
            var allAnswers = db.Answers.ToList();
            int id = allAnswers.Last().Id;
            return RedirectToAction("Answer", new { answerId = id });
        }

        [HttpGet]
        public ActionResult Answer(int answerId)
        {
            return PartialView(GetAnswer(db.Answers.Find(answerId)));
        }

        public AnswerDto GetAnswer(Answer answer)
        {
            AnswerDto answerDto = new AnswerDto();

            if (answer != null)
            {
                var user = db.Users.Find(answer.AuthorId);
                answerDto.Id = answer.Id;
                answerDto.CreateDate = answer.CreateDate;
                answerDto.Text = answer.Text;
                answerDto.FirstName = user.FirstName;
                answerDto.SurName = user.SurName;
            }

            return answerDto;
        }
    }
}