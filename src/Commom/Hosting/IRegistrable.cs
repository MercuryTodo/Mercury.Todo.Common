using Autofac;

namespace Common.Hosting
{
    public interface IRegistrable
    {
        void RegisterTo(ContainerBuilder container);
    }
}