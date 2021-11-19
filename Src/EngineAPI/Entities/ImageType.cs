using System.Collections.Generic;

namespace EngineAPI.Entities
{
    public class ImageType
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public ICollection<Image> Images { get; set; }
    }
}
