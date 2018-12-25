using Common.Storage;

namespace Common.Hosting
{
    /// <summary>
    /// Defines the client configuration properties.
    /// </summary>
    public interface IStorageConfiguration
    {
        DatabaseConfiguration DatabaseConfiguration { get; }
    }
}