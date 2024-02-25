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

namespace DumpApp.BAL.AdminModel
{
   
    public class LocationModel
    {

        private readonly IStatusItemRepository repoStatus;
        private readonly IUserProfileRepository repoUserProfile;
        private readonly ILocationRepository repoLocationRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        public LocationModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoStatus = new StatusItemRepository(idbfactory);
            repoLocationRepository = new LocationRepository(idbfactory);
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


        public List<admLocation> ListOfLocation()
        {

            var d = (from h in repoLocationRepository.GetAllNonAsync()
                     select new admLocation()
                     {
                         Id = h.Id,
                         Name = h.Name,
                         Server = h.Server,
                         IsHeadOffice = h.IsHeadOffice,
                         Port = h.Port,
                         Username = h.Username,
                         Password = h.Password,
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


        public async Task<admLocation> ViewDetails(int id)
        {
            try
            {
                var y = await repoLocationRepository.Get(p => p.Id == id);
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


        public async Task<ReturnValues> AddLocation(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var t = await repoLocationRepository.Get(c => c.Name.ToUpper() == p.admLocation.Name.ToUpper());
            if (t != null)
            {
                returnVal.nErrorCode = -2;
                returnVal.sErrorText = "Location Name Already Exist.";
                return returnVal;
            }

           
            if (p.admLocation.IsHeadOffice)
            {
                var location = await repoLocationRepository.GetAll();
                var isHeadOffice = location.Where(o => o.IsHeadOffice == true);
                if (isHeadOffice.Any())
                {
                    returnVal.nErrorCode = -2;
                    returnVal.sErrorText = "Head office location Already Exist.";
                    return returnVal;
                }
            }

            p.admLocation.DateCreated = DateTime.Now;
            p.admLocation.Status = "Active";
            p.admLocation.UserId = LoginUserId;
           
            repoLocationRepository.Add(p.admLocation);
            try
            {
                var retV = await unitOfWork.Commit(LoginUserId) > 0 ? true : false;

                if (retV)
                {

                    returnVal.nErrorCode = 0;
                    returnVal.sErrorText = "Record Added Succesfully";
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

        public async Task<ReturnValues> EditLocation(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var y = await repoLocationRepository.Get(a => a.Id == p.admLocation.Id);
            if (y != null)
            {
                if (p.admLocation.IsHeadOffice)
                {
                    var location = await repoLocationRepository.GetAll();
                    var isHeadOffice = location.Where(o => o.IsHeadOffice == true);
                    if (isHeadOffice.Any())
                    {
                        returnVal.nErrorCode = -2;
                        returnVal.sErrorText = "Head office location Already Exist.";
                        return returnVal;
                    }
                }

                y.Name = p.admLocation.Name;
                y.Description = p.admLocation.Description;
                y.Password = p.admLocation.Password;
                y.Server = p.admLocation.Server;
                y.Port =p.admLocation.Port;
                y.Username = p.admLocation.Username;
                y.Status = "Active";
                y.UserId = LoginUserId;
                repoLocationRepository.Update(y);
                try
                {
                    var retV = await unitOfWork.Commit(LoginUserId) > 0 ? true : false;

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

            }

            return returnVal;
        }

    }
}
