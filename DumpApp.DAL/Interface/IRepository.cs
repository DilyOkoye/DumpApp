using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DumpApp.DAL.Interface
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        T GetById(object id);
        T GetNonAsync(Expression<Func<T, bool>> where);
        Task <T> Get(Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> GetAll();
        IEnumerable<T> GetAllNonAsync();
        Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where);
        IEnumerable<T> GetManyNonAsync(Expression<Func<T, bool>> where);
        IEnumerable<T> ExecuteStoredProcedure(string procName, params object[] param);


    }
}
