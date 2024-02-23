﻿using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DumpApp.BAL.AdminModel;
using DumpApp.BAL.OperationsModel;
using EmailNotification.BAL.Utilities;
using DumpApp.BAL.OperationsModel.ViewModel;
using Microsoft.Reporting.WebForms;
using System.Threading.Tasks;
using DumpApp.BAL.ReportModel;
using DumpApp.BAL.ReportModel.ViewModel;

namespace DumpApp.Controllers
{
    public class ReportController : Controller
    {
        public ReportViewModel reportViewModel = null;
        public DumpModel dumpModel = null;
        public ReportModel reportModel = null;
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

        public ReportController()
        {
            reportViewModel = new ReportViewModel();
            dumpModel = new DumpModel();
            reportModel = new ReportModel();
        }

        // GET: Report
        public ActionResult AuditReport(int menuId)
        {
            reportViewModel.admUserProfile = new admUserProfile();
            reportViewModel.rv = new ReturnValues();
            reportViewModel.menuid = menuId;
            return View(reportViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuditReport(AdminViewModel rpt)
        {
            var dt = await reportModel.FetchAuditTrail(rpt.fromDate, rpt.toDate);
            ReportViewer reportView = new ReportViewer();
            reportView.LocalReport.ReportPath += @"Report/AuditTrail.rdlc";
            ReportParameter[] param = new ReportParameter[2];

            string froDate = !string.IsNullOrEmpty(rpt.fromDate) ? string.Format("{0:dd-MMM-yy}", Convert.ToDateTime(rpt.fromDate)) : "";
            string toDate = !string.IsNullOrEmpty(rpt.toDate) ? string.Format("{0:dd-MMM-yy}", Convert.ToDateTime(rpt.toDate)) : "";

            param[0] = new ReportParameter("FromDate", froDate);
            param[1] = new ReportParameter("ToDate", toDate);
            reportView.LocalReport.SetParameters(param);
            ReportDataSource rdc = new ReportDataSource("DataSet1", dt);
            reportView.LocalReport.DataSources.Clear();
            reportView.LocalReport.DataSources.Add(rdc);
            reportView.LocalReport.Refresh();
            reportView.SizeToReportContent = true;
            ViewBag.ReportByMemberAgeC = null;
            ViewBag.ReportByMemberAge = reportView;

            return View(reportViewModel);

        }
    }
}