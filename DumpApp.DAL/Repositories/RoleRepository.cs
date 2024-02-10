using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{
   
    public class RoleRepository : Repository<admRole>, IRoleRepository
    {
        public RoleRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }

    public interface IRoleRepository : IRepository<admRole>
    {

    }
}
