using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using University.Models;
using University.Models.Dto;

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
            List<DialogToUsersDto> dialogs = new List<DialogToUsersDto>();

            foreach (var dialogId in listDialogId)
            {
                var listUserId = db.UserToDialogs.Where(u => u.DialogId == dialogId).Select(u => u.UserId).ToList();
                DialogToUsersDto dialog = new DialogToUsersDto();
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
    }
}