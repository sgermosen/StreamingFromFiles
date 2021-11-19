using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EngineAPI.Utils
{
    public interface IStorageSaver
    {
        Task DeleteFile(string route, string container);
        Task<string> EditFile(string container, IFormFile file, string route);
        Task<string> SaveFile(string container, IFormFile file);
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);

        Task<Guid> UploadBlobAsync(byte[] file, string containerName);

        Task<Guid> UploadBlobAsync(string image, string containerName);

        Task DeleteBlobAsync(Guid id, string containerName);
    }
}
