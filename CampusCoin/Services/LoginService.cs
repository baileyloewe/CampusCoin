using System.Diagnostics;
using CampusCoin.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCoin.Services;

public class LoginService
{
    private IDbContextFactory<CampusCoinContext> _context;
    public LoginService(IDbContextFactory<CampusCoinContext> context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets a user from the user database by email
    /// </summary>
    /// <param name="email">The email address to search for in the database</param>
    /// <returns name = "user"> The user who matches the email, or null if none match</returns>
    public async Task<User> GetUserByEmail(string email)
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