using System.Threading;
using System.Threading.Tasks;

namespace Common.Storage
{
    public interface IRepository<T>
    {
        Task UpdateAsync(T item, CancellationToken cancellationToken);

        Task AddAsync(T item, CancellationToken cancellationToken);
    }
}