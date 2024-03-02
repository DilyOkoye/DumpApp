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

        public bool DumpTypeCheck { get; set; }

        public bool TapeTypeCheck { get; set; }

        public int? LocationId { get; set; }
        public int? TapeDeviceId { get; set; }
        public string DatebaseId { get; set; }
        public string ErrorMessage { get; set; }
        public string DatabaseName { get; set; }
        public string TapeName { get; set; }
        public string TapeName1 { get; set; }
        public string Duration { get; set; }

        public string Operation { get; set; }
        public string OperationType { get; set; }
    }
}