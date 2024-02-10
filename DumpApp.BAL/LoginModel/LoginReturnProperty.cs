namespace DumpApp.BAL.LoginModel
{
    public class LoginReturnProperty
    {
        public string EnforcePassChange { get; set; }
        public string FullName { get; set; }
        public int LicenseErrCode { get; set; }
        public string LicenseErrMsg { get; set; }
        public long UserId { get; set; }
        public string Username { set; get; }
        public string Status { set; get; }
        public int? RoleId { get; set; }
        public string LastLoginDate { get; set; }
        public short ErrorCode { get; set; }
        public string FriendlyErrorMessage { get; set; }
        public ErrorDisplay ErrorDisplay { get; set; }
        public string Url { get; set; }
    }
}
