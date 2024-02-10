using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;

namespace DumpApp.BAL.Utilities
{
    public class SessionManager
    {
        private readonly IClientProfileRepository clientProfile;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        public SessionManager()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            clientProfile = new ClientProfileRepository(idbfactory);
        }
        public short GetSessionTime()
        {
            return (short)clientProfile.GetNonAsync(null).SystemIdleTimeout;
        }
    }
}
