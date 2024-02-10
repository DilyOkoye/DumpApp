using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;

namespace DumpApp.BAL.AdminModel
{
    public class PriviledgeAssignmentManager
    {
        public bool CanEdit { get; set; }
        public bool CanAdd { get; set; }
        public bool CanAuth { get; set; }
        public bool CanView { get; set; }
        public bool IsGlobalSupervisor { get; set; }

    }
    public class PriviledgeManager
    {
        private int _MenuId;
        private int _RoleId;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        private readonly RoleAssignmentRepo repoRoleAssign;
        private PriviledgeAssignmentManager primanager;

        public PriviledgeManager(int MenuId, int RoleId)
        {
            _MenuId = MenuId;
            _RoleId = RoleId;
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoRoleAssign = new RoleAssignmentRepo(idbfactory);

        }
        public PriviledgeAssignmentManager AssignRoleToUser()
        {
            primanager = new PriviledgeAssignmentManager();

            var d = repoRoleAssign.GetNonAsync(p => p.menu_id == _MenuId && p.role_id == _RoleId);
            if (d != null)
            {
                primanager.CanAdd = d.can_add == 1 ? true : false;
                primanager.CanEdit = d.can_edit == 1 ? true : false;
                primanager.CanAuth = d.can_auth == 1 ? true : false;
                primanager.CanView = d.can_view == 1 ? true : false;
                primanager.IsGlobalSupervisor = d.is_global_supervisor == 1 ? true : false;
                return primanager;
            }
            return null;
        }
    }
}
