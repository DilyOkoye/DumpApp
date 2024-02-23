using DumpApp.BAL.AdminModel;
using DumpApp.BAL.OperationsModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DumpApp.BAL.ReportModel.ViewModel
{
    public class ReportViewModel
    {
        public Dumps dumps { get; set; }
        public admDump AdmDump { get; set; }
        public ReturnValues rv { get; set; }
        public List<Dumps> ListOfDumps { set; get; }
        public List<Dumps> ListOfLoad { set; get; }
        public PriviledgeAssignmentManager roleManager { get; set; }
        public IEnumerable<SelectListItem> drpLocation { get; set; }
        public IEnumerable<SelectListItem> drpDatabase { get; set; }
        public IEnumerable<SelectListItem> drpTapeDevice { get; set; }
        public int menuid { get; set; }
        public string Url { get; set; }

        public string ButtonType { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public admUserProfile admUserProfile { get; set; }

    }
}
