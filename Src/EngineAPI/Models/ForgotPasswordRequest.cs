using System.ComponentModel.DataAnnotations;

namespace EngineAPI.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
