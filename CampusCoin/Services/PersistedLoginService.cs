using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using CampusCoin.Models;
using Microsoft.EntityFrameworkCore;

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

    /// <summary> Logs out the current logged in user </summary>
    /// <param></param>
    /// <returns></returns>
    public void logout()
    {
        userIsLoggedIn = false;
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

    public async Task<bool> logoutConfirmation()
    {
        // Implement your logic to display a confirmation action sheet
        return await Shell.Current.DisplayAlert("Sign Out", "Would you like to sign out?", "Yes", "No");
    }

}