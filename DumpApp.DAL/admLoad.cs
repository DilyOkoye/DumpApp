//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DumpApp.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class admLoad
    {
        [Key]
        public int Id { get; set; }
        public string TapeIdentifier { get; set; }
        public string TapeDescription { get; set; }
        public string DumpName { get; set; }
        public string DumpDescription { get; set; }
        public Nullable<int> DumpType { get; set; }
        public string Filename { get; set; }
        public string TapeType { get; set; }
        public string Password { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> TapeDeviceId { get; set; }
        public string DatebaseId { get; set; }
        public string JobId { get; set; }
        public Nullable<int> ErrorId { get; set; }
        public string ErrorMessage { get; set; }
        public Nullable<System.DateTime> DumpDate { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string TotalDuration { get; set; }
        public string Status { get; set; }
        public Nullable<int> CreatedBy { get; set; }
    }
}
