using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{
    public class LoadRepository : Repository<admLoad>, ILoadRepository
    {
        public LoadRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }

    public interface ILoadRepository : IRepository<admLoad>
    {

    }
}
