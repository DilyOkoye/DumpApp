using System;

namespace DumpApp.BAL.OperationsModel
{
    public class Dumps
    {
        public int Id { get; set; }
        public string TapeIdentifier { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DumpType { get; set; }
        public string TapeType { get; set; }
        public string DumpDate { get; set; }
        public string DateCreated { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
    }
}
