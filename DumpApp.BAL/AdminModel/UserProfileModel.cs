using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using DumpApp.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DumpApp.BAL.OperationsModel;
using System.Data;

namespace DumpApp.BAL.AdminModel
{
    public class UserProfileModel
    {
        private readonly IUserProfileRepository repoadmUserProfile;
        private readonly IStatusItemRepository repoStatusItem;
        private readonly IClientProfileRepository repoClientProfileRepository;
        private readonly IRoleRepository reporole;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        private AdminViewModel adminviewModel = null;

        public UserProfileModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            reporole = new RoleRepository(idbfactory);
            repoadmUserProfile = new UserProfileRepository(idbfactory);
            repoStatusItem = new StatusItemRepository(idbfactory);
            repoClientProfileRepository = new ClientProfileRepository(idbfactory);
            adminviewModel = new AdminViewModel();

        }

        //public IEnumerable<SelectListItem> ListOfServiceSetUp()
        //{
        //    IEnumerable<System.Web.Mvc.SelectListItem> items = repoBankServiceSetUp.GetManyNonAsync(p => p.Status == "Active").AsEnumerable()
        //         .Select(p => new System.Web.Mvc.SelectListItem
        //         {
        //             Text = p.ConnectionName,
        //             Value = p.Itbid.ToString()
        //         });
        //    return items;
        //}



        public IEnumerable<SelectListItem> ListRole()
        {
            IEnumerable<System.Web.Mvc.SelectListItem> items = reporole.GetManyNonAsync(e => e.Status == "Active").AsEnumerable()
                .Select(d => new System.Web.Mvc.SelectListItem
                {
                    Text = d.RoleName,
                    Value = d.RoleId.ToString()
                });
            return items;
        }


        public async Task<string> GetFullname(int id)
        {
            var use = await repoadmUserProfile.Get(o => o.UserId == id);
            if (use != null)
            {
                return use.FullName;
            }
            return null;
        }

        public string GetCreatBy(int cb)
        {
            try
            {
                var by = repoadmUserProfile.GetNonAsync(c => c.UserId == cb).CreatedBy;
                var up = repoadmUserProfile.GetNonAsync(u => u.UserId == by).FullName;

                return up;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ReturnValues> ChangePassword(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();
            try
            {
                var res = repoClientProfileRepository.GetNonAsync(null);
                var y = repoadmUserProfile.GetNonAsync(a => a.UserId == LoginUserId);

                var logpass = repoadmUserProfile.GetNonAsync(c => c.Password == y.Password).Password;

                string oldpass = Cryptors.EncryptLogin(p.ChangePasswordModel.OldPassword, "DumpApp");
                if (y.Password != oldpass)
                {
                    returnVal.nErrorCode = -2;
                    returnVal.sErrorText = "Incorrect Old password.";
                    return returnVal;
                }

                else
                {
                    string Nwpass = Cryptors.EncryptLogin(p.ChangePasswordModel.NewPassword, "DumpApp");

                    if (p.ChangePasswordModel.NewPassword == null)
                    {
                        returnVal.nErrorCode = -2;
                        returnVal.sErrorText = "Enter New Password..";
                        return returnVal;
                    }
                    if (oldpass == Nwpass)
                    {
                        returnVal.nErrorCode = -2;
                        returnVal.sErrorText = "Old can't be the same as New password.";
                        return returnVal;
                    }

                    else
                    {
                        if (p.ChangePasswordModel.ConfirmPassword != null)
                        {
                            string Confpass = Cryptors.EncryptLogin(p.ChangePasswordModel.ConfirmPassword, "DumpApp");
                        }
                        else
                        {
                            returnVal.nErrorCode = -2;
                            returnVal.sErrorText = "Enter Confirm Password.";
                        }

                        if (p.ChangePasswordModel.NewPassword != p.ChangePasswordModel.ConfirmPassword)
                        {
                            returnVal.nErrorCode = -2;
                            returnVal.sErrorText = "New Password and Confirm Password are not the same.";
                            return returnVal;
                        }
                        y.Password = Nwpass;

                        y.PasswordExpiryDate = DateTime.Now.AddDays(Convert.ToDouble(res.EnforcePasswordChangeDays.ToString()));

                        repoadmUserProfile.Update(y);
                        try
                        {
                            var retV = await unitOfWork.Commit(LoginUserId) > 0 ? true : false;

                            if (retV)
                            {
                                returnVal.nErrorCode = 0;
                                returnVal.sErrorText = "Password Changed Successfully";

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
            catch (Exception ex)
            {
                returnVal.nErrorCode = -1;
                returnVal.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;

                return returnVal;
            }
        }

        public string GetDateCreate(int dd)
        {
            try
            {
                var p = repoadmUserProfile.GetNonAsync(f => f.UserId == dd).DateCreated;
                var pdate = DateTime.Parse(p.ToString()).ToString("dd-MM-yyyy hh:mm:ss");

                return pdate;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public admUserProfile ViewDetails(int UserId)
        {
            try
            {
                var y = repoadmUserProfile.GetNonAsync(p => p.UserId == UserId);
                return y;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public async Task<ReturnValues> ResetUserLock(string values, int userid)
        {
            var returnVal = new ReturnValues();
            try
            {
                if (values != null)
                {
                    var chkd = new List<long>(Array.ConvertAll(values.Split(';'), long.Parse));

                    for (int i = 0; i < chkd.Count(); i++)

                        try
                        {
                            var vals = Convert.ToInt32(chkd[i]);
                            var t = await repoadmUserProfile.Get(c => c.UserId == vals);
                            if (t != null)
                            {

                                t.lockcount = 0;
                                t.logincount = 0;
                                repoadmUserProfile.Update(t);
                                try
                                {
                                    var retV = await unitOfWork.Commit(userid) > 0 ? true : false;

                                    if (retV)
                                    {
                                        returnVal.nErrorCode = 0;
                                        returnVal.sErrorText = "Reset Lock Out Successfull";
                                    }

                                    return returnVal;
                                }
                                catch (Exception ex)
                                {
                                    returnVal.nErrorCode = -1;
                                    returnVal.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;

                                    return returnVal;
                                }
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
                else
                {
                    returnVal.nErrorCode = -3;
                    returnVal.sErrorText = "Tick the check box(es) to select user";
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

        public List<admUserProfile> ListOfLockedUser()
        {
            DateTime dt;
            var d = (from h in repoadmUserProfile.GetManyNonAsync(o => o.lockcount == 1)


                     select new admUserProfile
                     {
                         UserId = h.UserId,
                         LoginId = h.LoginId,
                         FullName = h.FullName,
                         DateCreated = h.DateCreated,
                         RoleName = h.RoleName,
                         //LastLoginDate = h.LastLoginDate == null ? string.Format("{0:dd-MM-yy}", DateTime.Now) : DateTime.TryParse(h.LastLoginDate.ToString(), out dt) ? string.Format("{0:dd-MM-yy}", dt) : string.Format("{0:dd-MM-yy}", DateTime.Now),
                         //LoginStatus = h.LoginStatus,
                         MobileNo = h.MobileNo,
                         // PasswordExpiryDate = h.PasswordExpiryDate,
                         Status = h.Status,
                         EmailAddress = h.EmailAddress,
                         CreatedBy = h.CreatedBy
                     }).ToList();

            return d;
        }
        public string RoleName(int roleId)
        {
            try
            {
                var res = reporole.GetNonAsync(o => o.RoleId == roleId);
                if (res != null)
                {
                    return res.RoleName;

                }

            }
            catch (Exception)
            {


            }
            return null;
        }

        public List<admUserProfile> ListOfUserProfile()
        {
            DateTime dt;
            var d = (from h in repoadmUserProfile.GetAllNonAsync()


                     select new admUserProfile
                     {
                         UserId = h.UserId,
                         LoginId = h.LoginId,
                         FullName = h.FullName,
                         DateCreated = h.DateCreated,
                         RoleName = h.RoleId == null ? "" : RoleName((int)h.RoleId),
                         MobileNo = h.MobileNo,
                         Status = h.Status,
                         EmailAddress = h.EmailAddress,
                         CreatedBy = h.CreatedBy
                     }).ToList();

            return d;
        }


        public IEnumerable<SelectListItem> ListStatus()
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = repoStatusItem.GetAllNonAsync().AsEnumerable()
                 .Select(p => new System.Web.Mvc.SelectListItem
                 {
                     Text = p.Status,
                     Value = p.StatusValue

                 });
            return items;
        }


        public async Task<ReturnValues> AddUserProfile(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var t = await repoadmUserProfile.Get(c => c.LoginId.ToUpper() == p.admUserProfile.LoginId.ToUpper());
            if (t != null)
            {
                returnVal.nErrorCode = -2;
                returnVal.sErrorText = "LoginId already exist.";
                return returnVal;
            }


            if (p.admUserProfile.RoleId != null)
            {
                var Role = await reporole.Get(e => e.RoleId == p.admUserProfile.RoleId);
                p.admUserProfile.RoleName = Role.RoleName;
            }

            p.admUserProfile.DateCreated = DateTime.Now;
            p.admUserProfile.LoggedOn = false;
            p.admUserProfile.PasswordExpiryDate = DateTime.Now.AddMonths(3);
            p.admUserProfile.IsFirstLogin = false;
            p.admUserProfile.CreatedBy = LoginUserId;
            p.admUserProfile.EnforcePswdChange = "Y";
            p.admUserProfile.lockcount = 0;
            p.admUserProfile.LoggedOn = false;
            p.admUserProfile.loginstatus = 0;
            p.admUserProfile.logincount = 0;
            p.admUserProfile.LoginId = p.admUserProfile.LoginId;
            p.admUserProfile.Password = Cryptors.EncryptLogin(p.admUserProfile.LoginId, "DumpApp");

            repoadmUserProfile.Add(p.admUserProfile);
            try
            {
                var retV = await unitOfWork.Commit(LoginUserId) > 0 ? true : false;

                if (retV)
                {


                    returnVal.nErrorCode = 0;
                    returnVal.sErrorText = "Record Added Successfully";
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

        public async Task<ReturnValues> EditUserProfile(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var y = await repoadmUserProfile.Get(a => a.UserId == p.admUserProfile.UserId);
            if (y != null)
            {


                if (p.admUserProfile.RoleId != null)
                {
                    var Role = await reporole.Get(e => e.RoleId == p.admUserProfile.RoleId);
                    p.admUserProfile.RoleName = Role.RoleName;
                    y.RoleName = p.admUserProfile.RoleName;
                }

                y.FullName = p.admUserProfile.FullName;
                y.Status = p.admUserProfile.Status;
                y.MobileNo = p.admUserProfile.MobileNo;

                y.RoleId = p.admUserProfile.RoleId;
                y.LoginId = p.admUserProfile.LoginId;

                repoadmUserProfile.Update(y);
                try
                {
                    var retV = await unitOfWork.Commit(LoginUserId) > 0 ? true : false;

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

            }

            return returnVal;
        }

        public List<UserDetails> GetUser(int h)
        {
            List<UserDetails> userlist = new List<UserDetails>();
            var Usr = new UserDetails();
            var User = repoadmUserProfile.GetNonAsync(t => t.UserId == h);
            Usr.UserId = User.UserId.ToString();
            Usr.FullName = User.FullName;
            Usr.RoleId = User.RoleId.ToString();
            Usr.RoleName = User.RoleName;

            userlist.Add(Usr);

            return userlist;

        }


        public string GetFullName(int ItbId)
        {
            return repoadmUserProfile.GetById(ItbId).FullName;
        }

        public int GetNotification()
        {
            var result = Cryptors.GetNotifications();
            
            return result.Rows.Count;
        }



    }

    public class Notification
    {
        public int Count { get; set; }
        public string Message { get; set; }

    }

    public class UserDetails
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }

    }
}
