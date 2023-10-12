using System.Net.Http.Json;
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

    
    public async Task<Users> GetUserByEmail(string email)
    {
        var dbContext = await _context.CreateDbContextAsync();
        Users user = null;
        try 
        {
            using (dbContext) 
            {
                user = dbContext.Users.FirstOrDefault(u => u.Email == email);
            }
        }
        catch (Exception ex)
        { 
        }
        return user;
    }
}
