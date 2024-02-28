using DumpApp.BAL.Utilities;

namespace DumpApp.BAL.AdminModel
{
    public class ChangePasswordModel
    {
        public string LoginID { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Status { get; set; }
        public ReturnValues rv { get; set; }
        public int Menuid { get; set; }
        public string Url { set; get; }
    }
}
