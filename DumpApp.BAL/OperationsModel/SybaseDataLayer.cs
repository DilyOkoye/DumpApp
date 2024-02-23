using System;
using DumpApp.BAL.Utilities;
using System.Threading.Tasks;
using AdoNetCore.AseClient;

namespace DumpApp.BAL.OperationsModel
{
    public class SybaseDataLayer
    {
        public async Task<ReturnValues> SqlDs(string commandQuery)
        {
            var rtv = new ReturnValues();
            var de = new ConnectionDetails();
            var connstring = System.Configuration.ConfigurationManager.ConnectionStrings["sybconnection"].ToString();
            connstring = connstring.Replace("{{Data Source}}", de.Server);
            connstring = connstring.Replace("{{port}}", de.Port.ToString());
            connstring = connstring.Replace("{{database}}", de.DatabaseName);
            connstring = connstring.Replace("{{uid}}", de.Userid);
            connstring = connstring.Replace("{{pwd}}", de.Password);
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
