using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DumpApp.BAL.AdminModel;
using DumpApp.BAL.OperationsModel;
using DumpApp.BAL.OperationsModel.ViewModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using EmailNotification.BAL.Utilities;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using static DumpApp.Models.Helper;

namespace DumpApp.Controllers
{
    [SessionExpire]
    public class OperationsController : Controller
    {
        public OperationsViewModel operationsViewModel = null;
        public DumpModel dumpModel = null;
        private string _userName = string.Empty;
        private PriviledgeManager primanager = null;
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

        public OperationsController()
        {
            operationsViewModel = new OperationsViewModel();
            dumpModel = new DumpModel();
        }

        public ActionResult LoadList(int menuId)
        {
            operationsViewModel.dumps = new Dumps();
            operationsViewModel.rv = new ReturnValues();
            operationsViewModel.menuid = menuId;
            operationsViewModel.ListOfLoad = dumpModel.ListOfLoad();

            return View(operationsViewModel);
        }
        
        

        public ActionResult DumpList(int menuid)
        {
            operationsViewModel.dumps = new Dumps();
            operationsViewModel.rv = new ReturnValues();
            operationsViewModel.menuid = menuid;
            operationsViewModel.ListOfDumps = dumpModel.ListOfDumps();
            primanager = new PriviledgeManager(menuid, _RoleId);
            operationsViewModel.roleManager = primanager.AssignRoleToUser() == null
                ? new PriviledgeAssignmentManager()
                : primanager.AssignRoleToUser();
            return View(operationsViewModel);
        }

        public async Task<ActionResult> AddNewDump(int menuid)
        {
            operationsViewModel.dumps = new Dumps();
            operationsViewModel.rv = new ReturnValues();
            primanager = new PriviledgeManager(menuid, _RoleId);
            operationsViewModel.roleManager = primanager.AssignRoleToUser() == null
                ? new PriviledgeAssignmentManager()
                : primanager.AssignRoleToUser();
            operationsViewModel.drpLocation = dumpModel.ListLocation();
            operationsViewModel.drpDatabase = await dumpModel.ListDatabase(1);
            operationsViewModel.drpTapeDevice = dumpModel.ListTapeDevice();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("DumpList", "Operations", new { menuid = menuid });
            operationsViewModel.Url = redirectUrl;

            return View(operationsViewModel);
        }

        [HttpPost]
        public async Task<JsonResult> TestConnection(Dumps dumps)
        {
            var rtv = new ReturnValues();
            rtv.nErrorCode = -1;
            var p = new OperationsViewModel()
            {
                dumps = new Dumps()
                {
                    Filename = dumps.Filename,
                    DumpTypeCheck = dumps.DumpTypeCheck,
                    DumpType = dumps.DumpType,
                    LocationId = dumps.LocationId,
                    TapeIdentifier = dumps.TapeIdentifier ,
                    DumpName  = dumps.DumpName,
                    DumpDescription = dumps.DumpDescription,
                    DatebaseId = dumps.DatebaseId,
                    TapeDeviceId = dumps.TapeDeviceId ,
                    OperationType = dumps.OperationType,
                    Id = dumps.Id
                }
            };

            if (p.dumps.OperationType == "AddDump")
            {
                rtv = await dumpModel.ProcessDump(p, _userId, "Test");
                return Json(rtv, JsonRequestBehavior.AllowGet);
            }
            else if (p.dumps.OperationType == "EditDump")
            {
                rtv = await dumpModel.EditDump(p, _userId, "Test");
                return Json(rtv, JsonRequestBehavior.AllowGet);
            }
            else
            {
                rtv = await dumpModel.ProcessLoad(p, _userId, "Test");
                return Json(rtv, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddNewDump(OperationsViewModel p, string button)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.rv = await dumpModel.ProcessDump(p, _userId, button);

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


        public async Task<JsonResult> GetDatabases(int locationId)
        {
            var databaseList =await dumpModel.DatabaseList(locationId);
            return Json(databaseList, JsonRequestBehavior.AllowGet);
        }
        

        public async Task<ActionResult> EditDump(int h, int menuid)
        {
            operationsViewModel.rv = new ReturnValues();
            operationsViewModel.dumps = await dumpModel.ViewDetails(h);
            operationsViewModel.drpLocation = dumpModel.ListLocation();
            operationsViewModel.drpDatabase =await dumpModel.ListDatabase(operationsViewModel.dumps.DumpType =="Offsite"? (int)operationsViewModel.dumps.LocationId:1);
            operationsViewModel.drpTapeDevice = dumpModel.ListTapeDevice();
            operationsViewModel.menuid = menuid;

            var redirectUrl = new UrlHelper(Request.RequestContext).Action("DumpList", "Operations", new { menuid = menuid });
            operationsViewModel.Url = redirectUrl;
            primanager = new PriviledgeManager(menuid, _RoleId);
            operationsViewModel.roleManager = primanager.AssignRoleToUser() == null
                ? new PriviledgeAssignmentManager()
                : primanager.AssignRoleToUser();

            return View(operationsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditDump(OperationsViewModel p, string button)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                    p.rv = await dumpModel.EditDump(p, _userId, button);

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

        public async Task<ActionResult> Load(int h, int menuid)
        {
            operationsViewModel.rv = new ReturnValues();
            operationsViewModel.dumps = await dumpModel.ViewDetails(h);
            operationsViewModel.drpLocation = dumpModel.ListLocation();
            operationsViewModel.drpDatabase = await dumpModel.ListDatabase(operationsViewModel.dumps.DumpType == "Offsite" ? (int)operationsViewModel.dumps.LocationId : 1);
            operationsViewModel.drpTapeDevice = dumpModel.ListTapeDevice();
            operationsViewModel.menuid = menuid;

            var redirectUrl = new UrlHelper(Request.RequestContext).Action("DumpList", "Operations", new { menuid = menuid });
            operationsViewModel.Url = redirectUrl;
            primanager = new PriviledgeManager(menuid, _RoleId);
            operationsViewModel.roleManager = primanager.AssignRoleToUser() == null
                ? new PriviledgeAssignmentManager()
                : primanager.AssignRoleToUser();

            return View(operationsViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Load(OperationsViewModel p, string button)
        {
            var rtv = new ReturnValues();
            try
            {
                if (ModelState.IsValid)
                {
                   p.rv = await dumpModel.ProcessLoad(p, _userId, button);

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
    }
}