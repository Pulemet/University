using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
            var listDialogs = db.Dialogs.Join(db.UserToDialogs.Where(d => d.UserId == currentUserId), d => d.Id, i => i.DialogId, (d, i) => d).ToList();
            var usersDialogs = db.UserToDialogs.Join(db.UserToDialogs.Where(d => d.UserId == currentUserId), u1 => u1.DialogId, u2 => u2.DialogId, (u1, u2) => u1)
                .GroupBy(u => u.DialogId, (key, elements) => new { Key = key, Count = elements.Distinct().Count() })
                .Where(d => d.Count == 2).Join(db.Dialogs, i => i.Key, d => d.Id, (i, d) => d).Where(d => d.IsConversation == false)
                .Join(db.UserToDialogs, d => d.Id, ud => ud.DialogId, (d, ud) => ud)
                .Join(db.Users, ud => ud.UserId, u => u.Id, (ud, u) => new { User = u, DialogId = ud.DialogId }).ToList();

            List<DialogDto> dialogs = new List<DialogDto>();

            var listRelationsUsers = db.Friends.Where(t => t.UserOneId == currentUserId || t.UserTwoId == currentUserId).Select(t => t);
            var listFriends = db.Users.Join(listRelationsUsers, u => u.Id, f => f.UserOneId == currentUserId ? f.UserTwoId : f.UserOneId, (u, f) => u).
                                       OrderBy(u => u.SurName).ToList();

            ViewData["Friends"] = listFriends;

            foreach (var dialog in listDialogs)
            {
                DialogDto dialogDto = new DialogDto();

                dialogDto.Id = dialog.Id;
                dialogDto.IsConversation = dialog.IsConversation;

                if (!dialog.IsConversation)
                {
                    List<ApplicationUser> membersToDialog = usersDialogs.Where(u => u.DialogId == dialog.Id).Select(u => u.User).ToList();
                    dialogDto.Name = GetDialogName(membersToDialog);
                }
                    else
                {
                    dialogDto.Name = dialog.Name;
                }
                
                dialogs.Add(dialogDto);
            }
            return View(dialogs);
        }

        [HttpGet]
        public JsonResult GetDialog(int id)
        {
            var dialog = db.Dialogs.Find(id);
            return Json(GetDialogDto(dialog), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetViewDialogByUser(string id)
        {
            bool isNewDialog = false;
            var dialog = GetDialogByUser(id, ref isNewDialog);       
            DialogJson dialogJson = new DialogJson(GetDialogDto(dialog));
            dialogJson.IsNewDialog = isNewDialog;

            return Json(dialogJson, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult NewConversation(List<string> listUsersId, string nameConversation)
        {
            string currentUserId = User.Identity.GetUserId();
            var users = db.Users.Join(listUsersId, u => u.Id, i => i, (u, i) => u).Where(u => u.Id != currentUserId).ToList();
            Dialog dialog = new Dialog() { IsConversation = true, Name = nameConversation};
            db.Dialogs.Add(dialog);
            db.SaveChanges();
            var allDialogs = db.Dialogs.ToList();
            int dialogId = allDialogs.Last().Id;
            listUsersId.Add(currentUserId);
            var usersToDialog = listUsersId.Select(i => new UserToDialog() { DialogId = dialogId, UserId = i }).ToList();
            db.UserToDialogs.AddRange(usersToDialog);
            db.SaveChanges();

            DialogDto dialogDto = new DialogDto();
            dialogDto.Id = dialogId;
            dialogDto.IsConversation = true;
            dialogDto.Name = nameConversation;
            dialogDto.Members = users;

            return Json(dialogDto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendMessageInDialog(Message msg)
        {
            var userId = User.Identity.GetUserId();
            msg.DateSend = DateTime.Now;
            msg.SenderId = userId;
            db.Messages.Add(msg);
            db.SaveChanges();
            var allMsg = db.Messages.ToList();
            int id = allMsg.Last().Id;
            return Json(GetMessage(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void SendMessage(string id, string text)
        {
            var userId = User.Identity.GetUserId();
            bool isNewDialog = false;
            var dialog = GetDialogByUser(id, ref isNewDialog);

            Message msg = new Message();
            msg.DialogId = dialog.Id;
            msg.DateSend = DateTime.Now;
            msg.SenderId = userId;
            msg.Text = text;
            db.Messages.Add(msg);
            db.SaveChanges();
        }

        //---------------------------------------------------------------------------------------------------------------------

        private string GetDialogName(List<ApplicationUser> users)
        {
            return users[0].Id == User.Identity.GetUserId()
                   ? users[1].SurName + " " + users[1].FirstName
                   : users[0].SurName + " " + users[0].FirstName;
        }

        private Dialog GetDialogByUser(string id, ref bool isNewDialog)
        {
            var userId = User.Identity.GetUserId();

            var listDialogs1 = db.UserToDialogs.Where(u => u.UserId == userId).Select(u => u);
            var listDialogs2 = db.UserToDialogs.Where(u => u.UserId == id).Select(u => u);

            var dialogs = listDialogs1.Join(listDialogs2, u => u.DialogId, d => d.DialogId, (u, d) => u);

            Dialog dialog = db.Dialogs.Join(dialogs, d1 => d1.Id, d2 => d2.DialogId, (d1, d2) => d1)
                                    .FirstOrDefault(d => d.IsConversation == false);

            if (dialog == null)
            {
                isNewDialog = true;
                db.Dialogs.Add(new Dialog() { IsConversation = false, Name = "" });
                db.SaveChanges();

                var lislDialogs = db.Dialogs.ToList();
                dialog = lislDialogs.Last();
                int lastDialogId = dialog.Id;

                db.UserToDialogs.Add(new UserToDialog() { DialogId = lastDialogId, UserId = userId });
                db.UserToDialogs.Add(new UserToDialog() { DialogId = lastDialogId, UserId = id });
                db.SaveChanges();
            }

            return dialog;
        }

        private DialogDto GetDialogDto(Dialog dialog)
        {
            string currentUserId = User.Identity.GetUserId();
            DialogDto dialogDto = new DialogDto();

            dialogDto.IsConversation = dialog.IsConversation;
            dialogDto.Id = dialog.Id;
            var dialogUsers = db.UserToDialogs.Where(ud => ud.DialogId == dialog.Id);
            dialogDto.Members = db.Users.Join(dialogUsers, u => u.Id, d => d.UserId, (u, d) => u).Where(u => u.Id != currentUserId).ToList();

            dialogDto.Name = dialog.IsConversation ? dialog.Name : GetDialogName(dialogDto.Members);

            var messages = db.Messages.Where(m => m.DialogId == dialog.Id)
                                              .Join(db.Users, m => m.SenderId, u => u.Id, (m, u) =>
                                                  new
                                                  { 
                                                      Id = m.Id,
                                                      DateSend = m.DateSend,
                                                      Text = m.Text,
                                                      FirstName = u.FirstName,
                                                      SurName = u.SurName
                                                  }).ToList();

            foreach (var message in messages)
            {
                dialogDto.Messages.Add(new MessageDto()
                {
                    Id = message.Id,
                    DateSend = GetStringDate(message.DateSend),
                    Text = message.Text,
                    FirstName = message.FirstName,
                    SurName = message.SurName
                });
            }

            return dialogDto;
        }

        public string GetStringDate(DateTime date)
        {
            return string.Format("{0:dd.MM.yyyy} в {0:H.mm}", date);
        }

        private MessageDto GetMessage(int msgId)
        {
            var msg = db.Messages.Find(msgId);
            MessageDto msgDto = new MessageDto();

            if (msg != null)
            {
                var user = db.Users.Find(msg.SenderId);
                msgDto = new MessageDto(msg);
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