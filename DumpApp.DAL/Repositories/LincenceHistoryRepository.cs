using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{
    public class LincenceHistoryRepository : Repository<admLicenseSetUpHistory>, ILincenceHistoryRepository
    {
        public LincenceHistoryRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }
    public interface ILincenceHistoryRepository : IRepository<admLicenseSetUpHistory>
    {

    }
}
