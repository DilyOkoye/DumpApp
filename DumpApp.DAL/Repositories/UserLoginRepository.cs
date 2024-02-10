using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;


namespace DumpApp.DAL.Repositories
{
    public class UserLoginRepository : Repository<admUserLogin>, IUserLoginRepository
    {
        public UserLoginRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }

    public interface IUserLoginRepository : IRepository<admUserLogin>
    {

    }
}
