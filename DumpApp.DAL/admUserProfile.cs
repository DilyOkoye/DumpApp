//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DumpApp.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class admUserProfile
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string LoginId { get; set; }
        public bool IsFirstLogin { get; set; }
        public string IsSupervisor { get; set; }
        public bool LoggedOn { get; set; }
        public Nullable<int> RoleId { get; set; }
        public System.DateTime PasswordExpiryDate { get; set; }
        public string Status { get; set; }
        public string RoleName { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNo { get; set; }
        public string EnforcePswdChange { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string Password { get; set; }
        public Nullable<short> loginstatus { get; set; }
        public Nullable<short> lockcount { get; set; }
        public Nullable<short> logincount { get; set; }
    }
}