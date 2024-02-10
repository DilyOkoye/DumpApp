using System;
using DumpApp.DAL.Context;

namespace DumpApp.DAL.Interface
{
    public interface IDbFactory : IDisposable
    {
        DumpAppContext Init();
    }
}
