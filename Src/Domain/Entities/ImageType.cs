using System.Collections.Generic;

namespace Domain.Entities
{
    public class ImageType
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public ICollection<Image> Images { get; set; }
    }
}
