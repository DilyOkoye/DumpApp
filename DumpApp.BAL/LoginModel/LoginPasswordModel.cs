using System.ComponentModel.DataAnnotations;

namespace DumpApp.BAL.LoginModel
{
    public class LoginPasswordModel
    {
        //[Required(ErrorMessage = "Password Required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public string Username { get; set; }
    }
}
