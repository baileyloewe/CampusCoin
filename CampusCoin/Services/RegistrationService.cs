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
    List<Users> usersList = new();

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
}