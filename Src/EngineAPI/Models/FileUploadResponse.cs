using System;

namespace EngineAPI.Models
{
    public class FileStorageResponse
    {
        public string Extension { get; set; }
        public string FileName { get; set; }
        public Guid Name { get; set; } 
        public byte[] File { get; set; }

    }
}
