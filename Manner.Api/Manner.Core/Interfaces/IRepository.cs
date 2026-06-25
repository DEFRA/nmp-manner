namespace Manner.Core.Interfaces;
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>?> FetchAllAsync();
    Task<T?> FetchByIdAsync(int id);
}
