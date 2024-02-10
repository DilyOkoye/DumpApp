

using System.Web.UI.WebControls;

namespace DumpApp.BAL.LoginModel.ViewModel
{
    public class LoginViewModel
    {
        public LoginPasswordModel LoginPwdModel { get; set; }

        public Login LoginModel { get; set; }
        public LoginReturnProperty LoginReturnProperty { get; set; }
    }
}
