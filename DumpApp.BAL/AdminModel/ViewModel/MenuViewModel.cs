using System.Collections.Generic;
using DumpApp.DAL;

namespace DumpApp.BAL.AdminModel.ViewModel
{
    public class MenuViewModel
    {
        public int UserId { set; get; }
        public int roleid { set; get; }
        public IEnumerable<admMenuControl> Menu { set; get; }
        public IEnumerable<admRoleAssignment> MenuAssign { set; get; }
        public Header oHeader { set; get; }
        public string FullName { get; set; }
        public string LastLoginTime { get; set; }
        public string date { get; set; }
        public string UsersRole { get; set; }
        public string status { get; set; }
    }
}
