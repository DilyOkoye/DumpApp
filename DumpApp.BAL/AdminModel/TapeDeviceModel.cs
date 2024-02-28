using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using DumpApp.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DumpApp.BAL.AdminModel
{

    public class TapeDeviceModel
    {

        private readonly IStatusItemRepository repoStatus;
        private readonly IUserProfileRepository repoUserProfile;
        private readonly ITapeDeviceRespository repoTapeDeviceRespository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        public TapeDeviceModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoStatus = new StatusItemRepository(idbfactory);
            repoTapeDeviceRespository = new TapeDeviceRespository(idbfactory);
            repoUserProfile = new UserProfileRepository(idbfactory);

        }

        public IEnumerable<SelectListItem> ListStatus()
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = repoStatus.GetAllNonAsync().AsEnumerable()
                 .Select(p => new System.Web.Mvc.SelectListItem
                 {
                     Text = p.Status,
                     Value = p.StatusValue

                 });
            return items;
        }


        public List<admTapeDevice> ListOfTapeDevice()
        {

            var d = (from h in repoTapeDeviceRespository.GetAllNonAsync()
                     select new admTapeDevice()
                     {
                         Id = h.Id,
                         Name = h.Name,
                         Description = h.Description,
                         Status = h.Status,
                     }).ToList();

            return d;
        }

        public async Task<string> GetFullname(int id)
        {
            var use = await repoUserProfile.Get(o => o.UserId == id);
            if (use != null)
            {
                return use.FullName;
            }
            return null;
        }


        public async Task<admTapeDevice> ViewDetails(int id)
        {
            try
            {
                var y = await repoTapeDeviceRespository.Get(p => p.Id == id);
                if (y != null)
                {
                    return y;
                }

            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public async Task<ReturnValues> AddTapeDevice(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var t = await repoTapeDeviceRespository.Get(c => c.Name.ToUpper() == p.admTapeDevice.Name.ToUpper());
            if (t != null)
            {
                returnVal.nErrorCode = -2;
                returnVal.sErrorText = "Tape Device Name Already Exist.";
                return returnVal;
            }

            p.admTapeDevice.DateCreated = DateTime.Now;
            p.admTapeDevice.Name = p.admTapeDevice.Name;
            p.admTapeDevice.Description = p.admTapeDevice.Description;
            p.admTapeDevice.Status = "Active";
            p.admTapeDevice.UserId = LoginUserId;
            repoTapeDeviceRespository.Add(p.admTapeDevice);
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

        public async Task<ReturnValues> EditTapeDevice(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var y = await repoTapeDeviceRespository.Get(a => a.Id == p.admTapeDevice.Id);
            if (y != null)
            {

                y.Name = p.admTapeDevice.Name;
                y.Description = p.admTapeDevice.Description;
                y.Status = "Active";
                y.UserId = LoginUserId;
                repoTapeDeviceRespository.Update(y);
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

    }
}
