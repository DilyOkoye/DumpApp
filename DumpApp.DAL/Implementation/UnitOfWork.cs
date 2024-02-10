using System.Threading.Tasks;
using DumpApp.DAL.Context;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory dbFactory;
        private DumpAppContext dbContext;

        public UnitOfWork(IDbFactory dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public DumpAppContext DbContext
        {
            get { return dbContext ?? (dbContext = dbFactory.Init()); }
        }

        public async Task<int> Commit(int userId)
        {

            return await DbContext.Commit(userId);
        }

        public int CommitNonAsync(int userId)
        {

            return  DbContext.CommitNonAsync(userId);
        }
    }
}
