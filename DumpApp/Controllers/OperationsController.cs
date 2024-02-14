﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DumpApp.BAL.AdminModel;
using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.OperationsModel;
using DumpApp.BAL.OperationsModel.ViewModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using EmailNotification.BAL.Utilities;

namespace DumpApp.Controllers
{
    public class OperationsController : Controller
    {
        public OperationsViewModel operationsViewModel = null;
        public DumpModel dumpModel = null;
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

        public OperationsController()
        {
            operationsViewModel = new OperationsViewModel();
            dumpModel = new DumpModel();
        }


        public ActionResult LoadDump(int menuid)
        {
            operationsViewModel.dumps = new Dumps();
            operationsViewModel.rv = new ReturnValues();
            operationsViewModel.menuid = menuid;
            operationsViewModel.ListOfDumps = dumpModel.ListOfDumps();

            return View(operationsViewModel);
        }

        public ActionResult AddNewDump(int menuid)
        {
            return View(operationsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddNewDump(OperationsViewModel p)
        {
            var rtv = new ReturnValues();
            try
            {
                //if (ModelState.IsValid)
                //{
                //    p.admUserProfile.Status = "Active";
                //    p.rv = await userprofilemodel.AddUserProfile(p, _userId);

                //    return (Json(JsonResponseFactory.SuccessResponse(p), JsonRequestBehavior.DenyGet));
                //}
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

        public ActionResult EditDump(int menuid)
        {
            return View(operationsViewModel);
        }
    }
}