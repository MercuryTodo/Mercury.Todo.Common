using System.Threading;
using System.Threading.Tasks;

namespace Common.Storage
{
    public interface IIdentifiableRepository<T> : IRepository<T> where T : IdentifiableEntity
    {
        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken);

        Task<T> GetByIdOrFailAsync(string id, CancellationToken cancellationToken);

        Task DeleteAsync(string id, CancellationToken cancellationToken);
    }
}