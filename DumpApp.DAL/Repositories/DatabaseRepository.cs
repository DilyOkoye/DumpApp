using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{
    public class DatabaseRepository : Repository<admDatabase>, IDatabaseRepository
    {
        public DatabaseRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IDatabaseRepository : IRepository<admDatabase>
    {

    }
}
