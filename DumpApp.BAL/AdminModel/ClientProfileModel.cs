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
        
        private readonly IClientProfileRepository repoClientProfile;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;

        public ClientProfileModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoClientProfile = new ClientProfileRepository(idbfactory);
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
                    returnVal.sErrorText = "Record Updated Succesfully";

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
