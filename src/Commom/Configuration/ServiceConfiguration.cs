using Common.Configuration;
using Common.Security;
using Common.Storage;

namespace Common.Hosting
{
    public class ServiceConfiguration : BaseConfiguration, IServiceConfiguration
    {
        public ServiceConfiguration(
            Microsoft.Extensions.Configuration.IConfiguration MSConfiguration,
            object conventionsConfiguration)
            : base(MSConfiguration, conventionsConfiguration)
        {
        }

        public ServiceConfiguration(
            Microsoft.Extensions.Configuration.IConfiguration MSConfiguration)
            : base(MSConfiguration, new ConventionsServiceConfiguration())
        {
        }

        public SmtpConfiguration Smtp { get; set; }

        public LoggingConfiguration Logging { get; set; }

        public SerilogSettings SerilogSettings { get; set; }

        public DatabaseConfiguration DatabaseConfiguration { get; set; }

        public JwtTokenSettings JwtTokenSettings { get; set; }

        public ServicesSettings ServicesSettings { get; set; }

        public ServiceSettings ServiceSettings { get; set; }

        public bool? IsMemoryInstance { get; set; }

        public string ServiceQueue { get; set; }
    }
}