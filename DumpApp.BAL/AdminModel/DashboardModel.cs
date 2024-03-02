using System;
using System.Collections.Generic;
using System.Data;
using DumpApp.BAL.AdminModel.ViewModel;
using DumpApp.BAL.OperationsModel;
using DumpApp.BAL.Utilities;

namespace DumpApp.BAL.AdminModel
{
    public  class DashboardModel
    {
        private Dashboard dashboard = null;
        private AdminViewModel adminviewModel = null;

        public DashboardModel()
        {
            adminviewModel = new AdminViewModel();
            dashboard = new Dashboard();
        }
        public Dashboard ListOfDumps()
        {
            double totalDuration = 0;
            int totalTransactions = 0;
            int successfulTransactions = 0;
            
            List<Dumps> dumps = new List<Dumps>();
            var result = Cryptors.GetTop20DumpRecords();
            foreach (DataRow row in result.Rows)
            {
                totalTransactions++; 
                Dumps dump = new Dumps
                {
                    DumpName = row["DumpName"].ToString(),
                    Filename = row["Filename"].ToString(),
                    DatebaseId = row["DatebaseId"].ToString(),
                    TapeIdentifier = row["TapeIdentifier"].ToString(),
                    DumpDate = row.IsNull("DumpDate") ? "" : $"{row["DumpDate"]:F}",
                    Duration = row["TotalDuration"].ToString(),
                    Status = row["Status"].ToString(),
                };

                if (!row.IsNull("TotalDuration"))
                {
                    bool success = DateTime.TryParse(row["TotalDuration"].ToString(), out DateTime dateTime);
                    if (success)
                    {
                        totalDuration += dateTime.Second; // Sum total seconds
                    }
                }
                
                if (row["Status"].ToString() == "Dumped")
                {
                    successfulTransactions++;
                }

                dumps.Add(dump);
            }

            double successPercentage = 0;
            if (totalTransactions > 0)
            {
                successPercentage = (double)successfulTransactions / totalTransactions * 100;
            }

            double averageDuration = 0;
            if (totalTransactions > 0) 
            {
                averageDuration = totalDuration / totalTransactions;
            }

            dashboard.dumps = dumps;
            dashboard.dumpPercentageRate = Math.Ceiling(successPercentage);
            dashboard.dumpAverageDuration = Math.Ceiling(averageDuration);
            return dashboard;
        }


        public Dashboard ListOfLoad()
        {
            double totalDuration = 0;
            int totalTransactions = 0;
            int successfulTransactions = 0;

            
            List<Dumps> loads = new List<Dumps>();
            var result = Cryptors.GetTop20LoadRecords();
            foreach (DataRow row in result.Rows)
            {
                totalTransactions++;
                Dumps dump = new Dumps
                {
                    DumpName = row["DumpName"].ToString(),
                    Filename = row["Filename"].ToString(),
                    DatebaseId = row["DatebaseId"].ToString(),
                    TapeIdentifier = row["TapeIdentifier"].ToString(),
                    DumpDate = row.IsNull("DumpDate") ? "" : $"{row["DumpDate"]:F}",
                    Duration = row["TotalDuration"].ToString(),
                    Status = row["Status"].ToString(),
                };

               if (!row.IsNull("TotalDuration"))
                {
                    bool success = DateTime.TryParse(row["TotalDuration"].ToString(), out DateTime dateTime);
                    if (success)
                    {
                        totalDuration += dateTime.Second; // Sum total seconds
                    }
                }

                if (row["Status"].ToString() == "Loaded")
                {
                    successfulTransactions++;
                }

                loads.Add(dump);
            }

            double successPercentage = 0;
            if (totalTransactions > 0)
            {
                successPercentage = (double)successfulTransactions / totalTransactions * 100;
            }
            double averageDuration = 0;
            if (totalTransactions > 0) 
            {
                averageDuration = totalDuration / totalTransactions;
            }

            dashboard.load = loads;
            dashboard.loadPercentageRate = Math.Ceiling(successPercentage);
            dashboard.loadAverageDuration = Math.Ceiling(averageDuration);
            return dashboard;
        }
        
    }
}
