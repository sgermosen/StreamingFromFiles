using System;
using Transversal.Utils;

namespace Domain.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public Guid ImageId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string ImageUrl { get; set; }
        public int TypeId { get; set; }
        public ImageType Type { get; set; }

        public string Container => TypeId switch
        {
            1 => "personalidentification",
            2 => "immunizationcards",
            _ => "profilepicture"
        };
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"{StaticValues.AppUrl}/images/unnamed.png"
            : $"{StaticValues.AzureBlobUrl}/{Container}/{ImageId}";
    }
}
