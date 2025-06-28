using System.ComponentModel.DataAnnotations;

namespace Small_Library.ViewModels
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Return date is required.");
            }

            if (DateTime.TryParse(value.ToString(), out DateTime returnDate))
            {
                if (returnDate.Date <= DateTime.Today)
                {
                    return new ValidationResult("Return date must be in the future (after today).");
                }
            }
            else
            {
                return new ValidationResult("Invalid date format.");
            }

            return ValidationResult.Success;
        }
    }
} 