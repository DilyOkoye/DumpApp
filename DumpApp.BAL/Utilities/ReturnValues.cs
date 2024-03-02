using System;

namespace DumpApp.BAL.Utilities
{
    public class ReturnValues
    {
        public int? nErrorCode { set; get; }
        public string sErrorText { set; get; }

        public string Url { set; get; }

        public string UserKey { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
        
         public TimeSpan TotalTime { get; set; }
    }
}
