using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection;

namespace Common.Hosting
{
    public abstract class BaseConfiguration
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _MSConfiguration;
        private readonly object _conventionsConfiguration;

        public BaseConfiguration(
            Microsoft.Extensions.Configuration.IConfiguration MSConfiguration,
            object conventionsConfiguration)
        {
            _MSConfiguration = MSConfiguration;
            _conventionsConfiguration = conventionsConfiguration;
            _MSConfiguration.Bind(this);
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            var configurationProperties = GetType().GetProperties().Where(p => p.CanWrite);

            foreach (var configurationProperty in configurationProperties)
            {
                object propertyValue = GetMSDefaultValue(configurationProperty.Name, configurationProperty.PropertyType)
                    ?? GetConventionsDefaultValue(configurationProperty.Name, configurationProperty.PropertyType);

                configurationProperty.SetValue(this, propertyValue, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            }
        }

        public virtual object GetMSDefaultValue(string propertyName, System.Type propertyType)
        {
            return GetType()
                .GetProperty(propertyName)
                ?.GetValue(this);
        }

        public virtual object GetConventionsDefaultValue(string propertyName, System.Type propertyType)
        {
            if (_conventionsConfiguration != null)
            {
                return _conventionsConfiguration
                    .GetType()
                    .GetProperty(propertyName)
                    ?.GetValue(_conventionsConfiguration);
            }
            return null;
        }
    }
}