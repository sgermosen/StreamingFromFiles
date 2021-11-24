using Microsoft.AspNetCore.Http;

namespace EngineAPI.Models
{
    public class FileModel
    {
        public IFormFile ImageFile { get; set; }
    }
}
