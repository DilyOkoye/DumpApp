using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DumpApp.BAL.LoginModel;
using DumpApp.BAL.LoginModel.ViewModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using EmailNotification.BAL.Utilities;
using Microsoft.Owin.Logging;
using Login = DumpApp.BAL.LoginModel.Login;

namespace DumpApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private LoginViewModel loginview = null;
        private string _userName = string.Empty;
        private int _userId;
        private string Username = string.Empty;
        private string Password = string.Empty;
        public int _RoleId;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {

            base.Initialize(requestContext);

            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {

                _userName = requestContext.HttpContext.User.Identity.Name;
                string val = new ProfileHelper().GetProfile(_userName, "User").ToString();
                _userId = Convert.ToInt32(new ProfileHelper().GetProfile(_userName, "User"));
                _RoleId = Convert.ToInt32(new ProfileHelper().GetProfile(_userName, "roleid"));
            }
        }

        public ActionResult Login()
        {
            loginview = new LoginViewModel();
            loginview.LoginModel = new Login();
            loginview.LoginReturnProperty = new LoginReturnProperty();
            return View(loginview);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AutheticateUserLogin(LoginViewModel loginViewmodel, FormCollection form)
        {
            try
            {
                var ologinReturn = new LoginReturnProperty();
                ologinReturn.ErrorDisplay = new ErrorDisplay();
                loginViewmodel.LoginModel = new Login();

                if (ModelState.IsValid)
                {
                    LogManager.SaveLog("Trying to Authenticate");
                    Username = HttpUtility.HtmlEncode(loginViewmodel.LoginPwdModel.Username);
                    Password = HttpUtility.HtmlEncode(loginViewmodel.LoginPwdModel.Password);

                    ologinReturn = await loginViewmodel.LoginModel.AutheticateUser(Username, Password);

                    if (ologinReturn.ErrorCode == -5)
                    {

                        return (Json(JsonResponseFactory.SuccessResponse(ologinReturn), JsonRequestBehavior.DenyGet));

                    }
                    if (ologinReturn.ErrorDisplay != null)
                    {
                        if (ologinReturn.ErrorDisplay.ErrorCode == 0)
                        {
                            Session.Clear();
                            string cookiesstr;
                            System.Configuration.Configuration configuration = WebConfigurationManager.OpenWebConfiguration("/aspnetTest");
                            AuthenticationSection authenticationSection = (AuthenticationSection)configuration.GetSection("system.web/authentication");
                            int gg = Session.Timeout;
                            FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, Username, DateTime.Now, DateTime.Now.AddMinutes(gg).AddSeconds(3), false, "");
                            cookiesstr = FormsAuthentication.Encrypt(tkt);
                            HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiesstr);
                            ck.Expires = tkt.Expiration;
                            ck.Path = FormsAuthentication.FormsCookiePath;
                            ck.Secure = true;
                            FormsAuthentication.SetAuthCookie(Username.Trim(), false);
                            new ProfileHelper().SetProfile(Username.Trim(), "User", ologinReturn.UserId.ToString(), "string");
                            new ProfileHelper().SetProfile(Username.Trim(), "roleid", ologinReturn.RoleId.ToString(), "int");
                            new ProfileHelper().SetProfile(Username.Trim(), "LastLoginDate", ologinReturn.LastLoginDate.ToString(), "string");
                            admUserLogin auditLogin = new admUserLogin();
                            Guid gh = Guid.NewGuid();
                            Session["guidNo"] = gh.ToString();
                            #region Log User for Audit Trail
                            auditLogin.UserId = (int)ologinReturn.UserId;
                            auditLogin.loginDate = DateTime.Now;
                            auditLogin.RowGuid = gh;
                            auditLogin.status = "Active";
                            loginViewmodel.LoginModel.LogUserLogin(auditLogin, Convert.ToInt32(ologinReturn.UserId), Username);
                            #endregion

                            ologinReturn.Url = Url.Action("DashBoard", "Home");
                            return (Json(JsonResponseFactory.SuccessResponse(ologinReturn), JsonRequestBehavior.DenyGet));
                        }
                        else
                        {
                            return (Json(JsonResponseFactory.SuccessResponse(ologinReturn), JsonRequestBehavior.DenyGet));

                        }
                    }




                }
                return (Json(JsonResponseFactory.SuccessResponse(ologinReturn), JsonRequestBehavior.DenyGet));

            }
            catch (Exception ex)
            {

            }

            return null;
        }


        public ActionResult Logout()
        {
            loginview = new LoginViewModel();
            loginview.LoginModel = new Login();
            var guidNo = Session["guidNo"];
            if (guidNo != null)
            {

                Guid gNo = Guid.Parse(guidNo.ToString());
                loginview.LoginModel.LogUserLogOut(gNo,
                    Convert.ToInt32(new ProfileHelper().GetProfile(HttpContext.User.Identity.Name, "User")));

            }
            FormsAuthentication.SignOut();
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            Session.Abandon();
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            return RedirectToAction("Login");


        }
    }
}
