using System;
using System.Collections.Generic;
using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DumpApp.BAL.Utilities;

namespace DumpApp.BAL.ReportModel
{
    public class ReportModel
    {
        private readonly IUserProfileRepository repoUserProfile;
        private readonly IClientProfileRepository repoClientProfile;
        private readonly IStatusItemRepository repoStatusItem;
        
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        private AdminViewModel adminViewModel;
        IDbConnection db = null;

        public ReportModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoUserProfile = new UserProfileRepository(idbfactory);
            repoClientProfile = new ClientProfileRepository(idbfactory);
            repoStatusItem = new StatusItemRepository(idbfactory);
            adminViewModel = new AdminViewModel();
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnectionProc"].ToString());
        }

        public async Task<List<Dump>> FetchLoad(string fromDate, string toDate)
        {

            DynamicParameters param = new DynamicParameters();

            if (fromDate != null && toDate != null)
            {

                param.Add("@pdtStartDate", Convert.ToDateTime(fromDate));
                param.Add("@pdtEndDate", Convert.ToDateTime(toDate));

                var result = await db.QueryAsync<Dump>(sql: "Isp_Load",
                    param: param, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            return null;
        }

        public async Task<List<Dump>> FetchPasswordedDump(string fromDate, string toDate)
        {

            DynamicParameters param = new DynamicParameters();

            if (fromDate != null && toDate != null)
            {

                param.Add("@pdtStartDate", Convert.ToDateTime(fromDate));
                param.Add("@pdtEndDate", Convert.ToDateTime(toDate));

                var result = await db.QueryAsync<Dump>(sql: "Isp_Dump",
                    param: param, commandType: CommandType.StoredProcedure);

                var enumerable = result as Dump[] ?? result.ToArray();
                foreach (var val in enumerable)
                {

                    val.Password = val.Password == null ? null : Cryptors.Decrypt(val.Password, "DumpApp");
                }
                return enumerable.ToList();
            }
            return null;
        }

        public async Task<List<Dump>> FetchDump(string fromDate, string toDate)
        {

            DynamicParameters param = new DynamicParameters();

            if (fromDate != null && toDate != null)
            {

                param.Add("@pdtStartDate", Convert.ToDateTime(fromDate));
                param.Add("@pdtEndDate", Convert.ToDateTime(toDate));

                var result = await db.QueryAsync<Dump>(sql: "Isp_Dump",
                    param: param, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            return null;
        }

        public async Task<List<AuditTrail>> FetchAuditTrail(string fromDate, string toDate)
        {

            DynamicParameters param = new DynamicParameters();

            if (fromDate != null && toDate != null)
            {

                param.Add("@pdtStartDate", Convert.ToDateTime(fromDate));
                param.Add("@pdtEndDate", Convert.ToDateTime(toDate));

                var result = await db.QueryAsync<AuditTrail>(sql: "Isp_AuditTrail",
                    param: param, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            return null;
        }
        public class AuditTrail
        {
            public int userId { get; set; }
            public string FullName { get; set; }
            public DateTime eventdateutc { get; set; }
            public string eventtype { get; set; }
            public string LoginId { get; set; }
            public string tablename { get; set; }
            public string recordid { get; set; }
            public string columnname { get; set; }
            public string originalvalue { get; set; }
            public string newvalue { get; set; }

        }

        public class Dump
        {
            public int Id { get; set; }
            public string TapeIdentifier { get; set; }
            public string TapeDescription { get; set; }
            public string DumpName { get; set; }

            public string DumpDescription { get; set; }
            public string DumpType { get; set; }
            public string Filename { get; set; }
            public string TapeType { get; set; }
            public string Password { get; set; }
            public DateTime DumpDate { get; set; }
            public string DateCreated { get; set; }
            public string Status { get; set; }
            public string CreatedBy { get; set; }


            public bool TapeTypeCheck { get; set; }

            public int? LocationId { get; set; }
            public int? TapeDeviceId { get; set; }
            public int? DatebaseId { get; set; }
            public string DatabaseName { get; set; }
            public string TapeName { get; set; }
            public string locationName { get; set; }
            public string DeviceName { get; set; }

            public string FullName { get; set; }

            public string JobId { get; set; }

            public string ErrorMessage { get; set; }

        }
    }
}
