using Autofac;
using Common.Consumers;
using Common.Files;
using Common.Security;
using Common.Services;
using Common.Storage;

namespace Common.Extensions
{
    public static class ContainerExtensions
    {
        public static ContainerBuilder RegisterCommon(this ContainerBuilder builder)
        {
            builder.RegisterModule<ConsumerModule>();
            builder.RegisterModule<SecurityModule>();
            builder.RegisterModule<ServicesModule>();
            builder.RegisterModule<StorageModule>();
            builder.RegisterModule<FilesModule>();
            return builder;
        }
    }
}