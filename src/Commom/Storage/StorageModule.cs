using Autofac;
using System.Data;

namespace Common.Storage
{
    public class StorageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConnectionFactory>()
                .As<IFactory<IDbConnection>>();
        }
    }
}