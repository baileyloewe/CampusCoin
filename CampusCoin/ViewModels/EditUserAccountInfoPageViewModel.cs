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
    string phonenumber;

    [FirstnameValidation]
    [ObservableProperty]
    string firstname;

    [LastnameValidation]
    [ObservableProperty]
    string lastname;

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
       
        if (!UserEmail.Equals(Firstname))
        {
            await ChangeFirstName();
        }
        if (!UserEmail.Equals(Lastname))
        {
            await ChangeLastName();
        }
        if (!UserEmail.Equals(Phonenumber))
        {
            await ChangePhoneNumber();
        }
    }

    [RelayCommand]
    async Task ChangeEmail()
    {
        await editUserInfoService.EditEmail(currentUser, "test@gmail.com");
    }

    [RelayCommand]
    async Task ChangePassword()
    {
        await editUserInfoService.EditPassword(currentUser, "test");
    }

    [RelayCommand]
    async Task ChangePhoneNumber()
    {
        await editUserInfoService.EditLastName(currentUser, "test1");
    }

    [RelayCommand]
    async Task ChangeFirstName()
    {
        await editUserInfoService.EditPhoneNumber(currentUser, "test2");
    }

    [RelayCommand]
    async Task ChangeLastName()
    {
        await editUserInfoService.EditFirstName(currentUser, "test3");
    }

    public void ResetValues()
    {
        Email = null;
        Password = null;
        Phonenumber = null;
        Firstname = null;
        Lastname = null;
    }

    public async Task setCurrentUser()
    {
        currentUser = persistedLoginService.GetUser();
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
