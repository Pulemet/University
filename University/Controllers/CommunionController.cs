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
            var listDialogId = db.UserToDialogs.Where(d => d.UserId == currentUserId).Select(d => d.DialogId);
            var listDialogs = db.Dialogs.Join(listDialogId, d => d.Id, i => i, (d, i) => d).ToList();
            List<DialogDto> dialogs = new List<DialogDto>();

            var listRelationsUsers = db.Friends.Where(t => t.UserOneId == currentUserId || t.UserTwoId == currentUserId).Select(t => t);
            var listFriends = db.Users.Join(listRelationsUsers, u => u.Id, f => f.UserOneId == currentUserId ? f.UserTwoId : f.UserOneId, (u, f) => u).
                                       OrderBy(u => u.SurName).ToList();

            ViewData["Friends"] = listFriends;

            foreach (var dialog in listDialogs)
            {
                var listUserId = db.UserToDialogs.Where(u => u.DialogId == dialog.Id).Select(u => u.UserId).ToList();
                DialogDto dialogDto = new DialogDto();
                List<ApplicationUser> membersToDialog = db.Users.Join(listUserId, u => u.Id, i => i, (u, i) => u).ToList();
                dialogDto.Id = dialog.Id;
                dialogDto.Members = membersToDialog;
                dialogDto.IsConversation = dialog.IsConversation;
                dialogDto.Name = GetDialogName(dialog.IsConversation, dialogDto.Members, dialog.Name);     
                dialogs.Add(dialogDto);
            }
            return View(dialogs);
        }

        [HttpGet]
        public ActionResult GetDialog(int id)
        {
            var dialog = db.Dialogs.Find(id);
            return PartialView("Dialog", GetDialogDto(dialog));
        }

        [HttpGet]
        public ActionResult GetViewDialogByUser(string id)
        {
            var dialog = GetDialogByUser(id);
            return PartialView("Dialog", GetDialogDto(dialog));
        }

        [HttpPost]
        public ActionResult NewConversation(List<string> listUsersId, string nameConversation)
        {
            var users = db.Users.Join(listUsersId, u => u.Id, i => i, (u, i) => u).ToList();
            Dialog dialog = new Dialog() { IsConversation = true, Name = nameConversation};
            db.Dialogs.Add(dialog);
            db.SaveChanges();
            var allDialogs = db.Dialogs.ToList();
            int dialogId = allDialogs.Last().Id;
            string currentUserId = User.Identity.GetUserId();
            listUsersId.Add(currentUserId);
            foreach (var userId in listUsersId)
            {
                UserToDialog userToDialog = new UserToDialog() { DialogId = dialogId, UserId = userId};
                db.UserToDialogs.Add(userToDialog);
                db.SaveChanges();
            }

            DialogDto dialogDto = new DialogDto();
            dialogDto.Id = dialogId;
            dialogDto.IsConversation = true;
            dialogDto.Name = nameConversation;
            dialogDto.Members = users;

            return PartialView("PartialViewDialog", dialogDto);
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

            var dialog = GetDialogByUser(id);

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

        //---------------------------------------------------------------------------------------------------------------------

        private string GetDialogName(bool isConversation, List<ApplicationUser> users, string name)
        {
            return isConversation ? name : (users.Count == 2 ? (users[0].Id == User.Identity.GetUserId()
                                                             ? users[1].SurName + " " + users[1].FirstName
                                                             : users[0].SurName + " " + users[0].FirstName) : "Undefined");
        }

        private Dialog GetDialogByUser(string id)
        {
            var userId = User.Identity.GetUserId();

            var listDialogs1 = db.UserToDialogs.Where(u => u.UserId == userId).Select(u => u);
            var listDialogs2 = db.UserToDialogs.Where(u => u.UserId == id).Select(u => u);

            var dialogs = listDialogs1.Join(listDialogs2, u => u.DialogId, d => d.DialogId, (u, d) => u);

            return db.Dialogs.Join(dialogs, d1 => d1.Id, d2 => d2.DialogId, (d1, d2) => d1)
                                    .FirstOrDefault(d => d.IsConversation == false);
        }

        private DialogDto GetDialogDto(Dialog dialog)
        {
            DialogDto dialogDto = new DialogDto();

            dialogDto.IsConversation = dialog.IsConversation;
            dialogDto.Id = dialog.Id;
            var dialogUsers = db.UserToDialogs.Where(ud => ud.DialogId == dialog.Id);
            dialogDto.Members = db.Users.Join(dialogUsers, u => u.Id, d => d.UserId, (u, d) => u).ToList();

            dialogDto.Name = GetDialogName(dialog.IsConversation, dialogDto.Members, dialog.Name);

            var messagesInDialog = db.Messages.Where(m => m.DialogId == dialog.Id).Select(m => m).ToList();
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
            return dialogDto;
        }

        private MessageDto GetMessage(int msgId)
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