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
    public ObservableCollection<User> UsersCollection { get; } = new();
    List<User> usersList = new();

    public RegistrationService(IDbContextFactory<CampusCoinContext> context)
    {
        _context = context;
    }

    /// <summary> Gets a list of all users from the database </summary>
    /// <param></param>
    /// <returns> A list of users from the database </returns>
    public async Task<List<User>> GetUsers()
    {
        var dbContext = await _context.CreateDbContextAsync();
        usersList = await dbContext.Users.ToListAsync();
        return usersList;
    }
    /// <summary> Registers a user in the database, then saves the database </summary>
    /// <param name="user">The user to be saved</param>
    /// <returns></returns>
    public async Task RegisterUser(User user)
    {
        var dbContext = await _context.CreateDbContextAsync(); // Create database context
        dbContext.Users.Add(user); // Add the user to the database
        dbContext.SaveChanges(); // Save the changes to the database
    }
}