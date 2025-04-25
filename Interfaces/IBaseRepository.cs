using System.Linq.Expressions;


namespace Projeto_Trainee_Hub.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<List<T>> GetAllAsync();
        Task DeleteAsync(int id);   
    }
    
}
