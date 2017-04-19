using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using University.Models;
using University.Models.Helper;
using University.Models.Tables;

namespace University.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private string _avatarsFolder = "/Files/Avatars/";
        private string _invalidLogin = "Неверный логин или пароль";
        private string _subjectName = "Подтверждение учетной записи";

        [Authorize(Roles = "admin")]
        public ActionResult ConfirmRegistration()
        {
            var users = db.Users.Join(db.AwaitingUsers, u => u.Id, a => a.UserId, (u, a) => u).OrderBy(u => u.SurName);
            return View(users);
        }

        [HttpPost]
        public async Task<string> DeleteUser(string id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                FileInfo file = new FileInfo(Server.MapPath(user.Photo));
                if(file.Exists)
                    file.Delete();
  
                var awaitingUser = db.AwaitingUsers.FirstOrDefault(a => a.UserId == user.Id);
                db.AwaitingUsers.Remove(awaitingUser);
                db.Users.Remove(user);
                db.SaveChanges();

                EmailService emailService = new EmailService();
                var message = String.Format("Уважаемый {0} {1}. Активация вашей учетной записи " +
                                            "была отклонена администратором.", user.SurName,
                                                                               user.FirstName);
                await emailService.SendEmailAsync(user.Email, _subjectName, message);

                return String.Format("Регистрация учетной записи {0} отклонена", user.Email); ;
            }
            return "Ошибка";
        }

        [HttpPost]
        public async Task<string> SubmitUser(string id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                var awaitingUser = db.AwaitingUsers.FirstOrDefault(a => a.UserId == user.Id);
                db.AwaitingUsers.Remove(awaitingUser);
                db.SaveChanges();

                EmailService emailService = new EmailService();
                var message = String.Format("Уважаемый {0} {1}, ваша учетная запись подтверждена.", user.SurName,
                                                                                                    user.FirstName);
                await emailService.SendEmailAsync(user.Email, _subjectName, message);
                return String.Format("Учетная запись {0} активирована", user.Email);
            }
            return "Ошибка";
        }

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
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
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // проверка подтверждения регистрации
            var user = await UserManager.FindAsync(model.Email, model.Password);
            if (user != null)
            {
                var submitRegistration = db.AwaitingUsers.FirstOrDefault(a => a.UserId == user.Id);
                if(submitRegistration != null)
                {
                    ModelState.AddModelError("", "Ваш аккаунт не подтвержден!");
                    return View(model);
                }
            }

            // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", _invalidLogin);
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            SelectList faculties = new SelectList(db.Faculties, "Id", "NameAbridgment");
            ViewData["Faculties"] = faculties;
            SelectList specialities = new SelectList(new List<Speciality>(), "Id", "NameAbridgment");
            ViewData["Specialities"] = specialities;
            SelectList groups = new SelectList(new List<StudentGroup>(), "Id", "Name");
            ViewData["Groups"] = groups;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetSpecialities(string id)
        {
            int facultyId = Int32.Parse(id);
            return PartialView(db.Specialities.Where(s => s.FacultyId == facultyId).ToList());
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetGroups(string id)
        {
            int groupId = Int32.Parse(id);
            return PartialView(db.StudentGroups.Where(g => g.SpecialityId == groupId).ToList());
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                try
                {                 
                    string fileName = "";
                    if (image != null)
                    {
                        fileName = model.FirstName.GetHashCode() + "-" +
                                   model.BirthDate.GetHashCode() + "-" +
                                   model.Email.GetHashCode() + ".jpg";
                        var saveFile = Server.MapPath(_avatarsFolder + fileName);
                        image.SaveAs(saveFile);
                        fileName = _avatarsFolder + fileName;
                    }

                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        SurName = model.SurName,
                        PatronymicName = model.PatronymicName,
                        BirthDate = model.BirthDate,
                        Photo = fileName
                    };

                    // если преподаватель, то группа не будет указана
                    if (model.Group != null)
                        user.GroupId = Int32.Parse(model.Group);

                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // добавление метки пользователя для подтверждения регистрации
                        db.AwaitingUsers.Add(new AwaitingUser() {UserId = user.Id});
                        db.SaveChanges();

                        if (model.Role == "Teacher")
                            await UserManager.AddToRoleAsync(user.Id, "teacher");
                        if (model.Role == "Student")
                            await UserManager.AddToRoleAsync(user.Id, "student");

                        // вход под только созданной учетной записью
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        return RedirectToAction("CompletionRegistration", "Home");
                    }
                    AddErrors(result);
                }
                catch
                {
                    SelectList faculties = new SelectList(db.Faculties, "Id", "NameAbridgment");
                    ViewData["Faculties"] = faculties;
                    SelectList specialities = new SelectList(new List<Speciality>(), "Id", "NameAbridgment");
                    ViewData["Specialities"] = specialities;
                    SelectList groups = new SelectList(new List<StudentGroup>(), "Id", "Name");
                    ViewData["Groups"] = groups;
                    return View(model);
                }
                
            }
            else
            {
                SelectList faculties = new SelectList(db.Faculties, "Id", "NameAbridgment");
                ViewData["Faculties"] = faculties;
                SelectList specialities = new SelectList(new List<Speciality>(), "Id", "NameAbridgment");
                ViewData["Specialities"] = specialities;
                SelectList groups = new SelectList(new List<StudentGroup>(), "Id", "Name");
                ViewData["Groups"] = groups;
                return View(model);
            }
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}