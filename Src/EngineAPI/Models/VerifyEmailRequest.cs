using System.ComponentModel.DataAnnotations;

namespace EngineAPI.Models
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
