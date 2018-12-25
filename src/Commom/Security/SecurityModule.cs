using Autofac;

namespace Common.Security
{
    public class SecurityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>();
            builder.RegisterType<ServiceAuthenticatorClient>().As<IServiceAuthenticatorClient>();
            builder.RegisterType<ServiceAuthenticatorHost>().As<IServiceAuthenticatorHost>();
        }
    }
}