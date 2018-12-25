using Common.Hosting;
using Npgsql;
using System.Data;

namespace Common.Storage
{
    public class ConnectionFactory : IFactory<IDbConnection>
    {
        private readonly IStorageConfiguration _hostConfiguration;

        public ConnectionFactory(IStorageConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
        }

        public IDbConnection Create()
        {
            var connection = new NpgsqlConnection(_hostConfiguration.DatabaseConfiguration.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}