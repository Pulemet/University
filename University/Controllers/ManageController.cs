using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using University.Models;
using University.Models.Helper;
using University.Models.Tables;

namespace University.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public void ChangeAvatar()
        {
            if (Request.Files.Count != 0 && Request.Files[0] != null)
            {
                var image = Request.Files[0];
                var user = db.Users.Find(User.Identity.GetUserId());

                FileInfo file = new FileInfo(Server.MapPath(user.Photo));
                if (file.Exists)
                    file.Delete();

                var fileName = user.FirstName.GetHashCode() + "-" +
                                   user.BirthDate.GetHashCode() + "-" +
                                   user.Email.GetHashCode() + ".jpg";
                var saveFile = Server.MapPath(ConstDictionary.AVATARS_FOLDER + fileName);
                image.SaveAs(saveFile);
                fileName = ConstDictionary.AVATARS_FOLDER + fileName;
                user.Photo = fileName;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ваш пароль изменен."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            var user = db.Users.Find(userId);
            ViewBag.SubjectName = user.Photo == "" ? ConstDictionary.NO_IMAGE : user.Photo;

            var role = db.Roles.Where(r => r.Name == ConstDictionary.ROLE_TEACHER).Select(r => r).FirstOrDefault();
            ViewBag.IsTeacher = user.Roles.Select(r => r.RoleId).Contains(role.Id);
            if (ViewBag.IsTeacher)
            {
                var mySubjects =
                    db.Subjects.Join((db.TeacherToSubjects.Where(ts => ts.TeacherId == userId).Select(ts => ts)),
                        s => s.Id, ts => ts.SubjectId, (s, ts) => s);
                ViewData["MySubjects"] = mySubjects.ToList();
                var otherSubjects = db.Subjects.Except(mySubjects);
                SelectList otherSubjectsList = new SelectList(otherSubjects, "Id", "NameAbridgment");
                ViewData["Subjects"] = otherSubjectsList;
            }
            return View(model);
        }


        [HttpGet]
        public ActionResult AddSubject(int id)
        {
            var userId = User.Identity.GetUserId();
            TeacherToSubject teacherToSubject = new TeacherToSubject() { SubjectId = id, TeacherId = userId };
            db.TeacherToSubjects.Add(teacherToSubject);
            db.SaveChanges();
            var mySubjects = db.Subjects.Join((db.TeacherToSubjects.Where(ts => ts.TeacherId == userId).Select(ts => ts)),
                                                                          s => s.Id, ts => ts.SubjectId, (s, ts) => s);
            var otherSubjects = db.Subjects.Except(mySubjects).ToList();
            return PartialView("GetSubjects", otherSubjects);
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
        }

#endregion
    }
}