using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using CampusCoin.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace CampusCoin.Services;

public class PersistedLoginService
{
    private readonly IDbContextFactory<CampusCoinContext> _context;
    public int verificationCode { get; private set; }
    public User currentUser { get; private set; }
    public bool userIsLoggedIn { get; private set; }

    public PersistedLoginService(IDbContextFactory<CampusCoinContext> context)
    {
        _context = context;
    }

    /// <summary> Gets the current logged in user </summary>
    /// <param></param>
    /// <returns>Returns the current user who is logged in</returns>
    public User getLoggedInUser()
    {
        return this.currentUser;
    }

    /// <summary> Logs in user by email </summary>
    /// <param name="email">The email of user to be logged in</param>
    /// <returns></returns>
    public async Task login(String email)
    {
        userIsLoggedIn = true;
        this.currentUser = await GetUserByEmail(email);
    }

    /// <summary> Logs in user  </summary>
    /// <param name="user">The user to be logged in</param>
    /// <returns></returns>
    public void login(User user)
    {
        userIsLoggedIn = true;
        this.currentUser = user;
    }

    /// <summary> Logs out the current logged in user </summary>
    /// <param></param>
    /// <returns></returns>
    public void logout()
    {
        userIsLoggedIn = false;
        if (Preferences.Default.ContainsKey("AuthToken"))
            Preferences.Default.Remove("AuthToken");
        this.currentUser = null;
    }

    /// <summary> Returns true if a user is logged in </summary>
    /// <param></param>
    /// <returns></returns>
    public bool loggedIn()
    {
        return userIsLoggedIn;
    }

    /// <summary> Gets a user by email from the database </summary>
    /// <param name="email"> email of the user</param>
    /// <returns> user from the database </returns>
    async Task<User> GetUserByEmail(string email)
    {
        var dbContext = await _context.CreateDbContextAsync();
        User user = null;
        try
        {
            using (dbContext)
            {
                user = dbContext.Users.FirstOrDefault(u => u.Email == email);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        return user;
    }

    /// <summary> Prompts the user to sign out </summary>
    /// <param></param>
    /// <returns> Returns the user's choice as true/false</returns>
    public async Task<bool> logoutPrompt()
    {
        if (userIsLoggedIn)
        {
            bool userWantsToLogout = await logoutConfirmation();
            if (userWantsToLogout)
            {
                logout();
                return true;
            }
            else return false;
        }
        return true;
    }

    /// <summary> Creates the prompt for the user to sign out </summary>
    /// <param></param>
    /// <returns></returns>
    public async Task<bool> logoutConfirmation()
    {
        return await Shell.Current.DisplayAlert("Sign Out", "Would you like to sign out?", "Yes", "No");
    }

    /// <summary> Generates an authentication token for "remember me" </summary>
    /// <param></param>
    /// <returns>Token converted to base64 string</returns>
    public static String generateAuthToken()
    {
        byte[] token = new byte[32];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(token);
        }
        return Convert.ToBase64String(token);
    }

    /// <summary> Saves user's auth token</summary>
    /// <param></param>
    /// <returns></returns>
    public async Task SaveAuthToken()
    {
        var dbContext = await _context.CreateDbContextAsync();
        try
        {
            User user = this.getLoggedInUser();
            string tokenFromDB = dbContext.Users
                .Where(u => u.UserId == user.UserId)
                .Select(u => u.AuthToken)
                .FirstOrDefault();
            Preferences.Default.Set("AuthToken", tokenFromDB);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving token: {ex.Message}");
        }
    }

    /// <summary> Clears user's auth token</summary>
    /// <param></param>
    /// <returns></returns>
    public void ClearAuthToken()
    {
        try
        {
            Preferences.Default.Remove("AuthToken");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing token: {ex.Message}");
        }
    }

    /// <summary> Checks if the token stored matches the database's token </summary>
    /// <param></param>
    /// <returns></returns>
    public async Task<bool> IsTokenValid()
    {
        string storedToken = Preferences.Default.Get("AuthToken", "Unknown");
        string authToken = "";
        User user = this.getLoggedInUser();
        var dbContext = await _context.CreateDbContextAsync();
        try
        {
           authToken =  dbContext.Users
                    .Where(u => u.UserId == user.UserId)
                    .Select(u => u.AuthToken)
                    .FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking token: {ex.Message}");
        }

        return string.Equals(storedToken, authToken);
    }
}