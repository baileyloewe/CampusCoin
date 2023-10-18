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
    /// Validates that the phone number is in a valid format, uses a regex pattern to validate
    /// </summary>
    public class PhoneNumberValidationAttribute : ValidationAttribute
    {
        private readonly string _phoneNumberRegexPattern = @"^\d{10,}$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Phone number is required");
            }

            string phonenumber = value.ToString();

            if (!Regex.IsMatch(phonenumber, _phoneNumberRegexPattern))
            {
                return new ValidationResult("Invalid phone number format, phone number must be 10 or more characters and only contain digits");
            }

            return ValidationResult.Success;
        }
    }
}
