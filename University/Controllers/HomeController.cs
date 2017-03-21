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

        [HttpGet]
        public ActionResult OpenDialog(string id)
        {
            // id = message recipient id

            var userId = User.Identity.GetUserId();

            var listDialogs1 = db.UserToDialogs.Where(u => u.UserId == userId).Select(u => u).ToList();
            var listDialogs2 = db.UserToDialogs.Where(u => u.UserId == id).Select(u => u).ToList();

            var dialogId = listDialogs1.Join(listDialogs2, u => u.DialogId, d => d.DialogId, (u, d) => new
            {
                dialogId = u.DialogId
            }).FirstOrDefault();

            // ---------------------------------------------------------------------------------------------------
            var dialogIdTest = listDialogs1.Join(listDialogs2, u => u.DialogId, d => d.DialogId, (u, d) => new
            {
                dialogId = u.Id
            }).ToList();

            if (dialogIdTest.Count > 1)
                throw new Exception("Несколько одинаковых диалогов");
            // ---------------------------------------------------------------------------------------------------

            if (dialogId == null)
            {
                db.Dialogs.Add(new Dialog());
                db.SaveChanges();

                var lislDialogs = db.Dialogs.ToList();
                int lastDialogId = lislDialogs.Last().Id;

                db.UserToDialogs.Add(new UserToDialog() { DialogId = lastDialogId, UserId = userId });
                db.UserToDialogs.Add(new UserToDialog() { DialogId = lastDialogId, UserId = id });
                db.SaveChanges();
            }

            return RedirectToAction("DialogPage", new { id = dialogId != null ? dialogId.dialogId : 0 });
        }

        public ActionResult DialogPage(int id)
        {
            DialogDto dialog = new DialogDto();

            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (id != 0)
            {
                dialog.Id = id;
                var messagesInDialog = db.Messages.Where(m => m.DialogId == id).Select(m => m).ToList();
                if (messagesInDialog.Count != 0)
                {
                    foreach (var msg in messagesInDialog)
                    {
                        MessageDto msgDto = new MessageDto();

                        if (msg != null)
                        {
                            msgDto.Id = msg.Id;
                            msgDto.DateSend = msg.DateSend;
                            msgDto.Text = msg.Text;
                            msgDto.SenderId = msg.SenderId;
                            var sender = db.Users.Find(msg.SenderId);
                            msgDto.FirstName = sender.FirstName;
                            msgDto.SurName = sender.SurName;
                        }

                        dialog.Messages.Add(msgDto);
                    }
                }
            }
            else
            {
                var lislDialogs = db.Dialogs.ToList();
                int lastDialogId = lislDialogs.Last().Id;

                dialog.Id = lastDialogId;

            }

            return View(dialog);
        }

        [HttpPost]
        public ActionResult SendMessage(Message msg)
        {
            var userId = User.Identity.GetUserId();
            msg.DateSend = DateTime.Now;
            msg.SenderId = userId;
            db.Messages.Add(msg);
            db.SaveChanges();
            var allMsg = db.Messages.ToList();
            int id = allMsg.Last().Id;
            return RedirectToAction("Message", new { msgId = id });
        }

        [HttpGet]
        public ActionResult Message(int msgId)
        {
            var msg = db.Messages.Find(msgId);
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            MessageDto msgDto = new MessageDto();

            if (msg != null)
            {
                msgDto.Id = msg.Id;
                msgDto.DateSend = msg.DateSend;
                msgDto.Text = msg.Text;
                msgDto.SenderId = msg.SenderId;
                msgDto.FirstName = user.FirstName;
                msgDto.SurName = user.SurName;
            }
            
            return PartialView(msgDto);
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