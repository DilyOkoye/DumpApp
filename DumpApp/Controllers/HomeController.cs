﻿using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.AdminModel;
using System.Web.Mvc;
using EmailNotification.BAL.Utilities;
using System;
using System.Threading.Tasks;

namespace DumpApp.Controllers
{
    public class HomeController : Controller
    {
        public AdminViewModel adminviewmodel = null;
        public UserProfileModel userprofileModel = null;
        private MenuModel oMenuModel = null;
        private MenuViewModel menuview = null;
        private Header oHeader = null;
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

        public HomeController()
        {
            oMenuModel = new MenuModel();
            menuview = new MenuViewModel();
            oHeader = new Header();
            adminviewmodel = new AdminViewModel();
            userprofileModel = new UserProfileModel();
        }

        public ActionResult DashBoard()
        {
            return View(adminviewmodel);
        }

        [ChildActionOnly]
        public ActionResult MenuControl()
        {
            try
            {
                menuview.Menu = oMenuModel.GetMainMenu();
                adminviewmodel.FullName = userprofileModel.GetFullName(Convert.ToInt32(new ProfileHelper().GetProfile(HttpContext.User.Identity.Name, "User")));
                menuview.UsersRole = oHeader.GetRole(_RoleId);
                menuview.MenuAssign = oMenuModel.GetMenuAssignmentAdmin();
                menuview.roleid = _RoleId;
                return PartialView("_SidePenal", menuview);

            }
            catch (Exception ex)
            {
                return PartialView("_SidePenal", menuview);

            }

        }


        [ChildActionOnly]
        public async Task<ActionResult> Header()
        {
            try
            {
                adminviewmodel.FullName = userprofileModel.GetFullName(Convert.ToInt32(new ProfileHelper().GetProfile(HttpContext.User.Identity.Name, "User")));
                return PartialView("_Header", adminviewmodel);

            }
            catch (Exception ex)
            {

                return PartialView("_Header", adminviewmodel);
            }

        }

    }
}