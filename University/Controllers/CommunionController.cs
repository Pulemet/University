using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using University.Models;
using University.Models.Dto;
using University.Models.Tables;

namespace University.Controllers
{
    public class CommunionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Communion
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            var listDialogId = db.UserToDialogs.Where(d => d.UserId == currentUserId).Select(d=>d.DialogId).ToList();
            List<DialogDto> dialogs = new List<DialogDto>();

            var listRelationsUsers = db.Friends.Where(t => t.UserOneId == currentUserId || t.UserTwoId == currentUserId).Select(t => t);
            var listFriends = db.Users.Join(listRelationsUsers, u => u.Id, f => f.UserOneId == currentUserId ? f.UserTwoId : f.UserOneId, (u, f) => u).
                                       OrderBy(u => u.SurName).ToList();

            ViewData["Friends"] = listFriends;

            foreach (var dialogId in listDialogId)
            {
                var listUserId = db.UserToDialogs.Where(u => u.DialogId == dialogId).Select(u => u.UserId).ToList();
                DialogDto dialog = new DialogDto();
                List<ApplicationUser> membersToDialog = new List<ApplicationUser>();
                foreach (var userId in listUserId)
                {
                    if (userId != currentUserId)
                    {
                        ApplicationUser member = db.Users.Find(userId);
                        membersToDialog.Add(member);
                    }            
                }
                dialog.Id = dialogId;
                dialog.Members = membersToDialog;
                dialogs.Add(dialog);
            }
            return View(dialogs);
        }

        [HttpGet]
        public ActionResult GetDialog(int id)
        {
            DialogDto dialogDto = new DialogDto();
            var dialog = db.Dialogs.Find(id);

            dialogDto.IsConversation = dialog.IsConversation;            
            dialogDto.Id = id;
            var dialogUsers = db.UserToDialogs.Join(db.Dialogs, u => u.DialogId, d => d.Id, (u, d) => u);
            dialogDto.Members = db.Users.Join(dialogUsers, u => u.Id, d => d.UserId, (u, d) => u).ToList();

            dialogDto.Name = dialog.IsConversation ? dialog.Name : (dialogDto.Members.Count == 2 ? (dialogDto.Members[0].Id == User.Identity.GetUserId()
                                                                    ? dialogDto.Members[0].SurName + dialogDto.Members[0].FirstName
                                                                    : dialogDto.Members[1].SurName + dialogDto.Members[1].FirstName) : "Undefined" );

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
                        var sender = db.Users.Find(msg.SenderId);
                        msgDto.FirstName = sender.FirstName;
                        msgDto.SurName = sender.SurName;
                    }

                    dialogDto.Messages.Add(msgDto);
                }
            }

            return PartialView("Dialog", dialogDto);
        }

        [HttpPost]
        public ActionResult SendMessageInDialog(Message msg)
        {
            var userId = User.Identity.GetUserId();
            msg.DateSend = DateTime.Now;
            msg.SenderId = userId;
            db.Messages.Add(msg);
            db.SaveChanges();
            var allMsg = db.Messages.ToList();
            int id = allMsg.Last().Id;
            return PartialView("Message", GetMessage(id));
        }

        [HttpPost]
        public void SendMessage(string id, string text)
        {
            var userId = User.Identity.GetUserId();

            var listDialogs1 = db.UserToDialogs.Where(u => u.UserId == userId).Select(u => u).ToList();
            var listDialogs2 = db.UserToDialogs.Where(u => u.UserId == id).Select(u => u).ToList();

            var dialogs = listDialogs1.Join(listDialogs2, u => u.DialogId, d => d.DialogId, (u, d) => u);

            var dialog = db.Dialogs.Join(dialogs, d1 => d1.Id, d2 => d2.DialogId, (d1, d2) => d1)
                                   .FirstOrDefault(d => !d.IsConversation);

            int dialogId = dialog != null ? dialog.Id : 0;

            if (dialog == null)
            {
                db.Dialogs.Add(new Dialog() { IsConversation = false, Name = "" });
                db.SaveChanges();

                var lislDialogs = db.Dialogs.ToList();
                int lastDialogId = lislDialogs.Last().Id;
                dialogId = lastDialogId;

                db.UserToDialogs.Add(new UserToDialog() { DialogId = lastDialogId, UserId = userId });
                db.UserToDialogs.Add(new UserToDialog() { DialogId = lastDialogId, UserId = id });
                db.SaveChanges();
            }

            Message msg = new Message();
            msg.DialogId = dialogId;
            msg.DateSend = DateTime.Now;
            msg.SenderId = userId;
            msg.Text = text;
            db.Messages.Add(msg);
            db.SaveChanges();
        }

        public MessageDto GetMessage(int msgId)
        {
            var msg = db.Messages.Find(msgId);
            MessageDto msgDto = new MessageDto();

            if (msg != null)
            {
                var user = db.Users.Find(msg.SenderId);
                msgDto.Id = msg.Id;
                msgDto.DateSend = msg.DateSend;
                msgDto.Text = msg.Text;
                msgDto.FirstName = user.FirstName;
                msgDto.SurName = user.SurName;
            }

            return msgDto;
        }
    }
}

// JSON результат (ПРИМЕР)
//[HttpGet]
//public JsonResult GetResult()
//{
//    return Json(Объект, JsonRequestBehavior.AllowGet);
//}