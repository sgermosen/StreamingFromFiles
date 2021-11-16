using BgServicex.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BgServicex
{
    public class UploadFileService : IUploadFileService
    {
        private readonly ILogger<UploadFileService> _logger;
        // private readonly IBlobHelper _blobHelper;
        private readonly CloudBlobClient _blobClient;
        private readonly string _container;

        public UploadFileService(ILogger<UploadFileService> logger, IConfiguration configuration)//, IBlobHelper blobHelper)
        {
            _logger = logger;
            // _blobHelper = blobHelper;
            string keys = configuration["Blob:ConnectionString"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            _blobClient = storageAccount.CreateCloudBlobClient();
            _container = configuration["Blob:ContainerName"];

        }

        public async Task<FileUploadedViewModel> Upload(string filename, string fullpath)
        {
            _logger.LogInformation("In Service Upload Start");

            var fileBytes = System.IO.File.ReadAllBytes(fullpath);// $"{basePath}\\{filename}");

            var containerObject = _blobClient.GetContainerReference(_container);

            if (containerObject.CreateIfNotExistsAsync().Result)
                await containerObject.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            
            var ext = Path.GetExtension(fullpath).ToString();
            Guid newFileame = Guid.NewGuid()   ;

            var fileobject = containerObject.GetBlockBlobReference($"{newFileame}{ext}");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filename, out string file_type))
                file_type = "application/octet-stream";

            fileobject.Properties.ContentType = file_type;
            await fileobject.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);

            _logger.LogInformation($"In Service Upload Done: {fileobject.Uri.AbsoluteUri}");
            return new FileUploadedViewModel
            {
                AzureUrl = fileobject.Uri.AbsoluteUri,
                Size = fileBytes.Length,
                FileName = fileobject.Name
            };

        }

        //public async void Run(IFormFile file, string containerName)
        //{
        //    _logger.LogInformation("In Service Upload Start");
        //    await _blobHelper.UploadBlobAsync(file, containerName);
        //    _logger.LogInformation("In Service Upload Done");

        //}

        //public async void Run(byte[] file, string containerName)
        //{
        //    _logger.LogInformation("In Service Upload Start");
        //    await _blobHelper.UploadBlobAsync(file, containerName);
        //    _logger.LogInformation("In Service Upload Done");

        //}

    }
}
