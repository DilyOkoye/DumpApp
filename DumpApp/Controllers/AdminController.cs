using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.AdminModel;
using EmailNotification.BAL.Utilities;
using System;
using System.Data.Entity;
using System.Web.Mvc;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using System.Threading.Tasks;
using static DumpApp.Models.Helper;
using DumpApp.BAL.OperationsModel.ViewModel;
using DumpApp.BAL.OperationsModel;
using DumpApp.DAL.Implementation;
using System.Collections.Generic;

namespace DumpApp.Controllers
{
    [SessionExpire]
    public class AdminController : Controller
    {
        public AdminViewModel adminviewModel = null;
        public RoleModel roleModel = null;
        public DatabaseModel databseModel = null;
        public LocationModel locationModel = null;
        public ClientProfileModel ClientProfileModel = null;
        private PriviledgeManager primanager = null;
        public UserProfileModel userprofilemodel = null;
        public TapeDeviceModel TapeDeviceModel = null;
        private string _userName = string.Empty;
        private int _userId;
        public int _RoleId;


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {

                _userName = requestContext.HttpContext.User.Identity.Name;
                _userId = Convert.ToInt32(new ProfileHelper().GetProfile(_userName, "User"));
                _RoleId = Convert.ToInt32(new ProfileHelper().GetProfile(_userName, "roleid"));

            }
        }

        public AdminController()
        {
            adminviewModel = new AdminViewModel();
            userprofilemodel = new UserProfileModel();
            roleModel = new RoleModel();
            ClientProfileModel = new ClientProfileModel();
            TapeDeviceModel = new TapeDeviceModel();
            locationModel = new LocationModel();
            databseModel = new DatabaseModel();

        }

        public ActionResult LicenseHistory(int menuid)
        {
            adminviewModel.rv = new ReturnValues();
            adminviewModel.menuid = menuid;
            //adminviewModel.drpStatus = bankmodel.ListStatus();
            adminviewModel.User = _userName.ToUpper();
            adminviewModel.admLicenseSetUpHistory = new admLicenseSetUpHistory();
            adminviewModel.ListOfLicenseHistory = ClientProfileModel.LincenseList();
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();

            return View(adminviewModel);
        }

        public ActionResult GenerateLicense(int menuid)
        {
            adminviewModel.rv = new ReturnValues();
            adminviewModel.admLicenseSetUpHistory = new admLicenseSetUpHistory();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("LicenseHistory", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();

            return View(adminviewModel);
        }   

        [HttpPost]
        public JsonResult GenerateNewLicense(string duration)
        {

            var rtv = ClientProfileModel.GenerateLicense(duration, _userId);
            return Json(rtv);

        }

        [HttpPost]
        public JsonResult ApplyLicense(string key)
        {

            var rtv = ClientProfileModel.ApplyLicense(key, _userId);
            return Json(rtv);

        }


        #region RenewLicense
        public ActionResult RenewLicense(int menuid)
        {
            adminviewModel.rv = new ReturnValues();
            adminviewModel.admLicenseSetUp = new admLicenseSetUp();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("LicenseHistory", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();

            return View(adminviewModel);
        }


        #endregion

        public ActionResult ManageClientProfile(int menuid)
        {
            adminviewModel.admClientProfile = new admClientProfile();
            adminviewModel.rv = new ReturnValues();
            adminviewModel.menuid = menuid;
            adminviewModel.admClientProfile = ClientProfileModel.ViewDetails();
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                ? new PriviledgeAssignmentManager()
                : primanager.AssignRoleToUser();
            if (adminviewModel.admClientProfile != null)
            {

                adminviewModel.dateCreated = adminviewModel.FormatDate(adminviewModel.admClientProfile.DateCreated);
            }
            return View(adminviewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ClientProfile(AdminViewModel p)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {

                    p.rv = ClientProfileModel.EditClientProfile(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }



        public ActionResult ManageRoles(int menuid)
        {
            adminviewModel.admRole = new admRole();
            adminviewModel.rv = new ReturnValues();
            adminviewModel.menuid = menuid;
            adminviewModel.ListOfRole = roleModel.ListOfRoles();
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();
            return View(adminviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddRole(AdminViewModel p)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {

                    p.rv = await roleModel.AddRoles(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }

        public ActionResult AddRole(int menuid)
        {
            adminviewModel.admRole = new admRole();
            adminviewModel.rv = new ReturnValues();
            adminviewModel.drpStatus = roleModel.ListStatus();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageRoles", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();

            return View(adminviewModel);
        }
        public async Task<ActionResult> EditRole(int h, int menuid)
        {
            adminviewModel.rv = new ReturnValues();
            adminviewModel.admRole = await roleModel.ViewDetails(h);
            adminviewModel.drpStatus = roleModel.ListStatus();
            if (adminviewModel.admRole != null)
            {
                adminviewModel.UsercreatedBy = adminviewModel.admUserProfile.CreatedBy == null ? "" : await roleModel.GetFullname((int)adminviewModel.admUserProfile.CreatedBy);

            }
            adminviewModel.menuid = menuid;

            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageRoles", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();


            return View(adminviewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditRole(AdminViewModel p)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.rv = await roleModel.EditRoles(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }

        public async Task<ActionResult> AssignPrivilege(int roleId, int menuid)
        {
            adminviewModel.rv = new ReturnValues();
            adminviewModel.RoleIds = roleId;
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();
            adminviewModel.RoleNames = await roleModel.GetRoleName(roleId);
            adminviewModel.AssignAdmin = roleModel.AssignValuesSystemAdmin(roleId);
            adminviewModel.AssignOperations = roleModel.AssignValuesOperations(roleId);
            adminviewModel.AssignReports = roleModel.AssignValuesReports(roleId);
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageRoles", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;

            return View(adminviewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AssignPrivilege(AdminViewModel cm)
        {
            cm.rv = new ReturnValues();
            cm.RoleIds = cm.RoleIds;
            cm.rv = await roleModel.AddMenusForUser(cm, cm.RoleIds, _userId);
            return (Json(JsonResponseFactory.SuccessResponse(cm), JsonRequestBehavior.DenyGet));
        }

        public ActionResult UnLockUser(int menuid)
        {
            adminviewModel.admUserProfile = new admUserProfile();
            adminviewModel.rv = new ReturnValues();
            adminviewModel.menuid = menuid;
            adminviewModel.ListadmUserProfile = userprofilemodel.ListOfLockedUser();

            return View(adminviewModel);
        }

        public ActionResult ManageUser(int menuid)
        {
            adminviewModel.admUserProfile = new admUserProfile();
            adminviewModel.rv = new ReturnValues();
            adminviewModel.menuid = menuid;
            adminviewModel.ListadmUserProfile = userprofilemodel.ListOfUserProfile();

            return View(adminviewModel);
        }

        public ActionResult AddUserProfile(int menuid)
        {
            adminviewModel.admUserProfile = new admUserProfile();
            adminviewModel.rv = new ReturnValues();
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                ? new PriviledgeAssignmentManager()
                : primanager.AssignRoleToUser();
            adminviewModel.drpRole = userprofilemodel.ListRole();
            adminviewModel.drpStatus = userprofilemodel.ListStatus();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageUser", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;

            return View(adminviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddUserProfile(AdminViewModel p, string datasources)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.admUserProfile.Status = "Active";
                    p.rv = await userprofilemodel.AddUserProfile(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }



        public async Task<ActionResult> EditUserProfile(int h, int menuid)
        {
            adminviewModel.rv = new ReturnValues();
            adminviewModel.admUserProfile = userprofilemodel.ViewDetails(h);
            if (adminviewModel.admUserProfile != null)
            {
                adminviewModel.PasswordExpiryDate = adminviewModel.admUserProfile.PasswordExpiryDate != null ? adminviewModel.FormatDate(adminviewModel.admUserProfile.PasswordExpiryDate) : null;
                adminviewModel.dateCreated = adminviewModel.admUserProfile.DateCreated != null ? adminviewModel.FormatDate(adminviewModel.admUserProfile.DateCreated) : null;
                adminviewModel.UsercreatedBy = adminviewModel.admUserProfile.CreatedBy == null ? "" : await roleModel.GetFullname((int)adminviewModel.admUserProfile.CreatedBy);

            }

            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();
            adminviewModel.drpRole = userprofilemodel.ListRole();
            adminviewModel.drpStatus = userprofilemodel.ListStatus();
            adminviewModel.menuid = menuid;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageUser", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;

            return View(adminviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditUserProfile(AdminViewModel p)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.rv = await userprofilemodel.EditUserProfile(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }

        [HttpPost]
        public async Task<JsonResult> ResetLockout(string chk)
        {
            AdminViewModel p = new AdminViewModel();
            p.rv = await userprofilemodel.ResetUserLock(chk, _userId);
            return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
        }



        public ActionResult ManageTapeDevice(int menuid)
        {
            adminviewModel.admTapeDevice = new admTapeDevice();
            adminviewModel.rv = new ReturnValues();
            adminviewModel.menuid = menuid;
            adminviewModel.ListOfTapeDevices = TapeDeviceModel.ListOfTapeDevice();

            return View(adminviewModel);
        }



        public ActionResult AddTapeDevice(int menuid)
        {
            adminviewModel.admTapeDevice = new admTapeDevice();
            adminviewModel.rv = new ReturnValues();
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();
            adminviewModel.drpStatus = TapeDeviceModel.ListStatus();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageTapeDevice", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;

            return View(adminviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddTapeDevice(AdminViewModel p, string datasources)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.admTapeDevice.Status = "Active";
                    p.rv = await TapeDeviceModel.AddTapeDevice(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }



        public async Task<ActionResult> EditTapeDevice(int h, int menuid)
        {
            adminviewModel.rv = new ReturnValues();
            adminviewModel.admTapeDevice =await TapeDeviceModel.ViewDetails(h);
            if (adminviewModel.admTapeDevice != null)
            {
                adminviewModel.dateCreated = adminviewModel.admTapeDevice.DateCreated != null ? adminviewModel.FormatDate(adminviewModel.admTapeDevice.DateCreated) : null;
                adminviewModel.UsercreatedBy = adminviewModel.admUserProfile.CreatedBy == null ? "" : await roleModel.GetFullname((int)adminviewModel.admUserProfile.CreatedBy);

            }

            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();
            adminviewModel.drpStatus = TapeDeviceModel.ListStatus();
            adminviewModel.menuid = menuid;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageTapeDevice", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;

            return View(adminviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditTapeDevice(AdminViewModel p)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.rv = await TapeDeviceModel.EditTapeDevice(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }

        [HttpPost]
        public async Task<JsonResult> TestLocationConfigConnection(string server,string port,string username, string password)
        {
            var rtv = new ReturnValues();
            var sybaseDataLayer = new SybaseDataLayer();
            rtv.nErrorCode = -1;
            rtv.sErrorText = "Could not establish connection to " + server;
            var p = new admLocation()
            {
                Password = password,
                Server = server,
                Port = port,
                Username = username
            };

            var result = await sybaseDataLayer.ValidateLocationConfig(p);
            if (result.Count > 0)
            {
                rtv.nErrorCode = 0;
                rtv.sErrorText = "Response from " + server + "Successful";
            }
          
            return Json(rtv, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ManageLocation(int menuid)
        {
            adminviewModel.admLocation = new admLocation();
            adminviewModel.rv = new ReturnValues();
            adminviewModel.menuid = menuid;
            adminviewModel.ListOfLocations = locationModel.ListOfLocation();
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();
            return View(adminviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddLocation(AdminViewModel p)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {

                    p.rv = await locationModel.AddLocation(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }

        public ActionResult AddLocation(int menuid)
        {
            adminviewModel.admLocation = new admLocation();
            adminviewModel.rv = new ReturnValues();
            adminviewModel.drpStatus = locationModel.ListStatus();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageLocation", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();

            return View(adminviewModel);
        }
        public async Task<ActionResult> EditLocation(int h, int menuid)
        {
            adminviewModel.rv = new ReturnValues();
            adminviewModel.admLocation = await locationModel.ViewDetails(h);
            adminviewModel.drpStatus = roleModel.ListStatus();
            if (adminviewModel.admLocation != null)
            {
                adminviewModel.UsercreatedBy = adminviewModel.admLocation.UserId == null ? "" : await roleModel.GetFullname((int)adminviewModel.admLocation.UserId);

            }
            adminviewModel.menuid = menuid;

            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageLocation", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();


            return View(adminviewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditLocation(AdminViewModel p)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.rv = await locationModel.EditLocation(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }




        public ActionResult ManageDatabase(int menuid)
        {
            adminviewModel.admDatabase = new admDatabase();
            adminviewModel.rv = new ReturnValues();
            adminviewModel.menuid = menuid;
            adminviewModel.ListOfDatabase = databseModel.ListOfDatabase();

            return View(adminviewModel);
        }



        public ActionResult AddDatabase(int menuid)
        {
            adminviewModel.admDatabase = new admDatabase();
            adminviewModel.rv = new ReturnValues();
            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();
            adminviewModel.drpStatus = databseModel.ListStatus();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageDatabase", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;

            return View(adminviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddDatabase(AdminViewModel p, string datasources)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.admDatabase.Status = "Active";
                    p.rv = await databseModel.AddDatabase(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }



        public async Task<ActionResult> EditDatabase(int h, int menuid)
        {
            adminviewModel.rv = new ReturnValues();
            adminviewModel.admDatabase = await databseModel.ViewDetails(h);
            if (adminviewModel.admDatabase != null)
            {
                adminviewModel.dateCreated = adminviewModel.admDatabase.DateCreated != null ? adminviewModel.FormatDate(adminviewModel.admDatabase.DateCreated) : null;
                adminviewModel.UsercreatedBy = adminviewModel.admUserProfile.CreatedBy == null ? "" : await roleModel.GetFullname((int)adminviewModel.admUserProfile.CreatedBy);

            }

            primanager = new PriviledgeManager(menuid, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                    ? new PriviledgeAssignmentManager()
                    : primanager.AssignRoleToUser();
            adminviewModel.drpStatus = TapeDeviceModel.ListStatus();
            adminviewModel.menuid = menuid;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageDatabase", "Admin", new { menuid = menuid });
            adminviewModel.Url = redirectUrl;

            return View(adminviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditDatabase(AdminViewModel p)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.rv = await databseModel.EditDatabase(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;
                p.rv = rtv;
                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }

        public ActionResult ChangePassword(int menuId)
        {
            adminviewModel.ChangePasswordModel = new ChangePasswordModel();
            adminviewModel.rv = new ReturnValues();
            adminviewModel.menuid = menuId;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("DashBoard", "Home", new { menuid = menuId });
            adminviewModel.Url = redirectUrl;
            primanager = new PriviledgeManager(menuId, _RoleId);
            adminviewModel.roleManager = primanager.AssignRoleToUser() == null
                ? new PriviledgeAssignmentManager()
                : primanager.AssignRoleToUser();
            return View(adminviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangePassword(AdminViewModel p)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.rv = await userprofilemodel.ChangePassword(p, _userId);

                    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));

                }
            }
            catch (Exception ex)
            {
                rtv.nErrorCode = -1001;
                rtv.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;

                return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
            }
            return (Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet));
        }

    }
}