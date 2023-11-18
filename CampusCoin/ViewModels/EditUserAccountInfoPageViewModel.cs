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

public partial class EditUserAccountInfoPageViewModel : ObservableValidator
{

    [ObservableProperty]
    string title;

    [EmailValidation]
    [ObservableProperty]
    string email;

    [PasswordValidation]
    [ObservableProperty]
    string password;

    [PasswordValidation]
    [ObservableProperty]
    string confirmPassword;

    [PhoneNumberValidation]
    [ObservableProperty]
    string phoneNumber;

    [FirstnameValidation]
    [ObservableProperty]
    string firstName;

    [LastnameValidation]
    [ObservableProperty]
    string lastName;

    [ObservableProperty]
    string userEmail;

    [ObservableProperty]
    string userPhoneNumber;

    [ObservableProperty]
    string userFirstName;

    [ObservableProperty]
    string userLastName;

    User currentUser;

    LoginService loginService;
    EmailService emailService;
    PersistedLoginService persistedLoginService;
    EditUserAccountInfoService editUserInfoService;

    public EditUserAccountInfoPageViewModel(LoginService loginService, EmailService emailService, PersistedLoginService persistedLoginService, EditUserAccountInfoService editUserInfoService)
    {
        this.loginService = loginService;
        this.emailService = emailService;
        this.editUserInfoService = editUserInfoService;
        this.persistedLoginService = persistedLoginService;
    }

    [RelayCommand]
    async Task SaveChanges() 
    { 
        if (!UserEmail.Equals(Email))
        {
            await ChangeEmail();
        }
       
        if (!UserEmail.Equals(FirstName))
        {
            await ChangeFirstName();
        }
        if (!UserEmail.Equals(LastName))
        {
            await ChangeLastName();
        }
        if (!UserEmail.Equals(PhoneNumber))
        {
            await ChangePhoneNumber();
        }
    }

    [RelayCommand]
    async Task ChangeEmail()
    {
        await editUserInfoService.EditEmail(currentUser, Email);
    }

    [RelayCommand]
    async Task ChangePassword()
    {
        await editUserInfoService.EditPassword(currentUser, SaltHashService.HashPassword(Password, currentUser.Salt));
    }

    [RelayCommand]
    async Task ChangePhoneNumber()
    {
        await editUserInfoService.EditLastName(currentUser, PhoneNumber);
    }

    [RelayCommand]
    async Task ChangeFirstName()
    {
        await editUserInfoService.EditPhoneNumber(currentUser, FirstName);
    }

    [RelayCommand]
    async Task ChangeLastName()
    {
        await editUserInfoService.EditFirstName(currentUser, LastName);
    }

    public void ResetValues()
    {
        Email = null;
        Password = null;
        PhoneNumber = null;
        FirstName = null;
        LastName = null;
    }

    public async Task setCurrentUser()
    {
        currentUser = persistedLoginService.getLoggedInUser();
        if (currentUser == null)
        {
            await Shell.Current.DisplayAlert("Error",
                $"Something went wrong :(", "OK");
            await Shell.Current.GoToAsync(nameof(MainPage));
        }
    }

    public void clearCurrentUser()
    {
        currentUser = null;
    }

 

}
