﻿using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DumpApp.BAL.AdminModel.ViewModel
{
    public class AdminViewModel
    {

        public string username { get; set; }
        public string UsercreatedBy { get; set; }
        public string RoleNames { get; set; }
        public int RoleIds { get; set; }

        public List<RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem> AssignOperations { set; get; }
        public List<RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem> AssignAdmin { set; get; }
        public List<RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem> AssignReports { set; get; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public PriviledgeAssignmentManager roleManager { get; set; }
        public admRole admRole { get; set; }
        public List<admRole> ListOfRole { get; set; }
        public string Url { get; set; }
        public string PasswordExpiryDate { get; set; }
        //public admEmailSetUp admEmailSetUp { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public bool RepeatTask { get; set; }
        public bool ssl { get; set; }
        public string RepeatTaskEvery { get; set; }
        public admUserProfile admUserProfile { get; set; }
        public List<admUserProfile> ListadmUserProfile { set; get; }
        //public List<Implementation.ClientProfileModel.AuditInfo> ListOfAudit { set; get; }

        //public List<admEmailSetUp> ListOfMails { set; get; }
        public string FullName { get; set; }
        public int menuid { get; set; }
        public int eventId { get; set; }
        public IEnumerable<SelectListItem> drpStatus { get; set; }
        public IEnumerable<SelectListItem> drpEvent { get; set; }
        public IEnumerable<SelectListItem> drpRole { get; set; }
        public admClientProfile admClientProfile { get; set; }
        //public sysaudit sysaudit { get; set; }
        public ReturnValues rv { get; set; }
        public int Itbid { get; set; }
        public string currProcessing { get; set; }
        public string dateCreated { get; set; }
        public string createdby { get; set; }


        #region Format Date
        public string FormatTransDate(DateTime? dt)
        {
            if (dt != null)
            {
                return string.Format("{0:dd-MMM-yy }", dt);
            }
            return null;
        }
        public string FormattedAmount(decimal amount)
        {
            return amount.ToString("N2", CultureInfo.InvariantCulture);
        }

        public string FormatDate(DateTime? dt)
        {
            if (dt != null)
            {
                return string.Format("{0:dd-MMM-yy HH:mm:ss}", dt);
            }
            return null;
        }

        public string FormatDateCurrProcessing(DateTime? dt)
        {
            if (dt != null)
            {
                return string.Format("{0:yyyy-MM-dd }", dt);
            }
            return null;
        }
        #endregion



    }
}
