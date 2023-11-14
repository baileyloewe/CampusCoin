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

    public PersistedLoginService(IDbContextFactory<CampusCoinContext> context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the current logged in user
    /// </summary>
    /// <param></param>
    /// <returns>Returns the current user who is logged in</returns>
    public User GetUser()
    {
        return this.currentUser;
    }

    /// <summary>
    /// Sets the current logged in user from database using email
    /// </summary>
    /// <param name="email">The email of user to be logged in</param>
    /// <returns></returns>
    public async Task SetUser(String email)
    {

        this.currentUser = await GetUserByEmail(email);
    }

    public void ClearUser()
    {
        this.currentUser = null;
    }

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

}