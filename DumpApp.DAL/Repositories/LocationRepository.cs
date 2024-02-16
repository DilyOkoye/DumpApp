using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{

    public class LocationRepository : Repository<admLocation>, ILocationRepository
    {
        public LocationRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface ILocationRepository : IRepository<admLocation>
    {

    }
}
