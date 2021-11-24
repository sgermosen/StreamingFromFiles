using AutoMapper;
using Domain;
using EngineAPI.Models;
using EngineAPI.Utils;
using Microsoft.AspNetCore.Mvc;
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
            var imgBytes = await StorageManager.GetFileAsync(fileName, "images");
            return File(imgBytes, "image/webp");
        }

        [Route("download")]
        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var imagBytes = await StorageManager.Get(fileName);
            return new FileContentResult(imagBytes, "application/octet-stream")
            {
                FileDownloadName = Guid.NewGuid().ToString() + ".webp",
            };
        }
    }
}
