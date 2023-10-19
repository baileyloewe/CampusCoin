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
    /// Validates that the password is in a valid format, uses a regex pattern to validate
    /// </summary>
    public class PasswordValidationAttribute : ValidationAttribute
    {
        private readonly string _passwordRegexPattern = @"^.{10,}$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Password is required");
            }

            string password = value.ToString();

            if (!Regex.IsMatch(password, _passwordRegexPattern))
            {
                return new ValidationResult("Password must be at least 10 characters");
            }

            return ValidationResult.Success;
        }
    }
}
