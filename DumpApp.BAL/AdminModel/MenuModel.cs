using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using System.Collections.Generic;
using System.Linq;
using DumpApp.DAL;
using DumpApp.DAL.Repositories;

namespace DumpApp.BAL.AdminModel
{
    public class MenuModel
    {

        private readonly MenuControlRepo _menuControl;
        private readonly RoleAssignmentRepo _MenuAssignment;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        public MenuModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            _menuControl = new MenuControlRepo(idbfactory);
            _MenuAssignment = new RoleAssignmentRepo(idbfactory);

        }

        public IEnumerable<admMenuControl> GetMainMenu()
        {
            var menu = _menuControl.GetManyNonAsync(c => c.status == "A");
            return menu.ToList();
        }

        public IEnumerable<admRoleAssignment> GetMenuAssignmentAdmin()
        {
            var menu = _MenuAssignment.GetAllNonAsync();
            return menu;
        }


    }
}
