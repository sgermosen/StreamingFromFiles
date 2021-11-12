using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BgServicex.Data
{
    public class AuditEntity //: BaseEntity
    {
        public bool Deleted { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public IdentityUser CreatedUser { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        [ForeignKey("UpdatedBy")]
        public IdentityUser UpdatedUser { get; set; }

        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        [ForeignKey("DeletedBy")]
        public IdentityUser DeletedUser { get; set; }
    }
}
