using System;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using EditPressRu.Filters;
using EditPressRu.Models.DB;
using System.Linq;
using EditPressRu.Controllers;
using EditPressRu.Models;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace Site.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : BaseController
    {
        private EditPressRuEntities Db = new EditPressRuEntities();

        [Authorize]
        public ActionResult Index(string mesage = "")
        {
            ViewBag.Msg = mesage;
            string sessId = Session.SessionID;
            return View();
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
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                //добавим сюда кусок обработки корзины
                try
                {
                    int userId = Db.UserProfile.SingleOrDefault(x=>x.UserName==model.UserName).UserId;
                   ViewBag.OrderId= HelpFunctions.UpdateCartLogin(userId,ViewBag.OrderId);
                }
                catch 
                {
                }

                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Имя пользователя или пароль не найдены.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult logoff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    //WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    // WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { EmailId = model.EmailId, Details = model.Details });
                    WebSecurity.CreateUserAndAccount(
                    model.UserName, model.Password,
                    new { Pass = model.Password });
                    Roles.AddUserToRoles(model.UserName, new[] { "User" });
                    WebSecurity.Login(model.UserName, model.Password);

                    #region UserProfileDetails

                    int userId = Db.UserProfile.FirstOrDefault(x => x.UserName.Equals(model.UserName)).UserId;

                    UserProfileDetails rec = new UserProfileDetails();

                    rec.DiscountID = 1;
                    rec.ShipmentAddress = "";
                    rec.UserID = userId;
                    rec.INN = "";
                    rec.OrgName = "";
                    rec.FIO = "";
                    rec.Phone = "";
                    if (model.UserName.Contains('@') && model.UserName.Contains('.'))
                    {
                        rec.email = model.UserName;
                    }
                    else
                    {
                        rec.email = "";
                    }

                    Db.UserProfileDetails.Add(rec);
                    Db.SaveChanges();
                    #endregion

                    //добавим сюда кусок обработки корзины
                    try
                    {
                        ViewBag.OrderId = HelpFunctions.UpdateCartLogin(userId, ViewBag.OrderId);
                    }
                    catch
                    {
                    }

                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        try
                        {
                            return RedirectToLocal(returnUrl);
                        }
                        catch
                        {
                        }

                    }
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {
                string filePath = Guid.NewGuid() + Path.GetExtension(file.FileName);
                file.SaveAs(Path.Combine(Server.MapPath("~/UploadedFiles"), filePath));
                //Here you can write code for save this information in your database if you want
            }

            return Json("file uploaded successfully");
        }

        //в деле
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SaveDruftOrderRegister(int orderId,int statusId)
        {
            AccountDetailsViewModel model = new AccountDetailsViewModel();
            model.LogMod = new LoginModel();

            model.OrderId = orderId;
            model.StatusId= statusId;

            ViewBag.ReturnUrl = "/orders/savedruftorder";

            return View(model);
        }

        //в деле
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SaveDruftOrderRegister(AccountDetailsViewModel collection)
        {
            string Sid = Session.SessionID;

            #region Register

            RegisterModel regModel = new RegisterModel();
            if (collection.UserName != null)
            { regModel.UserName = collection.UserName; }

            regModel.Password = "Edit123!";

            // Attempt to register the user
            try
            {
                WebSecurity.CreateUserAndAccount(
                regModel.UserName, regModel.Password,
                new { Pass = regModel.Password });
                Roles.AddUserToRoles(regModel.UserName, new[] { "User" });
                WebSecurity.Login(regModel.UserName, regModel.Password);


            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                AccountDetailsViewModel model = new AccountDetailsViewModel();
                model.DiscountValue = 1;
                model.FIO = collection.FIO;
                model.INN = collection.INN;
                model.LogMod = new LoginModel();
                model.LogMod.Password = regModel.Pass;
                model.LogMod.UserName = regModel.UserName;
                model.OrgName = collection.OrgName;
                model.PhoneNumber = collection.PhoneNumber;
                model.Shipment = collection.Shipment;
                model.UserName = collection.UserName;

                return View(model);
            }

            #endregion
            int userId = 0;

            try
            {
                userId = Db.UserProfile.FirstOrDefault(x => x.UserName.Equals(regModel.UserName)).UserId;
            }
            catch
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }


            #region UserProfileDetails
            UserProfileDetails rec = new UserProfileDetails();

            rec.DiscountID = 1;
            rec.ShipmentAddress = collection.Shipment;
            rec.UserID = userId;
            rec.INN = collection.INN;
            rec.OrgName = collection.OrgName;
            rec.FIO = String.IsNullOrEmpty(collection.FIO) ? "Покупатель" : collection.FIO;
            rec.Phone = collection.PhoneNumber;
            rec.email = collection.UserName;
            Db.UserProfileDetails.Add(rec);
            Db.SaveChanges();
            #endregion

            

            int statusId = collection.StatusId;

            int orderId = collection.OrderId;

            //добавим сюда кусок обработки корзины
            try
            {
                orderId = HelpFunctions.UpdateCartLogin(userId, orderId);
            }
            catch
            {
            }

            sbyte rez = HelpFunctions.SaveOrder(userId, orderId, regModel.Password, statusId);

            if (rez == -1)
            {
                return RedirectToAction("editorder", "orders", new { id = orderId, message = "Оформление заказа возможно при сумме покупки от 15 000 рублей" });
            }
            else
            {
                return RedirectToAction("orderdetails", "orders", new { id = orderId });
            }
           
           
        }

       

        //в деле пара для SaveAccountDetails
        [Authorize]
        public ActionResult AccountDetails()
        {
            AccountDetailsViewModel model = new AccountDetailsViewModel();

            ViewBag.PageName = "AccountDetails";

            int userID = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;

            UserProfileDetails data = Db.UserProfileDetails.FirstOrDefault(x => x.UserID == userID);
            model.UserID = userID;
            model.UserName = WebSecurity.CurrentUserName;


            if (data != null)
            {
                model.Shipment = data.ShipmentAddress;
                model.ProfileID = data.UserProfileDetailsID;
                model.DiscountValue = data.DiscountID;
                model.INN = data.INN;
                model.OrgName = data.OrgName;
                model.FIO = data.FIO;
                model.UserName = data.email;
                model.PhoneNumber = data.Phone;
                model.LogoImg = data.LogoImg;

                if (String.IsNullOrEmpty(model.LogoImg))
                {
                    model.LogoImg = "/images/no_image.png";
                }
            }
            else
            {
                model.DiscountValue = 1;
            }

            return View(model);

        }

        [HttpPost]
        [Authorize]
        public ActionResult SaveAccountDetails(AccountDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AccountDetails", model);
            }

            string body = "";

            int userId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
            string pass = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).Pass;


            UserProfileDetails rec = Db.UserProfileDetails.SingleOrDefault(c => c.UserID == userId);
            if (rec == null)
            {
                rec = new UserProfileDetails();
                rec.DiscountID = 1;
                rec.ShipmentAddress = model.Shipment;
                rec.UserID = userId;
                rec.INN = model.INN;
                rec.OrgName = model.OrgName;
                rec.FIO = model.FIO;
                rec.Phone = model.PhoneNumber;
                rec.email = model.UserName;
                Db.UserProfileDetails.Add(rec);
            }
            else
            {
                rec.ShipmentAddress = model.Shipment;
                rec.INN = model.INN;
                rec.OrgName = model.OrgName;
                rec.FIO = model.FIO;
                rec.Phone = model.PhoneNumber;
                rec.email = model.UserName;
            }
            Db.SaveChanges();
            //System.Web.Routing.RequestContext requestContext = new System.Web.Routing.RequestContext();
            try
            {
                string hostname = HttpContext.Request.Url.Authority.ToString() + "/account"; //requestContext.HttpContext.Request.Url.Host;
                body = String.Format(@"<p>Вы успешно отредактировали свой профиль <a href='http://editpress.ru'>на сайте http://editpress.ru </a></p>
                 <p>Для управления своим аккаунтом  <a href='http://{0}'>перейдите по ссылке</a></p>
                 <p>Для входа используйте адрес электронной почты <span style='color:green;font-weight:bold;text-decoration:underline'>{1}</span></p>
                 <p>Ваш пароль <span style='color:green;font-weight:bold;text-decoration:underline'>{2}</span></p>", hostname, WebSecurity.CurrentUserName, pass);
                HelpFunctions.SendMail2(model.UserName, "Ваши учетные данные на EDITPRESS.RU", body);
            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

        public ActionResult Manage(ManageMessageId? message)
        {
            return View();
        }


        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {

            ViewBag.ReturnUrl = Url.Action("Manage");

            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded = false;
                try
                {
                    UserProfile user = Db.UserProfile.FirstOrDefault(x => x.UserName.Equals(User.Identity.Name));
                    string OldPass = user.Pass;
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, OldPass, model.NewPassword);
                    user.Pass = model.NewPassword;
                    Db.SaveChanges();

                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Index", new { mesage = "Пароль изменен!" });
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный пароль.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult ChangePass()
        {
            ChangePass model = new ChangePass();
            model.ErrMessage = "";
            model.UserEmailOrLogin = "";

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ChangePass(ChangePass model)
        {
            string email = "";
            string login = model.UserEmailOrLogin;
            string pass = "";
            bool emailNotFound = true;
            model.ErrMessage = "";

            if (String.IsNullOrEmpty(model.UserEmailOrLogin))
            {
                model.ErrMessage = "Вы оставили поле пустым";
                return View(model);
            }

            if (!String.IsNullOrEmpty(model.UserEmailOrLogin))
            {
                //если логин в UserProfile это почта
                try
                {
                    email = Db.UserProfile.FirstOrDefault(x => x.UserName.Equals(model.UserEmailOrLogin)).UserName;
                    pass = Db.UserProfile.FirstOrDefault(x => x.UserName.Equals(model.UserEmailOrLogin)).Pass;
                    emailNotFound = false;

                    if (!email.Contains('@') && !email.Contains('.'))
                    {
                        email = "";
                    }
                }
                catch (Exception)
                {


                }

                //если нет то поищем в UserProfileDetails
                if (String.IsNullOrEmpty(email))
                {
                    try
                    {
                        int UserId = Db.UserProfile.FirstOrDefault(x => x.UserName.Equals(model.UserEmailOrLogin)).UserId;
                        email = Db.UserProfileDetails.FirstOrDefault(x => x.UserID == UserId).email;
                        emailNotFound = false;
                    }
                    catch (Exception)
                    {


                    }
                }
            }

            if (String.IsNullOrEmpty(pass))
            {
                model.ErrMessage = "Не удалось обнаружить пароль в базе. Зарегистрируйтесь повторно";
                return View(model);
            }
            string body = String.Format(@"<p>Входные данные <a href= 'http://editpress.ru' > на сайте http://editpress.ru </a></p>
                        <p>Для входа используйте адрес электронной почты <span style='color:green;font-weight:bold;text-decoration:underline'>{0}</span></p>
                         <p>Ваш пароль <span style='color:green;font-weight:bold;text-decoration:underline'>{1}</span></p> ", login, pass);
            HelpFunctions.SendMail2(email, "editpress.ru Ваши учетые данные", body);

            model.ErrMessage = String.Format("Ваш пароль был отправлен на почту {0}", email);
            return View(model);
            //return Redirect("/Home/Index");
            //}



        }


        //
        // POST: /Account/Disassociate

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Disassociate(string provider, string providerUserId)
        //{
        //    string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
        //    ManageMessageId? message = null;

        //    // Only disassociate the account if the currently logged in user is the owner
        //    if (ownerAccount == User.Identity.Name)
        //    {
        //        // Use a transaction to prevent the user from deleting their last login credential
        //        using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
        //        {
        //            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //            if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
        //            {
        //                OAuthWebSecurity.DeleteAccount(provider, providerUserId);
        //                scope.Complete();
        //                message = ManageMessageId.RemoveLoginSuccess;
        //            }
        //        }
        //    }

        //    return RedirectToAction("Manage", new { Message = message });
        //}

        //
        // GET: /Account/Manage


        //
        // POST: /Account/ExternalLogin

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //}

        //
        // GET: /Account/ExternalLoginCallback

        //[AllowAnonymous]
        //public ActionResult ExternalLoginCallback(string returnUrl)
        //{
        //    AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //    if (!result.IsSuccessful)
        //    {
        //        return RedirectToAction("ExternalLoginFailure");
        //    }

        //    if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
        //    {
        //        return RedirectToLocal(returnUrl);
        //    }

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        // If the current user is logged in add the new account
        //        OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // User is new, ask for their desired membership name
        //        string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
        //        ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
        //        ViewBag.ReturnUrl = returnUrl;
        //        return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
        //    }
        //}

        //
        // POST: /Account/ExternalLoginConfirmation

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        //{
        //    string provider = null;
        //    string providerUserId = null;

        //    if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
        //    {
        //        return RedirectToAction("Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Insert a new user into the database
        //        using (UsersContext db = new UsersContext())
        //        {
        //            UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
        //            // Check if user already exists
        //            if (user == null)
        //            {
        //                // Insert name into the profile table
        //                db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
        //                db.SaveChanges();

        //                OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
        //                OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

        //                return RedirectToLocal(returnUrl);
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
        //            }
        //        }
        //    }

        //    ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // GET: /Account/ExternalLoginFailure

        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //[ChildActionOnly]
        //public ActionResult ExternalLoginsList(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        //}

        //[ChildActionOnly]
        //public ActionResult RemoveExternalLogins()
        //{
        //    ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
        //    List<ExternalLogin> externalLogins = new List<ExternalLogin>();
        //    foreach (OAuthAccount account in accounts)
        //    {
        //        AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

        //        externalLogins.Add(new ExternalLogin
        //        {
        //            Provider = account.Provider,
        //            ProviderDisplayName = clientData.DisplayName,
        //            ProviderUserId = account.ProviderUserId,
        //        });
        //    }

        //    ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        //}




        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        //internal class ExternalLoginResult : ActionResult
        //{
        //    public ExternalLoginResult(string provider, string returnUrl)
        //    {
        //        Provider = provider;
        //        ReturnUrl = returnUrl;
        //    }

        //    public string Provider { get; private set; }
        //    public string ReturnUrl { get; private set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
        //    }
        //}

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Email  с таким адресом уже зарегистрирован в системе.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
