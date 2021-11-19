using System;
using System.ComponentModel.DataAnnotations;

namespace EngineAPI.DTOs
{
    public class UserCreationDTO : UserCredentials
    {
        [Required]
        [StringLength(300)]
        public string FullName { get; set; }

        [StringLength(15)]
        public string Identification { get; set; }

        [StringLength(15)]
        public string EmployeeNumber { get; set; }

        [StringLength(15)]
        public string StoreCode { get; set; }
    }
}
