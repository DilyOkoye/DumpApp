using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{
    
    public class ClientProfileRepository : Repository<admClientProfile>, IClientProfileRepository
    {
        public ClientProfileRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }

    public interface IClientProfileRepository : IRepository<admClientProfile>
    {

    }
}
