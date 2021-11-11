using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Play.Common
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T item);
        Task DeleteAsync(Guid id);
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByIdAsync(Expression<Func<T, bool>> filter);
        Task UpdateAsync(T item);
    }
}