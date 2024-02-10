using DumpApp.DAL.Context;
using DumpApp.DAL.Interface;

namespace DumpApp.DAL.Implementation
{
    public class DbFactory : Disposable, IDbFactory
    {
        DumpAppContext dbContext;

        public DumpAppContext Init()
        {
            return dbContext ?? (dbContext = new DumpAppContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }


    }
}
