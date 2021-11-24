using EngineAPI.Resources;
using System.ComponentModel.DataAnnotations;

namespace EngineAPI.Validations
{
    public class FirstLetterUppercaseAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;

            var firstLetter = value.ToString()[0].ToString();
            if (firstLetter != firstLetter.ToUpper())
                return new ValidationResult(Resource.FirstLetterUppercaseError);
            return ValidationResult.Success;

            // return base.IsValid(value, validationContext);
        }
    }
}
