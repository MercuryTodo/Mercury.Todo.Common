using Dapper;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Storage
{
    public abstract class RepositoryBase<T> : IRepository<T>
    {
        private readonly IFactory<IDbConnection> _dbConnectionFactory;
        private readonly string _schemaName;
        private readonly string _tableName;
        private readonly SemaphoreSlim _schemaSynchronizedSemaphore;

        public bool SchemaSynchronized { get; private set; }

        protected RepositoryBase(
            IFactory<IDbConnection> dbConnectionFactory,
            string tableName,
            string schemaName = "Default",
            bool schemaSynchronized = false)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _schemaName = schemaName;
            _tableName = tableName;
            _schemaSynchronizedSemaphore = new SemaphoreSlim(1);
            SchemaSynchronized = schemaSynchronized;
        }

        private async Task SynchronizeDatabaseAsync(CancellationToken cancellationToken)
        {
            if (!SchemaSynchronized)
            {
                await _schemaSynchronizedSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    if (!SchemaSynchronized)
                    {
                        using (var connection = _dbConnectionFactory.Create())
                        {
                            var schemaExists = await connection.QueryFirstOrDefaultAsync<bool>(
                                "SELECT EXISTS (SELECT 1 FROM information_schema.schemata " +
                                " WHERE schema_name = @SchemaName);",
                                new { SchemaName = _schemaName })
                                .ConfigureAwait(false);

                            if (!schemaExists)
                            {
                                await connection.ExecuteAsync(
                                    "CREATE SCHEMA @SchemaName;",
                                    new { SchemaName = _schemaName })
                                    .ConfigureAwait(false);
                            }

                            var tableExists = await connection.QueryFirstOrDefaultAsync<bool>(
                                "SELECT EXISTS (SELECT 1 FROM information_schema.tables " +
                                " WHERE table_schema = @SchemaName AND table_name = @TableName);",
                                new { SchemaName = _schemaName, TableName = GetTableName() })
                                .ConfigureAwait(false);

                            if (!tableExists)
                            {
                                await connection.ExecuteAsync(
                                    CreateBaseQuery(),
                                    new { SchemaName = _schemaName, TableName = GetTableName() })
                                    .ConfigureAwait(false);
                            }
                            SchemaSynchronized = true;
                        }
                    }
                }
                finally
                {
                    _schemaSynchronizedSemaphore.Release();
                }
            }
        }

        public string GetTableName()
        {
            return _tableName.ToLower();
        }

        public string GetSchemaName()
        {
            return _schemaName;
        }

        public string GetTableNameWithSchema()
        {
            return "\"" + _schemaName + "\"." + GetTableName();
        }

        public async Task<IDbConnection> GetConnectionAsync(CancellationToken cancellationToken)
        {
            await SynchronizeDatabaseAsync(cancellationToken).ConfigureAwait(false);
            return _dbConnectionFactory.Create();
        }

        public virtual async Task AddAsync(T item, CancellationToken cancellationToken)
        {
            using (var connection = await GetConnectionAsync(cancellationToken))
            {
                var props = item.GetType().GetProperties();
                var columns = props.Select(p => p.Name).ToArray();

                var insertQuery = $"INSERT INTO {GetTableNameWithSchema()} ({string.Join(",", columns)}) " +
                    $"VALUES (@{string.Join(",@", columns)})";

                await connection.ExecuteAsync(insertQuery, item);
            }
        }

        public string UpdateQueryWithoutWhere(T item)
        {
            var props = item.GetType().GetProperties();
            var columns = props.Select(p => p.Name).ToArray();

            var parameters = columns.Select(name => name + "=@" + name).ToList();

            return $"UPDATE {GetTableNameWithSchema()} SET {string.Join(",", parameters)} ";
        }

        private string CreateBaseQuery()
        {
            return string.Format(
                CreateQuery(),
                '"' + GetSchemaName() + '"',
                '"' + GetTableName() + '"');
        }

        public abstract string CreateQuery();

        public abstract Task UpdateAsync(T item, CancellationToken cancellationToken);
    }
}