using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{

    public class MenuControlRepo : Repository<admMenuControl>, IMenuControlRepo
    {
        public MenuControlRepo(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IMenuControlRepo : IRepository<admMenuControl>
    {

    }
}

