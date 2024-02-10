using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{
   

    public class StatusItemRepository : Repository<admStatusItem>, IStatusItemRepository
    {
        public StatusItemRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }

    public interface IStatusItemRepository : IRepository<admStatusItem>
    {

    }
}
