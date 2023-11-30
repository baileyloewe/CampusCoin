using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CampusCoin.Views;
using CampusCoin.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CampusCoin.ViewModels;

public partial class EditUserAccountInfoPageViewModel : ObservableValidator
{
    private readonly LoginService loginService;
    private readonly EmailService emailService;
    private readonly PersistedLoginService persistedLoginService;
    private readonly EditUserAccountInfoService editUserInfoService;
    private readonly IMessageOutputHandlingService _messageOutputHandlingService;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string userEmail;

    [EmailValidation]
    [ObservableProperty]
    private string email;

    [ObservableProperty]
    bool editEmailEntry;

    [ObservableProperty]
    bool editEmailFields;

    [PasswordValidation]
    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private bool editPasswordEntry;

    [ObservableProperty]
    bool editPasswordFields;

    [ObservableProperty]
    private string userPhoneNumber;

    [PhoneNumberValidation]
    [ObservableProperty]
    private string phoneNumber;

    [ObservableProperty]
    bool editPhoneNumberEntry;

    [ObservableProperty]
    bool editPhoneNumberFields;

    [ObservableProperty]
    private string userFirstName;

    [FirstnameValidation]
    [ObservableProperty]
    private string firstName;

    [ObservableProperty]
    bool editFirstNameEntry;

    [ObservableProperty]
    bool editFirstNameFields;

    [ObservableProperty]
    private string userLastName;

    [LastnameValidation]
    [ObservableProperty]
    private string lastName;

    [ObservableProperty]
    bool editLastNameEntry;

    [ObservableProperty]
    bool editLastNameFields;

    User currentUser;

    public EditUserAccountInfoPageViewModel(LoginService loginService, EmailService emailService, PersistedLoginService persistedLoginService, EditUserAccountInfoService editUserInfoService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.loginService = loginService;
        this.emailService = emailService;
        this.editUserInfoService = editUserInfoService;
        this.persistedLoginService = persistedLoginService;
        _messageOutputHandlingService = messageOutputHandlingService;
        initializeVals();
        _=setCurrentUser();
    }

    [RelayCommand]
    private void EditEmail()
    {
        Email = null;
        EditEmailEntry = !EditEmailEntry;
        EditEmailFields = !EditEmailFields;
    }

    [RelayCommand]
    private void EditPassword()
    {
        Password = null;
        EditPasswordEntry = !EditPasswordEntry;
        EditPasswordFields = !EditPasswordFields;
    }

    [RelayCommand]
    private void EditPhoneNumber()
    {
        PhoneNumber = null;
        EditPhoneNumberEntry = !EditPhoneNumberEntry;
        EditPhoneNumberFields = !EditPhoneNumberFields;
    }

    [RelayCommand]
    private void EditFirstName()
    {
        FirstName = null;
        EditFirstNameEntry = !EditFirstNameEntry;
        EditFirstNameFields = !EditFirstNameFields;
    }

    [RelayCommand]
    private void EditLastName()
    {
        LastName = null;
        EditLastNameEntry = !EditLastNameEntry;
        EditLastNameFields = !EditLastNameFields;
    }

    [RelayCommand]
    async Task SaveEmail()
    {
        if (!string.IsNullOrEmpty(Email))
        {
            ValidateProperty(Email, nameof(Email));
            if (!HasErrors)
            {
                await ChangeEmail();
                await Shell.Current.DisplayAlert("Account Email Change Submitted", "Your account's email has been changed", "OK");
                UserEmail = persistedLoginService.getLoggedInUser().Email;
            }
            else
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
            EditEmailEntry = true;
            EditEmailFields = false;
            Email = null;
        }
    }

    [RelayCommand]
    async Task SavePassword()
    {
        if (!string.IsNullOrEmpty(Password))
        {
            ValidateProperty(Password, nameof(Password));
            if (!HasErrors)
            {
                await ChangePassword();
                await Shell.Current.DisplayAlert("Account Password Change Submitted", "Your account's email has been changed", "OK");
            }
            else
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
            EditPasswordEntry = true;
            EditPasswordFields = false;
            Password = null;
        }
    }

    [RelayCommand]
    async Task SavePhoneNumber()
    {
        if (!string.IsNullOrEmpty(PhoneNumber))
        {
            ValidateProperty(PhoneNumber, nameof(PhoneNumber));
            if (!HasErrors)
            {
                await ChangePhoneNumber();
                await Shell.Current.DisplayAlert("Account Phone Number Change Submitted", "Your account's email has been changed", "OK");
                UserPhoneNumber = persistedLoginService.getLoggedInUser().PhoneNumber;
            }
            else
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
            EditPhoneNumberEntry = true;
            EditPhoneNumberFields = false;
            PhoneNumber = null;
        }
    }

    [RelayCommand]
    async Task SaveFirstName()
    {
        if (!string.IsNullOrEmpty(FirstName))
        {
            ValidateProperty(FirstName, nameof(FirstName));
            if (!HasErrors)
            {
                await ChangeFirstName();
                await Shell.Current.DisplayAlert("Account First Name Change Submitted", "Your account's email has been changed", "OK");
                UserFirstName = persistedLoginService.getLoggedInUser().FirstName;
            }
            else
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
            EditFirstNameEntry = true;
            EditFirstNameFields = false;
            FirstName = null;
        }
    }

    [RelayCommand]
    async Task SaveLastName()
    {
        if (!string.IsNullOrEmpty(LastName))
        {
            ValidateProperty(LastName, nameof(LastName));
            if (!HasErrors)
            {
                await ChangeLastName();
                await Shell.Current.DisplayAlert("Account Last Name Change Submitted", "Your account's email has been changed", "OK");
                UserLastName = persistedLoginService.getLoggedInUser().LastName;
            }
            else
            {
               await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
            EditLastNameEntry = true;
            EditLastNameFields = false;
            LastName = null;
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
        await editUserInfoService.EditPhoneNumber(currentUser, PhoneNumber);
    }

    [RelayCommand]
    async Task ChangeFirstName()
    {
        await editUserInfoService.EditFirstName(currentUser, FirstName);
    }

    [RelayCommand]
    async Task ChangeLastName()
    {
        await editUserInfoService.EditLastName(currentUser, LastName);
    }

    public void ResetValues()
    {
        Email = null;
        Password = null;
        PhoneNumber = null;
        FirstName = null;
        LastName = null;
        UserEmail = null;
        UserPhoneNumber = null;
        UserFirstName = null;
        UserLastName = null;
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

    public void initializeVals() 
    {
        User user = persistedLoginService.getLoggedInUser();
        UserEmail = user.Email;
        UserPhoneNumber = user.PhoneNumber;
        UserFirstName = user.FirstName;
        UserLastName = user.LastName;

        EditEmailEntry = true;
        EditPasswordEntry = true;
        EditPhoneNumberEntry = true;
        EditFirstNameEntry = true;
        EditLastNameEntry = true;

        EditEmailFields = false;
        EditPasswordFields = false;
        EditPhoneNumberFields = false;
        EditFirstNameFields = false;
        EditLastNameFields = false;
    }

 

}
