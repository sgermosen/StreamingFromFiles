using System.Collections.Generic;

namespace EngineAPI.DTOs
{
    public class UserBulkCreationDTO
    {
        public string BulkPassword { get; set; }
        public List<UserCreationDTO> Users { get; set; }
    }
}
