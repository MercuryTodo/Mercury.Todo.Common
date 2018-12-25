using Autofac;
using MassTransit;

namespace Common.Consumers
{
    public class ConsumerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ExceptionHandler>().As<IExceptionHandler>();
            builder.Register(context =>
            {
                var consumersConfiguration = context.Resolve<IConsumersConfiguration>();

                if (consumersConfiguration.IsMemoryInstance == true)
                {
                    var bus = Bus.Factory.CreateUsingInMemory(cfg =>
                    {
                        cfg.UseJsonSerializer();
                        cfg.ReceiveEndpoint(consumersConfiguration.ServiceQueue, ec =>
                        {
                            ec.LoadFrom(context);
                        });
                    });
                    bus.Start();
                    return bus;
                }
                throw new System.Exception("Bus factory not defined");
            }).SingleInstance().As<IBusControl>().As<IBus>();
        }
    }
}