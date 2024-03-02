using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Repositories
{
    public class LicenseRepository : Repository<admLicenseSetUp>, ILicenseRepository
    {
        public LicenseRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }
    public interface ILicenseRepository : IRepository<admLicenseSetUp>
    {

    }
}
