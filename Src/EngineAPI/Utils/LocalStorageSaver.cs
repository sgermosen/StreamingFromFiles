using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EngineAPI.Utils
{
    public class LocalStorageManager: IStorageManager
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LocalStorageManager(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveFile(string container, IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, container);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string route = Path.Combine(folder, fileName);

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                await File.WriteAllBytesAsync(route, content);
            }
            var actualUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var routeForDb = Path.Combine(actualUrl, container, fileName).Replace("\\", "/");
            return routeForDb;
        }

        public Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> UploadBlobAsync(string image, string containerName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBlobAsync(Guid id, string containerName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFile(string route, string container)
        {
            if (string.IsNullOrEmpty(route))
                return Task.CompletedTask;

            var filename = Path.Combine(route);
            var fileDirectory = Path.Combine(env.WebRootPath, container, filename);

            if (File.Exists(fileDirectory))
                File.Delete(fileDirectory);

            return Task.CompletedTask;
        }

        public async Task<string> EditFile(string container, IFormFile file, string route)
        {
            await DeleteFile(route, container);
            return await SaveFile(container, file);
        }

        public Task<byte[]> GetFileAsync(string fileName, string containerName)
        {
            throw new NotImplementedException();
        }
    }
}
