using DumpApp.BAL.Utilities;
using DumpApp.DAL;
using System.Collections.Generic;

namespace DumpApp.BAL.OperationsModel.ViewModel
{
    public class OperationsViewModel
    {
        public Dumps dumps { get; set; }
        public ReturnValues rv { get; set; }
        public List<Dumps> ListOfDumps { set; get; }
        public int menuid { get; set; }
    }
}
