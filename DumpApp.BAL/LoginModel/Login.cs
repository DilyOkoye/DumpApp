using DumpApp.BAL.Utilities;
using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using DumpApp.DAL;

namespace DumpApp.BAL.LoginModel
{
    public class Login
    {
        private readonly IUserProfileRepository repoUserProfile;
        private readonly IClientProfileRepository repoClientProfile;
        private readonly IUserLoginRepository repoUserlogin;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;

        public Login()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoUserProfile = new UserProfileRepository(idbfactory);
            repoUserlogin = new UserLoginRepository(idbfactory);
            repoClientProfile = new ClientProfileRepository(idbfactory);

        }
        public void LogUserLogin(admUserLogin user, int UserId, string AccountNo)
        {
            try
            {
                repoUserlogin.Add(user);
                unitOfWork.Commit(UserId);
            }
            catch (Exception ex)
            {

            }
        }
        public void LogUserLogOut(Guid gh, int UserId)
        {
            try
            {
                var h = repoUserlogin.GetById(gh);
                h.logoutDate = DateTime.Now;
                repoUserlogin.Update(h);
                unitOfWork.Commit(UserId);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<LoginReturnProperty> AutheticateUser(string Username, string Pwd)
        {
            var returnProp = new LoginReturnProperty();
            string uname = string.Empty;
            string pass = string.Empty;
            short? comp = null;
            try
            {


                admUserProfile admUserProfile = null;
                var cp = await repoClientProfile.Get(null);
                try
                {
                    admUserProfile = await repoUserProfile.Get(p => p.LoginId.ToUpper().Equals(Username.ToUpper().Trim()) && p.Status == "Active");
                }
                catch (Exception ex)
                {

                    returnProp.ErrorDisplay = new ErrorDisplay()
                    {
                        ErrorCode = -101,
                        ErrorText = string.Format("Failure to authenticate information. Please contact {0} local contact center", cp.BankName)
                    };
                    return returnProp;
                }
                if (admUserProfile == null)
                {
                    returnProp.ErrorDisplay = new ErrorDisplay()
                    {
                        ErrorCode = -101,
                        ErrorText =
                            $"Login ID Credentials Does Not Exist. Please contact {cp.BankName} local contact center"
                    };
                    return returnProp;
                }

                if (admUserProfile.logincount >= cp.LoginCount)
                {
                    admUserProfile.lockcount = 1;
                    repoUserProfile.Update(admUserProfile);
                    await unitOfWork.Commit(admUserProfile.UserId);
                    returnProp.ErrorCode = -1;
                    returnProp.ErrorDisplay = new ErrorDisplay()
                    {
                        ErrorCode = -1,
                        ErrorText = "User Locked. Contact administrator"
                    };
                    return returnProp;
                }


                #region Check for Enforce Password Change 
                if (admUserProfile.EnforcePswdChange == "Y")
                {
                    returnProp.UserId = admUserProfile.UserId;
                    returnProp.ErrorCode = -5;
                    returnProp.ErrorDisplay = new ErrorDisplay()
                    {

                        ErrorCode = -5,
                        ErrorText = string.Format("Enforce Change Password")
                    };
                    return returnProp;
                }
                #endregion

                string compare = Cryptors.EncryptLogin(Pwd, "DumpApp");
                var com = await repoUserProfile.Get(i => i.LoginId.Trim().ToUpper().Equals(Username.Trim().ToUpper(), StringComparison.InvariantCultureIgnoreCase) && i.Password == compare);
                if (com != null)
                {

                    returnProp.ErrorDisplay = new ErrorDisplay()
                    {
                        ErrorCode = 0,
                        ErrorText = "Login Successfull"
                    };
                    returnProp.ErrorCode = 0;
                    returnProp.EnforcePassChange = admUserProfile.EnforcePswdChange;
                    returnProp.RoleId = admUserProfile.RoleId;
                    returnProp.FullName = admUserProfile.FullName.ToUpper();
                    returnProp.UserId = admUserProfile.UserId;

                    var data = await repoUserlogin.GetMany(p => p.UserId == admUserProfile.UserId);
                    returnProp.LastLoginDate = string.Format("{0:dd/MM/yyyy HH:mm:ss}", data.Max(o => o.loginDate)) == string.Empty ?
                        string.Format("{0:dd-MM-yyyy HH:mm:ss}", DateTime.Now) : string.Format("{0:dd-MM-yyyy HH:mm:ss}", data.Max(o => o.loginDate));


                    comp = 1;
                    admUserProfile.lockcount = 0;
                    admUserProfile.loginstatus = 0;
                    admUserProfile.logincount = 0;
                    admUserProfile.EnforcePswdChange = "N";
                    repoUserProfile.Update(admUserProfile);
                    await unitOfWork.Commit(admUserProfile.UserId);
                }
                else
                {
                    if (admUserProfile.logincount >= cp.LoginCount)
                    {
                        admUserProfile.lockcount = 1;
                        repoUserProfile.Update(admUserProfile);
                        await unitOfWork.Commit(admUserProfile.UserId);
                        returnProp.ErrorCode = -1;
                        returnProp.ErrorDisplay = new ErrorDisplay()
                        {
                            ErrorCode = -1,
                            ErrorText = string.Format("User Locked. Contact administrator")
                        };
                        return returnProp;
                    }
                    if (admUserProfile.logincount < cp.LoginCount)
                    {

                        admUserProfile.logincount = Convert.ToInt16(admUserProfile.logincount + 1);
                        repoUserProfile.Update(admUserProfile);
                        await unitOfWork.Commit(admUserProfile.UserId);
                        returnProp.ErrorCode = -1;
                        returnProp.ErrorDisplay = new ErrorDisplay()
                        {
                            ErrorCode = -1,
                            ErrorText = "Invalid Login Id/Password.Enter Password (" + admUserProfile.logincount + "/" + cp.LoginCount + ")"
                        };

                    }
                }
            


            }

            catch (Exception ex)
            {
                returnProp.ErrorDisplay = new ErrorDisplay()
                {
                    ErrorCode = -2,
                    ErrorText = "Internal Error Occurred Kindly Contact Info Tech"
                };
                return returnProp;
            }
            return returnProp;


        }
        private const string key = "ietech12233";
        public async Task<short> compare(string psPWD, int userid, string username)
        {

            string compare = Cryptors.Encrypt(psPWD, key);

            var comp = await repoUserProfile.Get(i => i.LoginId.ToUpper().Equals(username.ToUpper()) && i.Password == compare);

            if (comp != null)
            {
                return 1;
            }
            return 0;
        }


        public async Task<short> PasswordUpdate(string psPWD, int userid, string username)
        {

            short? comp = null;
            string newpass = Cryptors.Encrypt(psPWD, userid.ToString());
            var pas = await repoUserProfile.Get(o => o.UserId == userid);
            if (pas != null)
            {
                if (newpass != pas.Password)
                {

                    pas.Password = newpass;
                    pas.EnforcePswdChange = "N";
                    repoUserProfile.Update(pas);

                    var retV = await unitOfWork.Commit(userid) > 0 ? true : false;

                    if (retV)
                    {
                        comp = 1;
                        return (short)comp;
                    }
                    else
                    {
                        comp = 0;
                        return (short)comp;
                    }
                }
                else if (newpass == pas.Password)
                {
                    comp = 2;// Same Password which should not be allowed
                    return (short)comp;

                }


            }

            else
            {
                comp = 0;// Credentials Does not Exist
                return (short)comp;
            }

            return (short)comp;
        }

    }
}
