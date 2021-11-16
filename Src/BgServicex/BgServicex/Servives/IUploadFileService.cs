using BgServicex.Models;
using System.Threading.Tasks;

namespace BgServicex
{
    public interface IUploadFileService
    {
        //void Run(IFormFile file, string containerName);
        //void Run(byte[] file, string containerName); 
        Task<FileUploadedViewModel> Upload(string filename, string basePath);
    }
}