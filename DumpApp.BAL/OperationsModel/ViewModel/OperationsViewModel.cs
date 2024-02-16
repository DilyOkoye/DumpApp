using DumpApp.BAL.AdminModel;
using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DumpApp.BAL.OperationsModel.ViewModel
{
    public class OperationsViewModel
    {
        public Dumps dumps { get; set; }
        public ReturnValues rv { get; set; }
        public List<Dumps> ListOfDumps { set; get; }
        public PriviledgeAssignmentManager roleManager { get; set; }
        public IEnumerable<SelectListItem> drpLocation { get; set; }
        public IEnumerable<SelectListItem> drpDatabase { get; set; }
        public IEnumerable<SelectListItem> drpTapeDevice{ get; set; }
        public int menuid { get; set; }
        public string Url { get; set; }

        public string ButtonType { get; set; }

    }
}
