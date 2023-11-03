using System.Diagnostics;
using CampusCoin.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Net;

namespace CampusCoin.Services;

public class EmailService
{
    private IDbContextFactory<CampusCoinContext> _context;
    public string DbPath { get; }
    //List<String> credentialsList = new();
    public int verificationCode;

    public EmailService(IDbContextFactory<CampusCoinContext> context)
    { 
        _context = context;
        GenerateCode();
    }

    //public async Task GetCredentials()
    //{
        //var dbContext = await _context.CreateDbContextAsync();
        //credentialsList = await dbContext.AppEmail.ToListAsync();
    //}

    public void GenerateCode()
    {
        verificationCode = new Random().Next(10000000, 100000000);
    }

    public async Task SendVerificationEmail(string userEmail)
    {
        // GetCredentials();

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("CampusCoinApp@gmail.com", "cpjo roby hgjj iejy"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("CampusCoinApp@gmail.com"),
            Subject = "Email Verification for CampusCoin",
            Body = "Your CampusCoin Verification code is " + verificationCode,
            IsBodyHtml = false,
        };

        mailMessage.To.Add(userEmail);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to send email: " + ex.Message);
        }
    }
}