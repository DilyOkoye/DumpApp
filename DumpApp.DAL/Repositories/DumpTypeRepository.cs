using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;


namespace DumpApp.DAL.Repositories
{
    public class DumpTypeRepository : Repository<admDumpType>, IDumpTypeRepository
    {
        public DumpTypeRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }

    public interface IDumpTypeRepository : IRepository<admDumpType>
    {

    }
}
