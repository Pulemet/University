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
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            //var subjects = db.Subjects.Join(db.SubjectsToSemesterInGroup, e => e.Id, o => o.Subject.Id, (e, o) =>
            //                                    new {
            //                                        e.NameFull,
            //                                        e.NameAbridgment,
            //                                        SemesterId = o.Semester.Id,
            //                                        StudentGroupId = o.StudentGroup.Id
            //                                    }).Where(t=>t.SemesterId == 5).Where(t=>t.StudentGroupId == 2).Select(t=>t).ToList();

            //string aa = subjects[0].NameFull;
            //Console.WriteLine(aa);

            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Register");
            }

            return View();
        }

        public ActionResult ShowFriedns(string userId)
        {
            var listFriendsId = db.Friends.Where(t => t.UserOneId == userId);
            List<ApplicationUser> listFriends = new List<ApplicationUser>();

            foreach (var friedId in listFriendsId)
            {
                ApplicationUser user = db.Users.Find(friedId.UserTwoId);
                if(user != null)
                    listFriends.Add(user);
            }
            return View(listFriends);
        }

        [HttpPost]
        public ActionResult SendMessage(Message message)
        {
            var userId = User.Identity.GetUserId();
            message.DateSend = DateTime.Now;
            message.SenderId = db.Users.Find(userId).Id;
            db.Messages.Add(message);
            db.SaveChanges();
            var allMessagesForDialog = db.Messages.Where(m=>m.DialogId == message.DialogId).Select(m=>m).ToList();
            int id = allMessagesForDialog.Last().Id;
            return RedirectToAction("Message", new { messageId = id });
        }

        [HttpGet]
        public ActionResult Message(int messageId)
        {
            var message = db.Messages.Find(messageId);
            return PartialView(message);
        }

        [HttpGet]
        public ActionResult OpenDialog(string recipientId)
        {
            DialogDto dialog = new DialogDto();
            var userId = User.Identity.GetUserId();

            var listDialogs1 = db.UserToDialogs.Where(u => u.UserId == userId).Select(u => u).ToList();
            var listDialogs2 = db.UserToDialogs.Where(u => u.UserId == recipientId).Select(u => u).ToList();

            var dialogId = listDialogs1.Join(listDialogs2, u => u.DialogId, d => d.DialogId, (u, d) => new
            {
                dialogId = u.Id
            }).FirstOrDefault();

            // ---------------------------------------------------------------------------------------------------
            var dialogIdTest = listDialogs1.Join(listDialogs2, u => u.DialogId, d => d.DialogId, (u, d) => new
            {
                dialogId = u.Id
            }).ToList();
            if(dialogIdTest.Count > 1)
                throw new Exception("Несколько одинаковых диалогов");
            // ---------------------------------------------------------------------------------------------------

            if (dialogId != null)
            {
                dialog.Id = dialogId.dialogId;
                dialog.Messages = db.Messages.Where(m => m.DialogId == dialogId.dialogId).Select(m => m).ToList();
            }
            else
            {
                db.Dialogs.Add(new Dialog());
                db.SaveChanges();

                var lislDialogs = db.Dialogs.ToList();
                int lastDialogId = lislDialogs.Last().Id;

                db.UserToDialogs.Add(new UserToDialog() {DialogId = lastDialogId, UserId = userId});
                db.UserToDialogs.Add(new UserToDialog() { DialogId = lastDialogId, UserId = recipientId });
                db.SaveChanges();

                dialog.Id = lastDialogId;
                dialog.Messages = new List<Message>();
            }
            return RedirectToAction("DialogPage", dialog);
        }

        public ActionResult DialogPage(DialogDto dialog)
        {
            return View(dialog);
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