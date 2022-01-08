using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BgServicex.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BgServicex.Servives
{
    public class UploadFileService : IUploadFileService
    {
        private readonly ILogger<UploadFileService> _logger;
        //private readonly CloudBlobClient _blobClient;
        private readonly string _container;
        private readonly string _connectionString;
        // private readonly BlobServiceClient _blobClient;


        public UploadFileService(ILogger<UploadFileService> logger, IConfiguration configuration)//, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            //  string keys = configuration["Blob:ConnectionString"];
            //  CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            //  _blobClient = storageAccount.CreateCloudBlobClient();
            _container = configuration["Blob:ContainerName"];
            _connectionString = configuration["Blob:ConnectionString"];// configuration.GetConnectionString("Blob:ConnectionString");
            //  _blobClient = blobServiceClient; 
        }

        public async Task<FileUploadedViewModel> Upload(string filename, string fullpath)
        {

            _logger.LogInformation("In Service Upload Start");

            var fileBytes = File.ReadAllBytes(fullpath);

            //var containerObject = _blobClient.GetContainerReference(_container); 
            //if (containerObject.CreateIfNotExistsAsync().Result)
            //    await containerObject.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            //var fileobject = containerObject.GetBlockBlobReference($"{newFileame}{ext}");
            //fileobject.Properties.ContentType = file_type;
            //await fileobject.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);

            MemoryStream stream = new MemoryStream(fileBytes);
            Guid name = Guid.NewGuid();

            var extension = Path.GetExtension(filename);
            var fileName = $"{name}{extension}";
            var client = new BlobContainerClient(_connectionString, _container);

            await client.CreateIfNotExistsAsync();//PublicAccessType.Blob);
            var blobClient = client.GetBlobClient(fileName);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filename, out string file_type))
                file_type = "application/octet-stream";

            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = file_type
            };
            await blobClient.UploadAsync(stream, httpHeaders);

            _logger.LogInformation($"In Service Upload Done: {client.Uri.AbsoluteUri}/{fileName}");
            return new FileUploadedViewModel
            {
                Name = name.ToString(),
                Extension = extension,
                AzureUrl = $"{client.Uri.AbsoluteUri}/{fileName}", //fileobject.Uri.AbsoluteUri,
                Size = fileBytes.Length,
                FileName = fileName// fileobject.Name
            };

        }




    }
}
