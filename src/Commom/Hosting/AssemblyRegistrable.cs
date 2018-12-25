using Autofac;
using Nancy.Bootstrappers.Autofac;
using System;
using System.Linq;
using System.Reflection;

namespace Common.Hosting
{
    /// <summary>
    /// Register the types of all registrables defined in an assembly.
    /// </summary>
    public class AssemblyRegistrable : AutofacNancyBootstrapper
    {
        private readonly Assembly _assembly;

        public AssemblyRegistrable(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            base.ConfigureApplicationContainer(existingContainer);
            if (existingContainer == null) throw new ArgumentNullException(nameof(existingContainer));

            var registrables = _assembly.GetTypes()
                .Where(t =>
                    t != GetType()
                    && !t.IsAbstract
                    && typeof(IRegistrable).IsAssignableFrom(t))
                .Select(t => (IRegistrable)Activator.CreateInstance(t));
            existingContainer.Update(b =>
            {
                foreach (var registrable in registrables)
                {
                    registrable.RegisterTo(b);
                }
            });
        }
    }
}