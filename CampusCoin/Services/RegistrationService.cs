using System.Net.Http.Json;
using CampusCoin.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCoin.Services;

public class RegistrationService
{
    private IDbContextFactory<CampusCoinContext> _context;
    public DbSet<Users> UsersList { get; set; }
    public string DbPath { get; }


    public RegistrationService(IDbContextFactory<CampusCoinContext> context)
    {
        _context = context;
    }

    List<Users> usersList = new();
    public async Task<List<Users>> GetUsers()
    {
        var dbContext = await _context.CreateDbContextAsync();
        return await dbContext.Users.ToListAsync();
    }

    public async Task RegisterUser(Users user)
    {
        var dbContext = await _context.CreateDbContextAsync();
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
    }
}
