using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Conan.Domain
{
    public interface IRepository<T> where T : RootEntity
    {
        Task<T> ByIdAsync(string id);
        Task<T> SingleAsync(Expression<Func<T, bool>> predicate);
        Task SaveAsync(T entity);
        Task DeleteAsync(T entity);

        IQueryable<T> Query();
    }
}
