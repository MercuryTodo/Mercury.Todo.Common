using Common.Configuration;
using Common.Consumers;
using Common.Security;

namespace Common.Hosting
{
    public interface IServiceConfiguration : IStorageConfiguration, ISecurityConfiguration, IConsumersConfiguration
    {
        LoggingConfiguration Logging { get; }
        SerilogSettings SerilogSettings { get; }
        SmtpConfiguration Smtp { get; }
    }
}