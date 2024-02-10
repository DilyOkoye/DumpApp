using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{
  

    public class UserProfileRepository : Repository<admUserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }

    public interface IUserProfileRepository : IRepository<admUserProfile>
    {

    }
}
