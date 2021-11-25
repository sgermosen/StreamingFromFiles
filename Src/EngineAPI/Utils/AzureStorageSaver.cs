using Azure.Storage.Blobs;
using EngineAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
//using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.AspNetCore.Mvc;

namespace EngineAPI.Utils
{
    public class AzureStorageManager : IStorageManager
    {
        private readonly string _connectionString;
        //  private readonly CloudBlobClient _blobClient;
        private readonly BlobServiceClient _blobClient;

        public AzureStorageManager(IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage");
            // string keys = configuration["Blob:ConnectionString"];
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            //_blobClient = storageAccount.CreateCloudBlobClient();
            _blobClient = blobServiceClient;

        }

        public async Task<FileStorageResponse> GetFileAsync(string fileName, string containerName)
        {
            var blobContainer = _blobClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainer.GetBlobClient(fileName);
            var downloadContent = await blobClient.DownloadAsync();
            var extension = Path.GetExtension(fileName);

            using MemoryStream ms = new MemoryStream();
            await downloadContent.Value.Content.CopyToAsync(ms);
            return new FileStorageResponse { FileName = fileName, Extension = extension, File = ms.ToArray() };
        }

        public async Task<FileStorageResponse> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream stream = new MemoryStream(file);
            Guid name = Guid.NewGuid();
            //CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            //CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}");
            //await blockBlob.UploadFromStreamAsync(stream);

            var extension = Path.GetExtension(file.ToString());
            var fileName = $"{name}{extension}";
            var client = new BlobContainerClient(_connectionString, containerName);
            await client.CreateIfNotExistsAsync();
            var blobClient = client.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream);
            return new FileStorageResponse { Extension = extension, FileName = fileName, Name = name };
        }

        public async Task<FileStorageResponse> UploadBlobAsync(IFormFile file, string containerName)
        {
            Stream stream = file.OpenReadStream();
            Guid name = Guid.NewGuid();
            //CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            //CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}");
            //await blockBlob.UploadFromStreamAsync(stream);
            //return name;

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{name}{extension}";
            var client = new BlobContainerClient(_connectionString, containerName);
            await client.CreateIfNotExistsAsync();
            var blobClient = client.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream);
            return new FileStorageResponse { Extension = extension, FileName = fileName, Name = name };


        }

        public async Task<FileStorageResponse> UploadBlobAsync(string image, string containerName)
        {
            Stream stream = File.OpenRead(image);
            Guid name = Guid.NewGuid();
            //CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            //CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}");
            // await blockBlob.UploadFromStreamAsync(stream); 
            var extension = Path.GetExtension(image);
            var fileName = $"{name}{extension}";
            var client = new BlobContainerClient(_connectionString, containerName);
            await client.CreateIfNotExistsAsync();
            var blobClient = client.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream);
            return new FileStorageResponse { Extension = extension, FileName = fileName, Name = name };

        }

        public async Task DeleteBlobAsync(Guid id, string containerName)
        {
            //CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            //CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{id}");
            // await blockBlob.DeleteAsync();

            var blobContainer = _blobClient.GetBlobContainerClient(containerName);// ("upload-file");
            var blobClient = blobContainer.GetBlobClient($"{id}");//file.Name); 
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> SaveFile(string container, IFormFile file)
        {
            var client = new BlobContainerClient(_connectionString, container);
            await client.CreateIfNotExistsAsync();
            client.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var blob = client.GetBlobClient(fileName);
            await blob.UploadAsync(file.OpenReadStream());
            return blob.Uri.ToString();

        }

        public async Task DeleteFile(string route, string container)
        {
            if (string.IsNullOrEmpty(route))
                return;
            var client = new BlobContainerClient(_connectionString, container);
            await client.CreateIfNotExistsAsync();
            var file = Path.GetFileName(route);
            var blob = client.GetBlobClient(file);
            await blob.DeleteIfExistsAsync();

        }

        public async Task<string> EditFile(string container, IFormFile file, string route)
        {
            await DeleteFile(route, container);
            return await SaveFile(container, file);
        }


        //public async Task<FileContentResult> WriteContentToStream()
        //{
        //    var cloudBlob = await _blobClient.GetBlobAsync(PlatformServiceConstants._blobIntroductoryVideoContainerPath + PlatformServiceConstants.IntroductoryVideo1, introductoryvideocontainerName);

        //    MemoryStream fileStream = new MemoryStream();
        //    await cloudBlob.DownloadToStreamAsync(fileStream);
        //    return new FileContentResult(fileStream.ToArray(), "application/octet-stream");

        //}
    }
}
