using Autofac;

namespace Common.Services
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Encrypter>().As<IEncrypter>();
        }
    }
}