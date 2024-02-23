using System;
using DumpApp.BAL.Utilities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using AdoNetCore.AseClient;

namespace DumpApp.BAL.OperationsModel
{
    public class SybaseDataLayer
    {
        public async Task<ReturnValues> TestSqlDs(string commandQuery)
        {
            var rtv = new ReturnValues();
            var de = new ConnectionDetails();
            var connstring = System.Configuration.ConfigurationManager.ConnectionStrings["sybconnection"].ToString();
            connstring = connstring.Replace("{{Data Source}}", de.Server);
            connstring = connstring.Replace("{{port}}", de.Port.ToString());
            connstring = connstring.Replace("{{database}}", de.DatabaseName);
            connstring = connstring.Replace("{{uid}}", de.Userid);
            connstring = connstring.Replace("{{pwd}}", de.Password);
            LogManager.SaveLog(connstring);
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
                LogManager.SaveLog($"An error occurred: {ex.Message}");
                rtv.sErrorText = ex.Message;
            }

            return rtv;
        }


        public async Task<DataTable> SqlDs(string SqlString, List<AseParameter> parameterPasses, int i)
        {
            var de = new ConnectionDetails();
            var connstring = System.Configuration.ConfigurationManager.ConnectionStrings["sybconnection"].ToString();
            connstring = connstring.Replace("{{Data Source}}", de.Server);
            connstring = connstring.Replace("{{port}}", de.Port.ToString());
            connstring = connstring.Replace("{{database}}", de.DatabaseName);
            connstring = connstring.Replace("{{uid}}", de.Userid);
            connstring = connstring.Replace("{{pwd}}", de.Password);
            LogManager.SaveLog(connstring);
            using (AseConnection theCons = new AseConnection(connstring))
            {
                var ds = new DataTable();

                theCons.Open();
                LogManager.SaveLog("EConnection Opened Sucessfully");
                try
                {

                    AseCommand cmd = new AseCommand();
                    cmd.Connection = theCons;

                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.Text;//i == 0 ? CommandType.StoredProcedure : CommandType.Text;
                                                       //if (parameterPasses != null)
                                                       //  cmd.Parameters.AddRange(parameterPasses.ToArray());
                    string qry = string.Empty;
                    List<string> sep = new List<string>();
                    sep.Add("AnsiString");// DateTime";
                    sep.Add("Date");
                    string fdf = string.Empty;
                    if (parameterPasses != null)
                    {
                        foreach (var t in parameterPasses.ToArray())
                        {
                            //sep=if t.DbType in () then "'" else ""
                            //sep = string.Empty;
                            fdf = string.Empty;

                            if (sep.Contains(t.DbType.ToString()))
                            {
                                fdf = t.Value == null ? "NULL," : (t.Value == DBNull.Value ? "NULL," : "'" + t.Value.ToString() + "',");
                            }
                            else
                            {
                                if (t.Value == null || t.Value == DBNull.Value)
                                {
                                    fdf = "NULL,";
                                }
                                else
                                {
                                    fdf = t.Value.ToString() + ",";
                                }
                            }

                            qry += t.ToString() + "=" + fdf;

                        }

                        cmd.CommandText = SqlString + " " + qry.TrimEnd(',');
                        LogManager.SaveLog("Parameter is not null " + cmd.CommandText);
                    }
                    else
                    {
                        cmd.CommandText = SqlString;
                        LogManager.SaveLog("Parameter is null " + cmd.CommandText);
                    }

                    LogManager.SaveLog("Before Execution");

                    IDataReader reader = await cmd.ExecuteReaderAsync();

                    ds.Load(reader);

                    LogManager.SaveLog("Reader Read Sucessfully");
                    reader.Close();
                    theCons.Close();

                }
                catch (Exception ex)
                {
                    LogManager.SaveLog("Exception from Procedure call " + ex.Message == null ? ex.InnerException.Message : ex.Message);
                }

                return ds;
            }
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
