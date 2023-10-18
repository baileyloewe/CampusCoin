using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CampusCoin.Validation
{

    /// <summary>
    /// Validates that the email address is in a valid format, uses a regex pattern to validate
    /// </summary>
    public class EmailValidationAttribute : ValidationAttribute
    {
        private readonly string _emailRegexPattern = @"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,6}$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Email address is required");
            }

            string email = value.ToString();

            if (!Regex.IsMatch(email, _emailRegexPattern))
            {
                return new ValidationResult("Invalid email address format");
            }

            return ValidationResult.Success;
        }
    }
}
