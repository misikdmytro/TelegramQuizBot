using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using QuizBot.Api.Models;

namespace QuizBot.Api.Repositories.Interfaces
{
    public interface IDbRepository<TEntity>
        where TEntity: class, IIDentifiable
    {
        Task<TEntity[]> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task RemoveAsync(int id);
        Task ClearAsync();
    }
}
