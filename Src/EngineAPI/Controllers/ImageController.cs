using AutoMapper;
using Domain;
using EngineAPI.Models;
using EngineAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EngineAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : AppBaseController
    {
        public ImageController(IMapper mapper, IStorageManager storageSaver, ApplicationDataContext context) : base(mapper, storageSaver, context)
        {
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
    }
}
