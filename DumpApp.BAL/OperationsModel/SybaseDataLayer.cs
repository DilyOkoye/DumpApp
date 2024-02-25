using System;
using DumpApp.BAL.Utilities;
using System.Threading.Tasks;
using AdoNetCore.AseClient;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using DumpApp.DAL.Implementation;

namespace DumpApp.BAL.OperationsModel
{
    public class SybaseDataLayer
    {
        private readonly ILocationRepository repoLocation;
        private readonly IDatabaseRepository repoDatabase;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;

        public SybaseDataLayer()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoLocation = new LocationRepository(idbfactory);
            repoDatabase = new DatabaseRepository(idbfactory);
        }
        public async Task<ReturnValues> SqlDs(string commandQuery,Dumps dump)
        {
            var rtv = new ReturnValues();
            var de = new ConnectionDetails();
            var connstring = System.Configuration.ConfigurationManager.ConnectionStrings["sybconnection"].ToString();
            var database = await repoDatabase.Get(o => o.Id == dump.DatebaseId);

            if (dump.DumpType == "Internal")
            {
                var location =await repoLocation.Get(o => o.IsHeadOffice);
                if (location != null)
                {
                    connstring = connstring.Replace("{{Data Source}}", location.Server);
                    connstring = connstring.Replace("{{port}}", location.Port.ToString());
                    connstring = connstring.Replace("{{database}}", database.Name);
                    connstring = connstring.Replace("{{uid}}", location.Username);
                    connstring = connstring.Replace("{{pwd}}", location.Password);
                }
            }
            else
            {
                var location = await repoLocation.Get(o => o.Id == dump.LocationId);
                if (location != null)
                {
                    connstring = connstring.Replace("{{Data Source}}", location.Server);
                    connstring = connstring.Replace("{{port}}", location.Port.ToString());
                    connstring = connstring.Replace("{{database}}", database.Name);
                    connstring = connstring.Replace("{{uid}}", location.Username);
                    connstring = connstring.Replace("{{pwd}}", location.Password);
                }
            }

            
            LogManager.SaveLog("Before Connecting " +connstring);
            rtv.nErrorCode = -1;
            try
            {
                using (var connection = new AseConnection(connstring))
                {
                    connection.Open();
                    LogManager.SaveLog(commandQuery);
                    LogManager.SaveLog("Connected to the database successfully.");

                    using (var command = new AseCommand(commandQuery, connection))
                    {
                        var result =command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            rtv.nErrorCode = 0;
                            rtv.sErrorText = "Database Dump Successful";
                            return rtv;
                        }
                        LogManager.SaveLog("Database dump successfully executed.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.SaveLog($"StackTrace exception error occurred: {ex.StackTrace}");
                LogManager.SaveLog($"Inner exception error occurred: {ex.InnerException}");
                LogManager.SaveLog($"An error occurred: {ex.Message}");
                rtv.sErrorText = ex.Message;
            }

            return rtv;
        }

        public class ConnectionDetails
        {
            public string Server { set; get; }
            public int Port { set; get; }
            public string DatabaseName { set; get; }
            public string Userid { set; get; }
            public string Password { set; get; }
        }
    }
}
