using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EngineAPI.Models
{
    public class RegisterViewModel
    {

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string ConfirmPassword { get; set; }
         
        [StringLength(15)]
        public string Identification { get; set; }

        public string EmployeeNumber { get; set; }
        public string StoreCode { get; set; }

        [Required]
        [StringLength(300)]
        public string FullName { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
