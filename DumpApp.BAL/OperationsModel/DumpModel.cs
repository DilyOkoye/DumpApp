using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.OperationsModel.ViewModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using Sybase.Data.AseClient;

namespace DumpApp.BAL.OperationsModel
{
    public class DumpModel
    {
        private readonly IUserProfileRepository repoUserProfileRepository;
        private readonly IDumpTypeRepository repoDumpTypeRepository;
        private readonly ILocationRepository repoLocation;
        private readonly IDatabaseRepository repoDatabase;
        private readonly IDumpRepository repoDumpRepository;
        private readonly ITapeDeviceRespository repoTapeDevice;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
         
        public DumpModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoUserProfileRepository = new UserProfileRepository(idbfactory);
            repoDumpTypeRepository = new DumpTypeRepository(idbfactory);
            repoDumpRepository = new DumpRepository(idbfactory);
            repoLocation = new LocationRepository(idbfactory);
            repoDatabase = new DatabaseRepository(idbfactory);
            repoTapeDevice = new TapeDeviceRespository(idbfactory);
        }

        public List<Dumps> ListOfDumps()
        {

            var d = (from h in repoDumpRepository.GetAllNonAsync()
                select new Dumps()
                {
                   DumpName = h.DumpName,
                   DumpDescription = h.DumpDescription,
                   TapeDescription = h.TapeDescription,
                   Status = h.Status,
                   Filename = h.Filename,
                   DumpType = h.DumpType == null? "": GetDumpTypeName((int)h.DumpType),
                   DumpDate = h.DumpDate == null?"": $"{h.DumpDate:F}",
                   TapeType = h.TapeType,
                   DateCreated = h.DateCreated == null ? "" : $"{h.DateCreated:F}",
                   TapeIdentifier = h.TapeIdentifier,
                   Id = h.Id,
                   CreatedBy = h.CreatedBy == null ? "" : GetUserById((int)h.CreatedBy),
                }).ToList();

            return d;
        }

        public IEnumerable<SelectListItem> ListLocation()
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = repoLocation.GetAllNonAsync().AsEnumerable()
                .Select(p => new System.Web.Mvc.SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()

                });
            return items;
        }

        public IEnumerable<SelectListItem> ListTapeDevice()
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = repoTapeDevice.GetAllNonAsync().AsEnumerable()
                .Select(p => new System.Web.Mvc.SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()

                });
            return items;
        }

        public IEnumerable<SelectListItem> ListDatabase()
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = repoDatabase.GetAllNonAsync().AsEnumerable()
                .Select(p => new System.Web.Mvc.SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()

                });
            return items;
        }


        public string GetUserById(int id)
        {
            var use =  repoUserProfileRepository.GetNonAsync(o => o.UserId == id);
            if (use != null)
            {
                return use.FullName;
            }
            return null;
        }


        public string GetDumpTypeName(int id)
        {
            var dumpType =  repoDumpTypeRepository.GetNonAsync(o => o.Id == id);
            if (dumpType != null)
            {
                return dumpType.Name;
            }
            return null;
        }


        public async Task<ReturnValues> ProcessDump(OperationsViewModel p, int loginUserId,string button)
        {
            var returnVal = new ReturnValues();

            var t = await repoDumpRepository.Get(c => c.TapeIdentifier.ToUpper() == p.dumps.TapeIdentifier.ToUpper());
            if (t != null)
            {
                returnVal.nErrorCode = -2;
                returnVal.sErrorText = "Tape Identifier Already Exist.";
                return returnVal;
            }

            var dumpType = p.dumps.DumpType == "Offsite" ? 1 : 2;
            var databaseName = repoDatabase.GetNonAsync(o => o.Id == p.dumps.DatebaseId).Name;
            p.dumps.DatabaseName = databaseName;

            var tapeName = repoTapeDevice.GetNonAsync(o => o.Id == p.dumps.TapeDeviceId).Name;
            p.dumps.TapeName = tapeName;
            
            var admDump = new admDump()
            {
                DumpDate = DateTime.Now,
                DumpType = p.dumps.DumpType =="Offsite"? 1: 2,
                CreatedBy = loginUserId,
                TapeIdentifier = p.dumps.TapeIdentifier,
                DumpName = p.dumps.DumpName,
                DateCreated = DateTime.Now,
                DatebaseId = p.dumps.DatebaseId,
                DumpDescription = p.dumps.DumpDescription,
                Filename = p.dumps.Filename,
                Status = button == "Save"?"Saved":"Active",
                TapeDeviceId = p.dumps.TapeDeviceId,
                TapeDescription = p.dumps.TapeDescription,
                LocationId = p.dumps.LocationId,
                TapeType = p.dumps.DumpTypeCheck ? "New":"Old",
                Password = dumpType == 1 ? Cryptors.Encrypt(p.dumps.TapeIdentifier, "DumpApp"):null 
            };

            p.dumps.Password = admDump.Password;
            if (button == "Test")
            {
                await ExecuteTestDump(p.dumps);
            }

            try
            {
                repoDumpRepository.Add(admDump);
                var result = await unitOfWork.Commit(loginUserId) > 0;
                if (result)
                {
                    if (button == "Save")
                    {
                        returnVal.nErrorCode = 0;
                        returnVal.sErrorText = "Record Saved Successfully";
                        return returnVal;
                    }

                    return await ExecuteDump(p.dumps);
                }
            }
            catch (Exception ex)
            {
                returnVal.nErrorCode = -1;
                returnVal.sErrorText = ex.Message ?? ex.InnerException.Message;

                return returnVal;
            }

            return returnVal;
        }

        public string GetQuery(Dumps dump)
        {
            var query = string.Empty;

            switch (dump.DumpTypeCheck)
            {
                case true when dump.DumpType == "Offsite":
                    query = $"dump {dump.DatabaseName}  to {dump.TapeName} file= '{dump.Filename}' with passwd = '{dump.Password}' with init go";
                    break;
                case false when dump.DumpType == "Offsite":
                    query = $"dump {dump.DatabaseName}  to {dump.TapeName} file= '{dump.Filename}' with passwd = '{dump.Password}'";
                    break;
                case true when dump.DumpType == "Internal":
                    query = $"dump {dump.DatabaseName}  to {dump.TapeName} file= '{dump.Filename}' with init go";
                    break;
                case false when dump.DumpType == "Internal":
                    query = $"dump {dump.DatabaseName}  to {dump.TapeName} file= '{dump.Filename}'";
                    break;
            }

            return query;
        }

        public string GetTestQuery(Dumps dump,string path)
        {
            var query = string.Empty;

            switch (dump.DumpTypeCheck)
            {
                case true when dump.DumpType == "Offsite":
                    query = $"dump {dump.DatabaseName}  to {path + dump.TapeName} file= '{dump.Filename}' with passwd = '{dump.Password}' with init go";
                    break;
                case false when dump.DumpType == "Offsite":
                    query = $"dump {dump.DatabaseName}  to {path + dump.TapeName} file= '{dump.Filename}' with passwd = '{dump.Password}'";
                    break;
                case true when dump.DumpType == "Internal":
                    query = $"dump {dump.DatabaseName}  to {path + dump.TapeName} file= '{dump.Filename}' with init go";
                    break;
                case false when dump.DumpType == "Internal":
                    query = $"dump {dump.DatabaseName}  to {path + dump.TapeName} file= '{dump.Filename}'";
                    break;
            }

            return query;
        }

        public async Task<ReturnValues> ExecuteTestDump(Dumps dump)
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["DumpPath"];

            var sybaseLayer = new SybaseDataLayer();

            return await sybaseLayer.SqlDs(GetTestQuery(dump, path));
        }

        public async Task<ReturnValues> ExecuteDump(Dumps dump)
        {
            var sybaseLayer = new SybaseDataLayer();
            
            return await sybaseLayer.SqlDs(GetQuery(dump));
        }
    }
}
