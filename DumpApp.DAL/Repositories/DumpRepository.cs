using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{
    public class DumpRepository : Repository<admDump>, IDumpRepository
    {
        public DumpRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }

    public interface IDumpRepository : IRepository<admDump>
    {

    }
}
