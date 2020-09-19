using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace sample.Abstract.IREPO
{
    public interface IRepo<T> where T : class 
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(object id);
        Task<IEnumerable<T>> Filter(Func<T, bool> predicate);
        Task Save(T entity);
        Task Update(T entity);
        Task Delete(object id);
        

    }
}