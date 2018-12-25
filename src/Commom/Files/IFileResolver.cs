using System.IO;
using System.Threading.Tasks;

namespace Common.Files
{
    public interface IFileResolver
    {
        File FromBase64(string base64, string name, string contentType);

        Task<Stream> FromUrlAsync(string url);
    }
}