using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;

namespace DumpApp.BAL.AdminModel
{
    public class DatabaseModel
    {

        private readonly IStatusItemRepository repoStatus;
        private readonly IUserProfileRepository repoUserProfile;
        private readonly IDatabaseRepository repoDatabaseRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        public DatabaseModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoStatus = new StatusItemRepository(idbfactory);
            repoDatabaseRepository = new DatabaseRepository(idbfactory);
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


        public List<admDatabase> ListOfDatabase()
        {

            var d = (from h in repoDatabaseRepository.GetAllNonAsync()
                     select new admDatabase()
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


        public async Task<admDatabase> ViewDetails(int id)
        {
            try
            {
                var y = await repoDatabaseRepository.Get(p => p.Id == id);
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


        public async Task<ReturnValues> AddDatabase(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var t = await repoDatabaseRepository.Get(c => c.Name.ToUpper() == p.admDatabase.Name.ToUpper());
            if (t != null)
            {
                returnVal.nErrorCode = -2;
                returnVal.sErrorText = "Database Name Already Exist.";
                return returnVal;
            }

            p.admDatabase.DateCreated = DateTime.Now;
            p.admDatabase.Name = p.admDatabase.Name;
            p.admDatabase.Description = p.admDatabase.Description;
            p.admDatabase.Status = "Active";
            p.admDatabase.UserId = LoginUserId;
            repoDatabaseRepository.Add(p.admDatabase);
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

        public async Task<ReturnValues> EditDatabase(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var y = await repoDatabaseRepository.Get(a => a.Id == p.admDatabase.Id);
            if (y != null)
            {

                y.Name = p.admDatabase.Name;
                y.Description = p.admDatabase.Description;
                y.Status = "Active";
                y.UserId = LoginUserId;
                repoDatabaseRepository.Update(y);
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
