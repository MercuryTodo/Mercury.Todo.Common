using Common.Services;
using Dapper;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Storage
{
    public abstract class IdentifiableRepositoryBase<T> : RepositoryBase<T>, IIdentifiableRepository<T>
        where T : IdentifiableEntity
    {
        protected IdentifiableRepositoryBase(IFactory<IDbConnection> dbConnectionFactory,
            string tableName,
            string schemaName = "Default",
            bool schemaSynchronized = false)
            : base(dbConnectionFactory, tableName, schemaName, schemaSynchronized)
        {
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            using (var connection = await GetConnectionAsync(cancellationToken))
            {
                await connection.ExecuteAsync(
                    $"Delete from {GetTableNameWithSchema()} where Id = @Id", new { Id = id });
            }
        }

        public async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            using (var connection = await GetConnectionAsync(cancellationToken))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(
                    $"Select * from {GetTableNameWithSchema()} where Id = @Id", new { Id = id });
            }
        }

        public async Task<T> GetByIdOrFailAsync(string id, CancellationToken cancellationToken)
        {
            var response = await GetByIdAsync(id, cancellationToken);
            if (response == null)
            {
                throw new ServiceException("This Entity doesnt exist");
            }
            return response;
        }

        public override async Task UpdateAsync(T item, CancellationToken cancellationToken)
        {
            using (var connection = await GetConnectionAsync(cancellationToken))
            {
                await connection.ExecuteAsync(
                    $"{UpdateQueryWithoutWhere(item)} WHERE Id = @Id", item);
            }
        }
    }
}