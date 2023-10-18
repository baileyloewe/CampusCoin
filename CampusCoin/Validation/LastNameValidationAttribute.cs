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
    /// Validates that the last name is in a valid format, uses a regex pattern to validate
    /// </summary>
    public class LastnameValidationAttribute : ValidationAttribute
    {
        private readonly string _lastnameRegexPattern = @"^.+$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Last name is required");
            }

            string lastname = value.ToString();

            if (!Regex.IsMatch(lastname, _lastnameRegexPattern))
            {
                return new ValidationResult("Last name must contain at least one character");
            }

            return ValidationResult.Success;
        }
    }
}
