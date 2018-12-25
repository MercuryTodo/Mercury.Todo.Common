using Common.Configuration;
using Common.Security;
using Common.Storage;

namespace Common.Hosting
{
    public class ConventionsServiceConfiguration : IServiceConfiguration
    {
        public DatabaseConfiguration DatabaseConfiguration => null;

        public JwtTokenSettings JwtTokenSettings => new JwtTokenSettings
        {
            UseRsa = true,
            RsaPublicKeyXML = "<RSAKeyValue><Modulus>xA7SEU+e0yQH5rm9kbCDN9o3aPIo7HbP7tX6WOocLZAtNfyxSZDU16ksL6WjubafOqNEpcwR3RdFsT7bCqnXPBe5ELh5u4VEy19MzxkXRgrMvavzyBpVRgBUwUlV5foK5hhmbktQhyNdy/6LpQRhDUDsTvK+g9Ucj47es9AQJ3U=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
        };

        public ServicesSettings ServicesSettings => null;

        public ServiceSettings ServiceSettings => null;

        public bool? IsMemoryInstance => true;

        public virtual string ServiceQueue => null;

        public SerilogSettings SerilogSettings => new SerilogSettings
        {
            Level = "Debug",
            IndexFormat = "",
            ConsoleEnabled = true,
            ElkEnabled = false,
            UseBasicAuth = false,
        };

        public LoggingConfiguration Logging => new LoggingConfiguration
        {
            IncludeScopes = false,
            LogLevel = new LogLevel
            {
                Default = "Verbose",
                System = "Information",
                Microsoft = "Information"
            }
        };

        public SmtpConfiguration Smtp => null;
    }
}