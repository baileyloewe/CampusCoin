using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using CampusCoin.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCoin.Services;

public class EmailService
{
    private readonly IDbContextFactory<CampusCoinContext> _context;
    public int verificationCode { get; private set; }

    public EmailService(IDbContextFactory<CampusCoinContext> context)
    {
        _context = context;
    }

    public void GenerateCode()
    {
        verificationCode = new Random().Next(10000000, 100000000);
    }

    private async Task SendEmail(string subject, string body, string userEmail, bool isHtml)
    {
        try
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("CampusCoinApp@gmail.com", "cpjo roby hgjj iejy"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage { };

            if (isHtml)
            {
                mailMessage = new MailMessage
                {
                    From = new MailAddress("CampusCoinApp@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
            }
            else
            {
                mailMessage = new MailMessage
                {
                    From = new MailAddress("CampusCoinApp@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };
            }

            mailMessage.To.Add(userEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to send email: " + ex.Message);
        }
    }

    public async Task SendVerificationEmail(string userEmail)
    {
        GenerateCode();
        string subject = "Account Verification for CampusCoin";
        string htmlBody = "<html><body style='text-align: center;'>";
        htmlBody += "<h1>Your verification code is below</h1>";
        htmlBody += "<p>Your verification code is:   <strong><font size = '6'>" + verificationCode + "</font></strong></p>";
        htmlBody += "</body></html>";
        await SendEmail(subject, htmlBody, userEmail, true);
    }

    public async Task SendSuccessEmail(string userEmail)
    {   
        string subject = "Registration Successful for CampusCoin";
        string htmlBody = "<html><body style='text-align: center;'>";
        htmlBody += "<h1>Welcome to CampusCoin!</h1>";
        htmlBody += "<p>Your email for future sign-ins is: <strong>" + userEmail + "</strong></p>";
        htmlBody += "</body></html>";
        await SendEmail(subject, htmlBody, userEmail, true);
    }


}