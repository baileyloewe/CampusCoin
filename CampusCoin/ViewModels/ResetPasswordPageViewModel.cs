using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CampusCoin.Views;
using CampusCoin.Validation;
using Microsoft.EntityFrameworkCore;

namespace CampusCoin.ViewModels;

public partial class ResetPasswordPageViewModel : ObservableValidator
{
    private readonly IMessageOutputHandlingService _messageOutputHandlingService;
    private readonly LoginService loginService;
    private readonly EmailService emailService;
    private readonly PersistedLoginService persistedLoginService;
    private readonly EditUserAccountInfoService editUserAccountInfoService;

    [EmailValidation]
    [ObservableProperty]
    private string email;

    [PasswordValidation]
    [ObservableProperty]
    private string newPassword;

    [ObservableProperty]
    private string confirmNewPassword;

    [ObservableProperty]
    private string verificationCode;

    [ObservableProperty]
    private bool isEmailVisible;

    [ObservableProperty]
    private bool isPasswordVisible;    

    [ObservableProperty]
    private bool isVerificationVisible;

    private bool VerificationEntered = false;

    private User currentUser;

    public ResetPasswordPageViewModel(LoginService loginService, EmailService emailService, PersistedLoginService persistedLoginService, EditUserAccountInfoService editUserAccountInfoService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.loginService = loginService;
        this.emailService = emailService;
        this.editUserAccountInfoService = editUserAccountInfoService;
        this.persistedLoginService = persistedLoginService;
        _messageOutputHandlingService = messageOutputHandlingService;
        isEmailVisible = true;
        isVerificationVisible = false;
        isPasswordVisible = false;
    }

    // Sends a password reset email and displays a message
    [RelayCommand]
    private async Task SendCode()
    {
         await emailService.SendPasswordResetEmail(Email);
         SetVisibilityOfEmail(false);
         SetVisibilityOfVerification(true);
         await App.Current.MainPage.DisplayAlert("Password Reset Verification Code Sent"
             , "A verification code has been sent to the email address associated with your CampusCoin account (" + Email +").\n\nPlease allow up to 3 minutes for the code to arrive."
             , "OK");
    }
    
    // Submits your verification code and if it is correct, hides the buttons, else reject it and reset the field
    [RelayCommand]
    private async Task SubmitVerificationCode()
    {
        if (VerificationCode == emailService.verificationCode.ToString())
        {
            currentUser = await loginService.GetUserByEmail(Email);
            SetVisibilityOfVerification(false);
            SetVisibilityOfPassword(true);
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Invalid verification code. Please try again.", "OK");
            VerificationCode = null;
            VerificationEntered = false;
        }
    }
    
    // Validates then calls the savepasswordchange function to save the password to database
    [RelayCommand]
    private async Task SubmitPasswordChange()
    {
        if (NewPassword != null && ConfirmNewPassword != null)
        {
            if (NewPassword != ConfirmNewPassword)
            {
                await App.Current.MainPage.DisplayAlert("Your Passwords do not match", "Passwords must be the same", "OK");
                NewPassword = null;
                ConfirmNewPassword = null;
            }
            else
            {
                ValidateAllProperties();
                if (!HasErrors)
                {
                    await SavePasswordChange();
                    ResetValues();
                    await Shell.Current.GoToAsync(nameof(MainPage));
                }
                else
                {
                    await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
                    NewPassword = "";
                    ConfirmNewPassword = "";
                }            
            }
        }
    }

    // Saves the password to the database
    private async Task SavePasswordChange()
    {
        await editUserAccountInfoService.EditPassword(currentUser, SaltHashService.HashPassword(NewPassword, currentUser.Salt));
        await EmailService.SendPasswordResetSuccessEmail(currentUser.Email);
        await App.Current.MainPage.DisplayAlert("Password succesfully reset!", "Your password has been successfully reset", "OK");
      
    }

    // Sets the VerificationEntered ObservableProperty to true
    [RelayCommand]
    private void Verification()
    {
        VerificationEntered = true;
    }

    // Sets the isEmailVisible ObservableProperty to visibleStatus (t/f)
    private void SetVisibilityOfEmail(bool visibileStatus)
    {
        IsEmailVisible = visibileStatus;
    }

    // Sets the isVerificationVisible ObservableProperty to visibleStatus (t/f)
    private void SetVisibilityOfVerification(bool visibileStatus)
    {
        IsVerificationVisible = visibileStatus;
    }

    // Sets the isPasswordVisible ObservableProperty to visibleStatus (t/f)
    private void SetVisibilityOfPassword(bool visibileStatus)
    {
        IsPasswordVisible = visibileStatus;
    }

    // Resets all values on the page
    private void ResetValues()
    {
        currentUser = null;
        Email = null;
        NewPassword = null;
        ConfirmNewPassword = null;
        VerificationCode = null;
        VerificationEntered = false;
        SetVisibilityOfEmail(true);
        SetVisibilityOfVerification(false); 
        SetVisibilityOfPassword(false);
    }
}
