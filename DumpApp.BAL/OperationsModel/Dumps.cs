using System;

namespace DumpApp.BAL.OperationsModel
{
    public class Dumps
    {
        public int Id { get; set; }
        public string TapeIdentifier { get; set; }
        public string TapeDescription { get; set; }
        public string DumpName { get; set; }
        public string DumpDescription { get; set; }
        public string DumpType { get; set; }
        public string Filename { get; set; }
        public string TapeType { get; set; }
        public string Password { get; set; }
        public string DumpDate { get; set; }
        public string DateCreated { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }

        public bool DumpTypeCheck{ get; set; }

        public bool TapeTypeCheck { get; set; }
    }
}
