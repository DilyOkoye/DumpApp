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
    
    public partial class admClientProfile
    {
        [Key]
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public int EnforcePasswordChangeDays { get; set; }
        public Nullable<bool> EnforceStrngPwd { get; set; }
        public Nullable<int> SystemIdleTimeout { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string Status { get; set; }
        public Nullable<int> LoginCount { get; set; }
        public Nullable<bool> DumpWithPath { get; set; }
    }
}
