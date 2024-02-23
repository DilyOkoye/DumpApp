using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdoNetCore.AseClient;
using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.OperationsModel.ViewModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using Hangfire;
using Hangfire.Server;

namespace DumpApp.BAL.OperationsModel
{
    public class DumpModel
    {
        private readonly IUserProfileRepository repoUserProfileRepository;
        private readonly IDumpTypeRepository repoDumpTypeRepository;
        private readonly ILocationRepository repoLocation;
        private readonly IDatabaseRepository repoDatabase;
        private readonly IDumpRepository repoDumpRepository;
        private readonly ILoadRepository repoLoadRepository;
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
            repoLoadRepository = new LoadRepository(idbfactory);
        }

        public List<Dumps> ListOfLoad()
        {

            var d = (from h in repoLoadRepository.GetAllNonAsync()
                select new Dumps()
                {
                    DumpName = h.DumpName,
                    DumpDescription = h.DumpDescription,
                    TapeDescription = h.TapeDescription,
                    Status = h.Status,
                    Filename = h.Filename,
                    DumpType = h.DumpType == null ? "" : GetDumpTypeName((int)h.DumpType),
                    DumpDate = h.DumpDate == null ? "" : $"{h.DumpDate:F}",
                    TapeType = h.TapeType,
                    DateCreated = h.DateCreated == null ? "" : $"{h.DateCreated:F}",
                    TapeIdentifier = h.TapeIdentifier,
                    Id = h.Id,
                    CreatedBy = h.CreatedBy == null ? "" : GetUserById((int)h.CreatedBy),
                }).ToList();

            return d;
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

        public async Task<Dumps> ViewDetails(int id)
        {
            try
            {
                var y = await repoDumpRepository.Get(p => p.Id == id);
                if (y != null)
                {
                    return new Dumps()
                    {
                        DumpDate = $"{y.DumpDate:F}",
                        DateCreated = $"{y.DateCreated:F}",
                        TapeIdentifier = y.TapeIdentifier,
                        TapeDescription = y.TapeDescription,
                        DumpDescription = y.DumpDescription,
                        DumpName = y.DumpName,
                        Filename = y.Filename,
                        DumpTypeCheck = y.TapeType == "New",
                        Status = y.Status,
                        CreatedBy = y.CreatedBy == null ? "" : GetUserById((int)y.CreatedBy),
                        DatebaseId = y.DatebaseId,
                        LocationId = y.LocationId,
                        Password = y.Password,
                        Id = y.Id,
                        TapeDeviceId = y.TapeDeviceId,
                        DumpType = y.DumpType ==1? "Offsite": "Internal"
                    };
                }

            }
            catch (Exception ex)
            {

            }
            return null;
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
        
        

        public async Task<ReturnValues> ProcessLoad(OperationsViewModel p, int loginUserId, string button)
        {
            var returnVal = new ReturnValues();

            var t = await repoDumpRepository.Get(c => c.TapeIdentifier.ToUpper() == p.dumps.TapeIdentifier.ToUpper());
            if (t is null)
            {
                returnVal.nErrorCode = -2;
                returnVal.sErrorText = "Tape Identifier Does not Exist";
                return returnVal;
            }

            var dumpType = p.dumps.DumpType == "Offsite" ? 1 : 2;
            var databaseName = repoDatabase.GetNonAsync(o => o.Id == p.dumps.DatebaseId).Name;
            p.dumps.DatabaseName = databaseName;

            var tapeName = repoTapeDevice.GetNonAsync(o => o.Id == p.dumps.TapeDeviceId).Name;
            p.dumps.TapeName = tapeName;

            var admLoad = new admLoad()
            {
                DumpDate = DateTime.Now,
                DumpType = p.dumps.DumpType == "Offsite" ? 1 : 2,
                CreatedBy = loginUserId,
                TapeIdentifier = p.dumps.TapeIdentifier,
                DumpName = p.dumps.DumpName,
                DateCreated = DateTime.Now,
                DatebaseId = p.dumps.DatebaseId,
                DumpDescription = p.dumps.DumpDescription,
                Filename = p.dumps.Filename,
                TapeDeviceId = p.dumps.TapeDeviceId,
                TapeDescription = p.dumps.TapeDescription,
                LocationId = p.dumps.LocationId,
                TapeType = p.dumps.DumpTypeCheck ? "New" : "Old",
                Password = dumpType == 1 ? Cryptors.Encrypt(RandomString(10), "DumpApp") : null
            };

            p.dumps.Password = admLoad.Password;
            if (button == "Test")
            {
                await ExecuteTestLoad(p.dumps);
            }

            try
            {
                repoLoadRepository.Add(admLoad);
                var result = await unitOfWork.Commit(loginUserId) > 0;
                if (result)
                {
                    var jobId = BackgroundJob.Enqueue(
                        () => ExecuteLoad(p.dumps, null, loginUserId));

                    if (!string.IsNullOrEmpty(jobId))
                    {
                        admLoad.JobId = jobId;
                        admLoad.Status = "Processing";
                        await unitOfWork.Commit(loginUserId);
                        returnVal.sErrorText = "Load Operation Successful, Check back later to check Load Status";

                    }
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
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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
                Status = button == "Save"?"Saved":"Processing",
                TapeDeviceId = p.dumps.TapeDeviceId,
                TapeDescription = p.dumps.TapeDescription,
                LocationId = p.dumps.LocationId,
                TapeType = p.dumps.DumpTypeCheck ? "New":"Old",
                Password = dumpType == 1 ? Cryptors.Encrypt(RandomString(10), "DumpApp"):null 
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

                    var jobId = BackgroundJob.Enqueue(
                        () => ExecuteDump(p.dumps,null,loginUserId));
                    
                    if (!string.IsNullOrEmpty(jobId))
                    {
                        admDump.JobId = jobId;
                        admDump.Status = "Processing";
                        await unitOfWork.Commit(loginUserId);
                        returnVal.nErrorCode = 0;
                        returnVal.sErrorText = "Dump Operation Successful, Check back later to check Dump Status";
                    }
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

        public async Task<ReturnValues> EditDump(OperationsViewModel p, int loginUserId, string button)
        {
            var returnVal = new ReturnValues();

            var t = await repoDumpRepository.Get(c => c.Id == p.dumps.Id);
            if (t == null)
            {
                returnVal.nErrorCode = -2;
                returnVal.sErrorText = "Record Does not Exist.";
                return returnVal;
            }

            var dumpType = p.dumps.DumpType == "Offsite" ? 1 : 2;
            var databaseName = repoDatabase.GetNonAsync(o => o.Id == p.dumps.DatebaseId).Name;
            p.dumps.DatabaseName = databaseName;

            var tapeName = repoTapeDevice.GetNonAsync(o => o.Id == p.dumps.TapeDeviceId).Name;
            p.dumps.TapeName = tapeName;


            t.DumpDate = DateTime.Now;
            t.DumpType = p.dumps.DumpType == "Offsite" ? 1 : 2;
            t.CreatedBy = loginUserId;
            t.TapeIdentifier = p.dumps.TapeIdentifier;
            t.DumpName = p.dumps.DumpName;
            t.DateCreated = DateTime.Now;
            t.DatebaseId = p.dumps.DatebaseId;
            t.DumpDescription = p.dumps.DumpDescription;
            t.Filename = p.dumps.Filename;
            t.Status = "Saved";
            t.TapeDeviceId = p.dumps.TapeDeviceId;
            t.TapeDescription = p.dumps.TapeDescription;
            t.LocationId = p.dumps.LocationId;
            t.TapeType = p.dumps.DumpTypeCheck ? "New" : "Old";
            t.Password = dumpType == 1 ? Cryptors.Encrypt(RandomString(10), "DumpApp") : null;
           

            p.dumps.Password = t.Password;
            if (button == "Test")
            {
                await ExecuteTestDump(p.dumps);
            }

            try
            {
                repoDumpRepository.Update(t);
                var result = await unitOfWork.Commit(loginUserId) > 0;
                if (result)
                {
                    if (button == "Save")
                    {
                        returnVal.nErrorCode = 0;
                        returnVal.sErrorText = "Record Updated Successfully";
                        return returnVal;
                    }

                    var jobId = BackgroundJob.Enqueue(
                        () => ExecuteDump(p.dumps, null,loginUserId));

                    if (!string.IsNullOrEmpty(jobId))
                    {
                        t.JobId = jobId;
                        t.Status = "Processing";
                        await unitOfWork.Commit(loginUserId);
                        returnVal.sErrorText = "Dump Operation Successful, Check back later to check Dump Status";

                    }
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

        public string GetTestLoadQuery(Dumps dump, string path)
        {
            var query = string.Empty;

            switch (dump.DumpTypeCheck)
            {
                case true when dump.DumpType == "Offsite":
                    query = $"load {dump.DatabaseName}  to {path + dump.TapeName} file= '{dump.Filename}' with passwd = '{dump.Password}' with init go";
                    break;
                case false when dump.DumpType == "Offsite":
                    query = $"load {dump.DatabaseName}  to {path + dump.TapeName} file= '{dump.Filename}' with passwd = '{dump.Password}'";
                    break;
                case true when dump.DumpType == "Internal":
                    query = $"load {dump.DatabaseName}  to {path + dump.TapeName} file= '{dump.Filename}' with init go";
                    break;
                case false when dump.DumpType == "Internal":
                    query = $"load {dump.DatabaseName}  to {path + dump.TapeName} file= '{dump.Filename}'";
                    break;
            }

            return query;
        }

        public string GetTestDumpQuery(Dumps dump,string path)
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

        public async Task<ReturnValues> ExecuteTestLoad(Dumps dump)
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["LoadPath"];

            var sybaseLayer = new SybaseDataLayer();

            return await sybaseLayer.TestSqlDs(GetTestLoadQuery(dump, path));
        }

        public async Task<ReturnValues> ExecuteTestDump(Dumps dump)
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["DumpPath"];

            var sybaseLayer = new SybaseDataLayer();

            return await sybaseLayer.TestSqlDs(GetTestDumpQuery(dump, path));
        }

       
        private object CheckDBNullValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DBNull.Value;
            else
                return value;
        }

        public async Task<ReturnValues> ExecuteLoad(Dumps dump, PerformContext context, int loginUserId)
        {
            var rtv = new ReturnValues();
            rtv.nErrorCode = -1;
            var dumpRecord = await repoDumpRepository.Get(o => o.Id == dump.Id);

            try
            {

                var sybaseLayer = new SybaseDataLayer();

                List<AseParameter> sp = new List<AseParameter>()
                {
                    new AseParameter() {ParameterName = "@pbNewInitiatization", AseDbType = AseDbType.Bit, Value= dump.DumpType},
                    new AseParameter() {ParameterName = "@psDumpType", AseDbType = AseDbType.VarChar, Value= dump.DumpType},
                    new AseParameter() {ParameterName = "@psDatabaseName", AseDbType = AseDbType.VarChar, Value= dump.DatabaseName},
                    new AseParameter() {ParameterName = "@psTapeName", AseDbType = AseDbType.VarChar, Value= dump.TapeName},
                    new AseParameter() {ParameterName = "@psFileName", AseDbType = AseDbType.Decimal, Value=dump.TapeName},
                    new AseParameter() {ParameterName = "@psUserName", AseDbType = AseDbType.VarChar, Value=CheckDBNullValue(dump.Password)}
                };


                var dt = await sybaseLayer.SqlDs("sp_loaddb", sp, 0);

                if (dt != null && dt.Rows.Count > 0)
                {
                    rtv.nErrorCode = Convert.ToInt32(dt.Rows[0]);

                    if (rtv.nErrorCode == 0)
                    {
                        rtv.nErrorCode = 0;
                        rtv.sErrorText = "Load Successful";

                        if (dumpRecord != null)
                        {
                            dumpRecord.JobId = context.BackgroundJob.Id;
                            dumpRecord.ErrorId = rtv.nErrorCode;
                            dumpRecord.Status = "Loaded";
                            repoDumpRepository.Update(dumpRecord);
                            await unitOfWork.Commit(loginUserId);
                            return rtv;
                        }

                    }

                }
                if (dumpRecord != null)
                {
                    dumpRecord.JobId = context.BackgroundJob.Id;
                    dumpRecord.ErrorId = rtv.nErrorCode;
                    dumpRecord.Status = "Error";
                    dumpRecord.ErrorMessage = "Error While Loading";
                    repoDumpRepository.Update(dumpRecord);
                    await unitOfWork.Commit(loginUserId);
                    return rtv;
                }
            }
            catch (Exception e)
            {
                if (dumpRecord != null)
                {
                    dumpRecord.JobId = context.BackgroundJob.Id;
                    dumpRecord.ErrorId = -1;
                    dumpRecord.Status = "Error";
                    dumpRecord.ErrorMessage = e.ToString();
                    repoDumpRepository.Update(dumpRecord);
                    await unitOfWork.Commit(loginUserId);
                }
            }

            return rtv;
        }

        public async Task<ReturnValues> ExecuteDump(Dumps dump, PerformContext context,int loginUserId)
        {
            var rtv = new ReturnValues();
            rtv.nErrorCode = -1;
            var dumpRecord = await repoDumpRepository.Get(o => o.Id == dump.Id);

            try
            {
                
                var sybaseLayer = new SybaseDataLayer();

                List<AseParameter> sp = new List<AseParameter>()
                {
                    new AseParameter() {ParameterName = "@pbNewInitiatization", AseDbType = AseDbType.Bit, Value= dump.DumpType},
                    new AseParameter() {ParameterName = "@psDumpType", AseDbType = AseDbType.VarChar, Value= dump.DumpType},
                    new AseParameter() {ParameterName = "@psDatabaseName", AseDbType = AseDbType.VarChar, Value= dump.DatabaseName},
                    new AseParameter() {ParameterName = "@psTapeName", AseDbType = AseDbType.VarChar, Value= dump.TapeName},
                    new AseParameter() {ParameterName = "@psFileName", AseDbType = AseDbType.Decimal, Value=dump.TapeName},
                    new AseParameter() {ParameterName = "@psUserName", AseDbType = AseDbType.VarChar, Value=CheckDBNullValue(dump.Password)}
                };

                
                var dt = await sybaseLayer.SqlDs("sp_dumpdb", sp, 0);

                if (dt != null && dt.Rows.Count > 0)
                {
                    rtv.nErrorCode = Convert.ToInt32(dt.Rows[0]);

                    if (rtv.nErrorCode == 0)
                    {
                        rtv.nErrorCode = 0;
                        rtv.sErrorText = "Dump Successful";

                        if (dumpRecord != null)
                        {
                            dumpRecord.JobId = context.BackgroundJob.Id;
                            dumpRecord.ErrorId = rtv.nErrorCode;
                            dumpRecord.Status = "Dumped";
                            repoDumpRepository.Update(dumpRecord);
                            await unitOfWork.Commit(loginUserId);
                            return rtv;
                        }

                    }
                   
                }
                if (dumpRecord != null)
                {
                    dumpRecord.JobId = context.BackgroundJob.Id;
                    dumpRecord.ErrorId = rtv.nErrorCode;
                    dumpRecord.Status = "Error";
                    dumpRecord.ErrorMessage = "Error While Dumping";
                    repoDumpRepository.Update(dumpRecord);
                    await unitOfWork.Commit(loginUserId);
                    return rtv;
                }
            }
            catch (Exception e)
            {
                if (dumpRecord != null)
                {
                    dumpRecord.JobId = context.BackgroundJob.Id;
                    dumpRecord.ErrorId = -1;
                    dumpRecord.Status = "Error";
                    dumpRecord.ErrorMessage = e.ToString();
                    repoDumpRepository.Update(dumpRecord);
                    await unitOfWork.Commit(loginUserId);
                }
            }

            return rtv;
        }
    }
}
