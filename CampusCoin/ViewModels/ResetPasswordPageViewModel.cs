﻿using CampusCoin.Services;
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


    [EmailValidation]
    [ObservableProperty]
    string email;

    [PasswordValidation]
    [ObservableProperty]
    string newPassword;

    [PasswordValidation]
    [ObservableProperty]
    string confirmNewPassword;

    [ObservableProperty]
    string verificationcode;

    [ObservableProperty]
    private bool isEmailVisible;

    [ObservableProperty]
    private bool isPasswordVisible;    

    [ObservableProperty]
    private bool isVerificationVisible;


    public bool VerificationEntered = false;

    User currentUser;

    LoginService loginService;
    EmailService emailService;
    PersistedLoginService persistedLoginService;
    EditUserAccountInfoService editUserInfoService;

    public ResetPasswordPageViewModel(LoginService loginService, EmailService emailService, PersistedLoginService persistedLoginService, EditUserAccountInfoService editUserInfoService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.loginService = loginService;
        this.emailService = emailService;
        this.editUserInfoService = editUserInfoService;
        this.persistedLoginService = persistedLoginService;
        _messageOutputHandlingService = messageOutputHandlingService;
        isEmailVisible = true;
        isVerificationVisible = false;
        isPasswordVisible = false;
    }

    [RelayCommand]
    async Task CheckEmail()
    {
         await emailService.SendPasswordResetEmail(Email);
         SetVisibilityOfEmail(false);
         SetVisibilityOfVerification(true);
         await App.Current.MainPage.DisplayAlert("Password Reset Verification Code Sent"
             , "A verification code has been sent to the email address associated with your CampusCoin account (" + Email +").\n\nPlease allow up to 3 minutes for the code to arrive."
             , "OK");
         // Wait for the verification code to be entered
         while (!VerificationEntered)
         {
             await Task.Delay(100); // Wait for .1 second before checking again
         }
         if (Verificationcode == emailService.verificationCode.ToString())
         {
             currentUser = await loginService.GetUserByEmail(Email);
             SetVisibilityOfVerification(false);
             SetVisibilityOfPassword(true);
         }
         else
         {
             await App.Current.MainPage.DisplayAlert("Error", "Invalid verification code. Please try again.", "OK");
             Verificationcode = null;
             VerificationEntered = false;
         }
    }

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
                await SavePasswordChange();
                ResetValues();
                await Shell.Current.GoToAsync(nameof(MainPage));
            }
        }
    }

    async Task SavePasswordChange()
    {
        await editUserInfoService.EditPassword(currentUser, NewPassword);
        await EmailService.SendPasswordResetSuccessEmail(currentUser.Email);
        await App.Current.MainPage.DisplayAlert("Password succesfully reset!", "Your password has been successfully reset", "OK");
    }

    [RelayCommand]
    private void Verification()
    {
        VerificationEntered = true;
    }

    public void SetVisibilityOfEmail(bool visibileStatus)
    {
        IsEmailVisible = visibileStatus;
    }

    public void SetVisibilityOfVerification(bool visibileStatus)
    {
        IsVerificationVisible = visibileStatus;
    }

    public void SetVisibilityOfPassword(bool visibileStatus)
    {
        IsPasswordVisible = visibileStatus;
    }

    public void ResetValues()
    {
        currentUser = null;
        Email = null;
        NewPassword = null;
        ConfirmNewPassword = null;
        Verificationcode = null;
        VerificationEntered = false;
        SetVisibilityOfEmail(true);
        SetVisibilityOfVerification(false); 
        SetVisibilityOfPassword(false);
    }
}
