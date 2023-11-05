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

    /// <summary>
    /// Generates a verification code
    /// </summary>
    /// <param></param>
    /// <returns></returns>
    public void GenerateCode()
    {
        verificationCode = new Random().Next(10000000, 100000000);
    }

    /// <summary>
    /// Outputs validation errors to the user
    /// </summary>
    /// <param name="subject">Subject of the email</param>
    /// <param name="body">Body of the email</param>
    /// <param name="userEmail">The email address that will recieve this email</param>
    /// <param name="isHtml">Indicates whether the body of the email is a string or html</param>
    /// <returns></returns>
    private static async Task SendEmail(string subject, string body, string userEmail, bool isHtml)
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

    /// <summary>
    /// Sends a verification email
    /// </summary>
    /// <param name="userEmail">The email address that will recieve this email</param>
    /// <returns></returns>
    public async Task SendVerificationEmail(string userEmail)
    {
        GenerateCode();
        string subject = "Account Verification for CampusCoin";
        string htmlBody = "<html><body>";
        htmlBody += "<h1>Your verification code is below</h1>";
        htmlBody += "<p>Your verification code is:   <strong><font size = '6'>" + verificationCode + "</font></strong></p>";
        htmlBody += "<p><br /></p>";
        htmlBody += "<p>Enter your code into the CampusCoin app to verify your email address and complete your Registration</p>";
        htmlBody += "</body></html>";
        await EmailService.SendEmail(subject, htmlBody, userEmail, true);
    }
    /// <summary>
    /// Sends a sucessful sign up email
    /// </summary>
    /// <param name="userEmail">The email address that will recieve this email</param>
    /// <returns></returns>
    public static async Task SendRegistrationSuccessEmail(string userEmail)
    {   
        string subject = "Registration Successful for CampusCoin";
        string htmlBody = "<html><body>";
        htmlBody += "<h1>Welcome to CampusCoin!</h1>";
        htmlBody += "<p>Your email for future logins: <strong>" + userEmail + "</strong></p>";
        htmlBody += "</body></html>";
        await EmailService.SendEmail(subject, htmlBody, userEmail, true);
    }

    /// <summary>
    /// Sends a reset password email
    /// </summary>
    /// <param name="userEmail">The email address that will recieve this email</param>
    /// <returns></returns>
    public async Task SendPasswordResetEmail(string userEmail)
    {
        GenerateCode();
        string subject = "Password Reset for Campus Coin";
        string htmlBody = "<html><body>";
        htmlBody += "<h1>Your verification code is below</h1>";
        htmlBody += "<p>Your verification code for resetting your password is:   <strong><font size = '6'>" + verificationCode + "</font></strong></p>";
        htmlBody += "<p><br /></p>";
        htmlBody += "<p>Enter your code into the CampusCoin app to reset your password</p>";
        htmlBody += "</body></html>";
        await EmailService.SendEmail(subject, htmlBody, userEmail, true);
    }

    /// <summary>
    /// Sends a sucessful password reset email
    /// </summary>
    /// <param name="userEmail">The email address that will recieve this email</param>
    /// <returns></returns>
    public static async Task SendPasswordResetSuccessEmail(string userEmail)
    {
        string subject = "Registration Successful for CampusCoin";
        string htmlBody = "<html><body>";
        htmlBody += "<h1>Welcome to CampusCoin!</h1>";
        htmlBody += "<p>Your email for future logins: <strong>" + userEmail + "</strong></p>";
        htmlBody += "</body></html>";
        await EmailService.SendEmail(subject, htmlBody, userEmail, true);
    }

}