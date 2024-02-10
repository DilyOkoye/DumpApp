using System.Threading.Tasks;

namespace DumpApp.DAL.Interface
{
    public interface IUnitOfWork
    {
        Task<int> Commit(int userId);
        int CommitNonAsync(int userId);

    }
}
