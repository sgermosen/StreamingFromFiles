using System;

namespace BgServicex.Models
{
    public class FileUploadedViewModel
    {
        public string AzureUrl { get; set; }
        public string FileName { get; set; }
        public int Size { get; set; }
        public string Name { get;  set; }
        public string Extension { get;  set; }
    }
}
