using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumpApp.DAL.Repositories
{
    public class TapeDeviceRespository : Repository<admTapeDevice>, ITapeDeviceRespository
    {
        public TapeDeviceRespository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface ITapeDeviceRespository : IRepository<admTapeDevice>
    {

    }
}
