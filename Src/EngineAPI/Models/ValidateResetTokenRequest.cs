using System.ComponentModel.DataAnnotations;

namespace EngineAPI.Models
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
