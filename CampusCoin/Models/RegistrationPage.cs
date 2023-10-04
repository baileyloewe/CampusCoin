using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ShellMixedSample.Models
{
    // This class handles the registration page data (which is the purpose of model classes) It performs input validation for the registration page VM and View & creates users once all validate properly
    internal class RegistrationPage
    {
        public CampusCoinContext _dbContext;

        public RegistrationPage(CampusCoinContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public void RegistrationSubmitted(Users CurrentUser)
        {
            ValidateUserInfo(CurrentUser);
            SaveUser();
        }

        public void SaveUser()
        {
            //_dbContext.Users.Add(newUser);
        }

        public void ValidateUserInfo(Users CurrentUser)
        {
            ValidateEmail(CurrentUser.Email);
            ValidatePassword(CurrentUser.Password);
            ValidatePassword(CurrentUser.PhoneNumber);
            ValidatePassword(CurrentUser.FirstName);
            ValidatePassword(CurrentUser.LastName);
        }

        public bool ValidateEmail(string Email)
        {
            try
            {
                Email = new MailAddress(Email).Address;
            }
            catch (FormatException)
            {
                return false;
            }
            return true;
        }

        public bool ValidatePassword(string Password)
        {
            if (Password == null || Password.Length < 10)
            {
                return false;
            }
            else return true;
        }

        public bool ValidatePhoneNumber(string PhoneNumber)
        {
            if (Regex.Replace(PhoneNumber, "[^0-9]", "").Length != 10)
            {
                return false;
            }
            else if (PhoneNumber == null || PhoneNumber.Length < 2)
            {
                return false;
            }
            else return true;
        }

        public bool ValidateFirstName(string FirstName)
        {
            if (FirstName == null || FirstName.Length < 2)
            {
                return false;
            }
            else return true;
        }

        public bool ValidateLastName(string LastName)
        {
            if (LastName == null || LastName.Length < 2)
            {
                return false;
            }
            else return true;
        }

    }
}
