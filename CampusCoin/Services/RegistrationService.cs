using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.Mail;
using CampusCoin.Models;
using CampusCoin.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CampusCoin.Services;

public class RegistrationService
{
    private IDbContextFactory<CampusCoinContext> _context;
    public string DbPath { get; }
    public ObservableCollection<Users> UsersCollection { get; } = new();


    public RegistrationService(IDbContextFactory<CampusCoinContext> context)
    {
        _context = context;
    }

    public async Task<List<Users>> GetUsers()
    {

        var dbContext = await _context.CreateDbContextAsync();
        usersList = await dbContext.Users.ToListAsync();
        return usersList;
    }

    public async Task RegisterUser(Users user)
    {
        var dbContext = await _context.CreateDbContextAsync();
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
    }

    public List<String> ValidateUserInput(Users potentialUser)
    {
        List<string> errorList = new();
        string[] requiredFields = { potentialUser.Email, potentialUser.Password, potentialUser.PhoneNumber, potentialUser.FirstName, potentialUser.LastName };

        // Check all fields to see if any fields are empty
        if (requiredFields.Any(string.IsNullOrEmpty))
        {
            errorList.Add("All fields are required");
        }

        // Check if valid email address
        try
        {
            var emailAddress = new MailAddress(potentialUser.Email);
        }
        catch
        {
           errorList.Add("Invalid Email Format");
        }

        // Check if email is already in use
        if (UserExistsWithEmail(potentialUser))
        {
            errorList.Add("Email already in use");
        }

        // Check if phone number is in a valid format
        if (!UserPhoneNumberFormatIsValid(potentialUser))
        {
            errorList.Add("Phone number must be in a valid format (Digits Only, 10 minimum)");
        }

        // Check if phone number is already in use
        if (UserExistsWithPhoneNumber(potentialUser))
        {
            errorList.Add("Phone number already in use");
        }
        

        return errorList;
    }

    public Boolean UserPhoneNumberFormatIsValid(Users potentialUser)
    {
        try
        {
            foreach (char c in potentialUser.PhoneNumber)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
                else if (potentialUser.PhoneNumber.Length < 10)
                {
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        return true;
    }

    public Boolean UserExistsWithPhoneNumber(Users potentialUser)
    {
        try
        {
            foreach (var user in usersList)
            {
                if (user.PhoneNumber == potentialUser.PhoneNumber)
                {
                    return true;
                }
            }
            if (usersList == null || usersList.Count == 0)
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        return false;
    }

    public Boolean UserExistsWithEmail(Users potentialUser)
    {
        try
        {
            foreach (var user in usersList)
            {
                if (user.Email == potentialUser.Email)
                {
                    return true;
                }
            }
            if (usersList == null || usersList.Count == 0)
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        return false;
    }
}
