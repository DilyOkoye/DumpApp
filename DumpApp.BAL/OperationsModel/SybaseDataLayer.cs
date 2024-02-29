using System;
using DumpApp.BAL.Utilities;
using System.Threading.Tasks;
using AdoNetCore.AseClient;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using DumpApp.DAL.Implementation;
using System.Collections.Generic;
using System.Data;
using static DumpApp.BAL.ReportModel.ReportModel;
using System.Data.Entity;

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
        public async Task<ReturnValues> SqlDs(string commandQuery, Dumps dump)
        {
            var startTime = DateTime.Now; // Log start time
            LogManager.SaveLog($"{commandQuery} execution start time: {startTime}");

            var rtv = new ReturnValues();
            var connstring = System.Configuration.ConfigurationManager.ConnectionStrings["sybconnection"].ToString();
            var database = await repoDatabase.Get(o => o.Id == dump.DatebaseId);

            if (dump.DumpType == "Internal")
            {
                var location = await repoLocation.Get(o => o.IsHeadOffice);
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


            LogManager.SaveLog("Before Connecting " + connstring);
            rtv.nErrorCode = -1;
            try
            {
                using (var connection = new AseConnection(connstring))
                {
                    connection.Open();
                    LogManager.SaveLog("Query" + commandQuery);
                    LogManager.SaveLog("Connected to the database successfully.");

                    using (var command = new AseCommand(commandQuery, connection))
                    {
                        var result = command.ExecuteNonQuery();
                        if (result == 0)
                        {
                            rtv.nErrorCode = 0;
                            rtv.sErrorText = "Database Loaded Successful";

                        }
                        LogManager.SaveLog(commandQuery + " successfully executed.");
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

            var endTime = DateTime.Now;
            LogManager.SaveLog($"{commandQuery} end time: {endTime}");

            // Optionally, log the duration
            LogManager.SaveLog($"{commandQuery} Total execution time: {endTime - startTime}");
            rtv.StartDateTime = startTime;
            rtv.EndDateTime = endTime;
            rtv.TotalTime = endTime - startTime;
            return rtv;
        }

        public async Task<ReturnValues> ExecuteDumpStoredProcedure(string storedProcedureName, List<AseParameter> parameterPasses, Dumps dump)
        {
            var startTime = DateTime.Now; // Log start time
            LogManager.SaveLog($"ExecuteDumpStoredProcedure execution start time: {startTime}");

            var rtv = new ReturnValues();
            rtv.nErrorCode = -1;
            rtv.sErrorText = "Dump command execution failed.";
            var connString = System.Configuration.ConfigurationManager.ConnectionStrings["sybconnection"].ConnectionString;
            var database = await repoDatabase.Get(o => o.Id == dump.DatebaseId);
            var location = dump.DumpType == "Internal"
                ? await repoLocation.Get(o => o.IsHeadOffice)
                : await repoLocation.Get(o => o.Id == dump.LocationId);

            if (location != null)
            {
                connString = connString.Replace("{{Data Source}}", location.Server)
                                       .Replace("{{port}}", location.Port.ToString())
                                       .Replace("{{database}}", database.Name)
                                       .Replace("{{uid}}", location.Username)
                                       .Replace("{{pwd}}", location.Password);
            }

            LogManager.SaveLog(connString);

            string responseMessage = "No response message set.";
            using (var theConn = new AseConnection(connString))
            {

                try
                {
                    await theConn.OpenAsync();
                    LogManager.SaveLog("Connection Opened Successfully");

                    using (var cmd = new AseCommand(storedProcedureName, theConn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add existing parameters to the command
                        foreach (var param in parameterPasses)
                        {
                            cmd.Parameters.Add(param);
                        }

                        // Add the @ResponseMessage parameter as output
                        var responseMsgParam = new AseParameter("@ResponseMessage", AseDbType.VarChar)
                        {
                            Direction = ParameterDirection.Output,
                            Size = 8000 // Adjust size as necessary
                        };
                        cmd.Parameters.Add(responseMsgParam);

                        LogManager.SaveLog("Before Execution");

                        // Execute the stored procedure without needing to read a DataReader
                        await cmd.ExecuteNonQueryAsync();

                        // Retrieve the output parameter value after command execution
                        responseMessage = responseMsgParam.Value?.ToString() ?? "No message returned.";
                        LogManager.SaveLog($"Stored Procedure Response: {responseMessage}");

                        switch (responseMessage)
                        {
                            case "No response message set.":
                                rtv.nErrorCode = -1;
                                rtv.sErrorText = "No response message set.";
                                break;
                            case "No message returned.":
                                rtv.nErrorCode = -1;
                                rtv.sErrorText = "No message returned.";
                                break;
                            case "Invalid input parameters.":
                                rtv.nErrorCode = -1;
                                rtv.sErrorText = "Invalid input parameters.";
                                break;
                            case "Dump command execution failed.":
                                rtv.nErrorCode = -1;
                                rtv.sErrorText = "Dump command execution failed.";
                                break;
                            case "Dump command executed successfully.":
                                rtv.nErrorCode = 0;
                                rtv.sErrorText = "Dump command executed successfully.";
                                break;
                        }

                    }
                }
                catch (Exception ex)
                {
                    var message = ex.Message ?? ex.InnerException?.Message;
                    LogManager.SaveLog($"Exception from Dump Procedure call: {message}");
                    rtv.nErrorCode = -1;
                    rtv.sErrorText = message;
                }
            }

            var endTime = DateTime.Now;
            LogManager.SaveLog($"Procedure execution end time: {endTime}");

            // Optionally, log the duration
            LogManager.SaveLog($"Total execution time: {endTime - startTime}");
            rtv.StartDateTime = startTime;
            rtv.EndDateTime = endTime;
            rtv.TotalTime = endTime - startTime;
            return rtv;
        }


        public async Task<ReturnValues> ExecuteLoadStoredProcedure(string storedProcedureName, List<AseParameter> parameterPasses, Dumps dump)
        {
            var startTime = DateTime.Now; // Log start time
            LogManager.SaveLog($"ExecuteLoadStoredProcedure execution start time: {startTime}");


            var rtv = new ReturnValues();
            rtv.nErrorCode = -1;
            rtv.sErrorText = "Load command execution failed.";
            var connString = System.Configuration.ConfigurationManager.ConnectionStrings["sybconnection"].ConnectionString;
            var database = await repoDatabase.Get(o => o.Id == dump.DatebaseId);
            var location = dump.DumpType == "Internal"
                ? await repoLocation.Get(o => o.IsHeadOffice)
                : await repoLocation.Get(o => o.Id == dump.LocationId);

            if (location != null)
            {
                connString = connString.Replace("{{Data Source}}", location.Server)
                                       .Replace("{{port}}", location.Port.ToString())
                                       .Replace("{{database}}", database.Name)
                                       .Replace("{{uid}}", location.Username)
                                       .Replace("{{pwd}}", location.Password);
            }

            LogManager.SaveLog(connString);

            string responseMessage = "No response message set.";
            using (var theConn = new AseConnection(connString))
            {

                try
                {
                    await theConn.OpenAsync();
                    LogManager.SaveLog("Connection Opened Successfully");


                    using (var cmd = new AseCommand(storedProcedureName, theConn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add existing parameters to the command
                        foreach (var param in parameterPasses)
                        {
                            cmd.Parameters.Add(param);
                        }

                        // Add the @ResponseMessage parameter as output
                        var responseMsgParam = new AseParameter("@ResponseMessage", AseDbType.VarChar)
                        {
                            Direction = ParameterDirection.Output,
                            Size = 8000 // Adjust size as necessary
                        };
                        cmd.Parameters.Add(responseMsgParam);

                        LogManager.SaveLog("Before Execution");

                        // Execute the stored procedure without needing to read a DataReader
                        await cmd.ExecuteNonQueryAsync();

                        // Retrieve the output parameter value after command execution
                        responseMessage = responseMsgParam.Value?.ToString() ?? "No message returned.";
                        LogManager.SaveLog($"Stored Procedure Response: {responseMessage}");

                        switch (responseMessage)
                        {
                            case "No response message set.":
                                rtv.nErrorCode = -1;
                                rtv.sErrorText = "No response message set.";
                                break;
                            case "No message returned.":
                                rtv.nErrorCode = -1;
                                rtv.sErrorText = "No message returned.";
                                break;
                            case "Invalid input parameters.":
                                rtv.nErrorCode = -1;
                                rtv.sErrorText = "Invalid input parameters.";
                                break;
                            case "Dump command execution failed.":
                                rtv.nErrorCode = -1;
                                rtv.sErrorText = "Load command execution failed.";
                                break;
                            case "Dump command executed successfully.":
                                rtv.nErrorCode = 0;
                                rtv.sErrorText = "Load command executed successfully.";
                                break;
                        }

                    }
                }
                catch (Exception ex)
                {
                    var message = ex.Message ?? ex.InnerException?.Message;
                    LogManager.SaveLog($"Exception from Load Procedure call: {message}");
                    rtv.nErrorCode = -1;
                    rtv.sErrorText = message;

                }
            }

            var endTime = DateTime.Now;
            LogManager.SaveLog($"Procedure execution end time: {endTime}");

            // Optionally, log the duration
            LogManager.SaveLog($"Total execution time: {endTime - startTime}");
            rtv.StartDateTime = startTime;
            rtv.EndDateTime = endTime;
            rtv.TotalTime = endTime - startTime;
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
