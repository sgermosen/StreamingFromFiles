using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using EngineAPI.Models;

namespace EngineAPI.Utils
{
    public interface IStorageManager
    {
        Task<FileStorageResponse> GetFileAsync(string fileName, string containerName); 
        Task DeleteFile(string route, string container);
        Task<string> EditFile(string container, IFormFile file, string route);
        Task<string> SaveFile(string container, IFormFile file);
        Task<FileStorageResponse> UploadBlobAsync(IFormFile file, string containerName);

        Task<FileStorageResponse> UploadBlobAsync(byte[] file, string containerName);

        Task<FileStorageResponse> UploadBlobAsync(string image, string containerName);

        Task DeleteBlobAsync(Guid id, string containerName);
    }
}
