using System;
using System.Threading.Tasks;

namespace Common.Files
{
    public interface IFileHandler
    {
        Task UploadAsync(File file, string newName, Action<string, string> onUploaded = null);

        Task DeleteAsync(string name);
    }
}