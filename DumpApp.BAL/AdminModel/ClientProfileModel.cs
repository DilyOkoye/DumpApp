using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumpApp.BAL.AdminModel
{
    public class ClientProfileModel
    {
        private readonly ILicenseRepository repoLincence;
        private readonly ILincenceHistoryRepository repoLincenceHistory;
        private readonly IUserProfileRepository repoUserProfile;
        private readonly IClientProfileRepository repoClientProfile;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;

        public ClientProfileModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoClientProfile = new ClientProfileRepository(idbfactory);
            repoLincence = new LicenseRepository(idbfactory);
            repoLincenceHistory = new LincenceHistoryRepository(idbfactory);
            repoUserProfile = new UserProfileRepository(idbfactory);
        }

        public string FormatTransDate(DateTime? dt)
        {
            if (dt != null)
            {
                return string.Format("{0:dd-MMM-yy }", dt);
            }
            return null;
        }

        public class LincenseHistory
        {
            public string LincenseKey { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string UserId { get; set; }
            public string DateCreated { get; set; }
        }
        public string GetFullname(int id)
        {
            var use =  repoUserProfile.GetNonAsync(o => o.UserId == id);
            if (use != null)
            {
                return use.FullName;
            }
            return null;
        }

        public List<LincenseHistory> LincenseList()
        {

            var d = (from p in repoLincenceHistory.GetAllNonAsync()

                     select new LincenseHistory
                     {

                         LincenseKey = p.LincenseKey,
                         StartDate = FormatTransDate(p.StartDate),
                         EndDate = FormatTransDate(p.EndDate),
                         DateCreated = FormatTransDate(p.DateCreated),
                         UserId = p.UserId ==null?"": GetFullname((int)p.UserId)
                     }).ToList();

            return d;
        }

        public ReturnValues GenerateLicense(string duration, int loginUserId)
        {
            var returnVal = new ReturnValues();
            string key = Cryptors.Encrypt(duration,"DumpApp");
            if (!string.IsNullOrEmpty(key))
            {
                returnVal.UserKey = key;
                returnVal.nErrorCode = 0;
                returnVal.sErrorText = "Generated Successfully for " + duration + " Days";
                return returnVal;

            }
            return null;

        }

        public ReturnValues ApplyLicense(string LincenseKey, int LoginUserId)
        {
            var returnVal = new ReturnValues();


            if (!string.IsNullOrEmpty(LincenseKey))
            {
                string decrptedDays = "";
                var y = repoLincence.GetNonAsync(null);
                decrptedDays = Cryptors.Decrypt(LincenseKey, "DumpApp");

                #region Insert into Lincense History
                var linceHistory = new admLicenseSetUpHistory();
                linceHistory.LincenseKey = y.LincenseKey;
                linceHistory.StartDate = y.StartDate;
                linceHistory.EndDate = Convert.ToDateTime(Cryptors.Decrypt(y.EndDate,"DumpApp"));
                linceHistory.DateCreated = DateTime.Now;
                linceHistory.Status = "Active";
                linceHistory.UserId = LoginUserId;
                repoLincenceHistory.Add(linceHistory);
                var retV2 = unitOfWork.CommitNonAsync(LoginUserId) > 0 ? true : false;
                #endregion

                y.LincenseKey = LincenseKey;
                DateTime dt = DateTime.Now.AddDays(Convert.ToDouble(decrptedDays));
                string fmtDate = string.Format("{0:dd-MMM-yy }", dt);
                y.EndDate = Cryptors.Encrypt(fmtDate,"DumpApp");
                y.StartDate = DateTime.Now;
                repoLincence.Update(y);
                try
                {
                    var retV = unitOfWork.CommitNonAsync(LoginUserId) > 0 ? true : false;

                    if (retV)
                    {

                        returnVal.nErrorCode = 0;
                        returnVal.sErrorText = "License Renewal Applied Successfully to Expire on " + fmtDate;

                        return returnVal;
                    }
                }
                catch (Exception ex)
                {
                    returnVal.nErrorCode = -1;
                    returnVal.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;

                    return returnVal;
                }

            }
            else
            {
                returnVal.nErrorCode = -1;
                returnVal.sErrorText = "License Key is Invalid";
                return returnVal;


            }

            return returnVal;

        }




        public admClientProfile ViewDetails()
        {

            try
            {
                var y = repoClientProfile.GetNonAsync(null);


                return y;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public ReturnValues EditClientProfile(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var y = repoClientProfile.GetNonAsync(null);

            //y.EnforceStrngPwd = p.admClientProfile.EnforceStrngPwd;
            y.Status = "Active";
            y.SystemIdleTimeout = p.admClientProfile.SystemIdleTimeout;
            y.UserId = p.admClientProfile.UserId;
            y.LoginCount = p.admClientProfile.LoginCount;
            y.BankCode = p.admClientProfile.BankCode;
            y.BankAddress = p.admClientProfile.BankAddress;
            y.BankName = p.admClientProfile.BankName;

            repoClientProfile.Update(y);
            try
            {
                var retV = unitOfWork.CommitNonAsync(LoginUserId) > 0 ? true : false;

                if (retV)
                {
                    returnVal.nErrorCode = 0;
                    returnVal.sErrorText = "Record Updated Successfully";

                    return returnVal;
                }
            }
            catch (Exception ex)
            {
                returnVal.nErrorCode = -1;
                returnVal.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;

                return returnVal;
            }

            return returnVal;

        }
    }
}
