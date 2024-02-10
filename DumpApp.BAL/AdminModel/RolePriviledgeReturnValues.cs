namespace DumpApp.BAL.AdminModel
{
    public class RolePriviledgeReturnValues
    {
        public class CheckBoxRoleAssignmentDetailsSystem
        {
            public int MenuId { set; get; }
            public bool CanView { set; get; }
            public bool CanAdd { set; get; }
            public bool CanEdit { set; get; }
            public bool CanDelete { set; get; }
            public bool CanAuth { set; get; }
            public bool IsGlobalSupervisor { set; get; }
            public string MenuName { set; get; }
        }
    }
}
