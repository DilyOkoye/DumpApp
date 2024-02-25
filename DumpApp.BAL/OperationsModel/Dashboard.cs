using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumpApp.BAL.OperationsModel
{
    public class Dashboard
    {
        public List<Dumps> dumps { get; set; }

        public List<Dumps> load { get; set; }

        public double dumpPercentageRate { get; set; }

        public double loadPercentageRate { get; set; }

        public double dumpAverageDuration { get; set; }

        public double loadAverageDuration { get; set; }
    }
}
