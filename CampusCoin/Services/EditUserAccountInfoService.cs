using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.Mail;
using CampusCoin.Models;
using CampusCoin.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace CampusCoin.Services;

public class EditUserAccountInfoService
{
    private IDbContextFactory<CampusCoinContext> _context;
    public string DbPath { get; }
    public ObservableCollection<User> UsersCollection { get; } = new();

    public EditUserAccountInfoService(IDbContextFactory<CampusCoinContext> context)
    {
        _context = context;
    }

    public async Task EditEmail(User user, String email)
    {
        var dbContext = await _context.CreateDbContextAsync(); // Create database context
        dbContext.Users
                .Where(u => u.UserId == user.UserId)
                .ExecuteUpdate(b =>
                b.SetProperty(u => u.Email, email)); // Edit the user's email
        dbContext.SaveChanges(); // Save the changes to the database
    }

    public async Task EditPassword(User user, String password)
    {
        var dbContext = await _context.CreateDbContextAsync(); // Create database context
        dbContext.Users
                .Where(u => u.UserId == user.UserId)
                .ExecuteUpdate(b =>
                b.SetProperty(u => u.Password, password)); // Edit the user's password
        dbContext.SaveChanges(); // Save the changes to the database
    }

    public async Task EditPhoneNumber(User user, String phoneNumber)
    {
        var dbContext = await _context.CreateDbContextAsync(); // Create database context
        dbContext.Users
                .Where(u => u.UserId == user.UserId)
                .ExecuteUpdate(b =>
                b.SetProperty(u => u.PhoneNumber, phoneNumber)); // Edit the user's phone number
        dbContext.SaveChanges(); // Save the changes to the database
    }

    public async Task EditFirstName(User user, String firstName)
    {
        var dbContext = await _context.CreateDbContextAsync(); // Create database context
        dbContext.Users
                .Where(u => u.UserId == user.UserId)
                .ExecuteUpdate(b =>
                b.SetProperty(u => u.FirstName, firstName)); // Edit the user's first name
        dbContext.SaveChanges(); // Save the changes to the database
    }

    public async Task EditLastName(User user, String lastName)
    {
        var dbContext = await _context.CreateDbContextAsync(); // Create database context
        dbContext.Users
                .Where(u => u.UserId == user.UserId)
                .ExecuteUpdate(b =>
                b.SetProperty(u => u.LastName, lastName)); // Edit the user's last name
        dbContext.SaveChanges(); // Save the changes to the database
    }
}