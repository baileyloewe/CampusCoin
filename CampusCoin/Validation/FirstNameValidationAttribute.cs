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
    /// Validates that the first name is in a valid format, uses a regex pattern to validate
    /// </summary>
    public class FirstnameValidationAttribute : ValidationAttribute
    {
        private readonly string _firstnameRegexPattern = @"^.+$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("First name is required");
            }

            string firstname = value.ToString();

            if (!Regex.IsMatch(firstname, _firstnameRegexPattern))
            {
                return new ValidationResult("First name must contain at least one character");
            }

            return ValidationResult.Success;
        }
    }
}
