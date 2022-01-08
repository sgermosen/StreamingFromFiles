using AutoMapper;
using Domain;
using EngineAPI.Models;
using EngineAPI.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EngineAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : AppBaseController
    {
        private readonly IWebHostEnvironment _env;
        public ImageController(IMapper mapper, IStorageManager storageSaver, ApplicationDataContext context, IWebHostEnvironment env) : base(mapper, storageSaver, context)
        {
            _env = env;
        }

        [Route("upload")]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] FileModel model)
        {
            if (model.ImageFile != null)
            {
                await StorageManager.UploadBlobAsync(model.ImageFile, "images");
            }
            return Ok();
        }

        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> Get(string fileName)
        {
            string encodingType = "";
            var fileBytes = await StorageManager.GetFileAsync(fileName, "images");
            if (fileBytes.Extension == ".mp4")
                encodingType = "video/mp4";
            else if (fileBytes.Extension == ".jpg" || fileBytes.Extension == ".png" || fileBytes.Extension == ".webp")
                encodingType = "image/webp";

            return File(fileBytes.File, encodingType);


        }

        [Route("download")]
        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var imagBytes = await StorageManager.GetFileAsync(fileName, "images");
            return new FileContentResult(imagBytes.File, "application/octet-stream")
            {
                FileDownloadName = Guid.NewGuid().ToString() + imagBytes.Extension,
            };
        }

        //public HttpResponseMessage GetVideoContent()
        //{
        //    var httpResponce =  Request.CreateResponse();
        //    httpResponce.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)WriteContentToStream);
        //    return httpResponce;
        //}

        public async void WriteContentToStream(Stream outputStream, HttpContent content, TransportContext transportContext)
        {
            //path of file which we have to read//  
            var filePath = Path.Combine(_env.ContentRootPath, "wwwroot/Files/Coffee45358.mp4");

              //here set the size of buffer, you can set any size  
            int bufferSize = 1000;
            byte[] buffer = new byte[bufferSize];
            //here we re using FileStream to read file from server//  
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read,  FileShare.Read))//))
            {
                int totalSize = (int)fileStream.Length;
                /*here we are saying read bytes from file as long as total size of file 

                is greater then 0*/
                while (totalSize > 0)
                {
                    int count = totalSize > bufferSize ? bufferSize : totalSize;
                    //here we are reading the buffer from orginal file  
                    int sizeOfReadedBuffer = fileStream.Read(buffer, 0, count);
                    //here we are writing the readed buffer to output//  
                    await outputStream.WriteAsync(buffer, 0, sizeOfReadedBuffer);
                    //and finally after writing to output stream decrementing it to total size of file.  
                    totalSize -= sizeOfReadedBuffer;
                }
            }
        }

    }
}
