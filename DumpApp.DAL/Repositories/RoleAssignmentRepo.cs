using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{

    public class RoleAssignmentRepo : Repository<admRoleAssignment>, IRoleAssignmentRepo
    {
        public RoleAssignmentRepo(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IRoleAssignmentRepo : IRepository<admRoleAssignment>
    {

    }
}

