using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CampusCoin.Views;
using System.Collections.Generic;
using CampusCoin.Validation;

namespace CampusCoin.ViewModels;

public partial class RegistrationPageViewModel : ObservableValidator
{

    private readonly IMessageOutputHandlingService _messageOutputHandlingService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    private string title;

    [EmailValidation]
    [ObservableProperty]
    string email;

    [PasswordValidation]
    [ObservableProperty]
    string password;

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
    string verificationcode;

    [ObservableProperty]
    private bool isVerificationCodeVisible;

    [ObservableProperty]
    private bool isVerificationCodeBtnVisible;

    [ObservableProperty]
    string errorText;

    [ObservableProperty]
    List<String> errorList;

    public bool IsNotBusy => !IsBusy;
    public bool VerificationEntered = false;

    RegistrationService registrationService;
    EmailService emailService;

    public ObservableCollection<User> UsersCollection { get; } = new();

    public RegistrationPageViewModel(RegistrationService registrationService, EmailService emailService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.registrationService = registrationService;
        this.emailService = emailService;
        _messageOutputHandlingService=messageOutputHandlingService;
        IsVerificationCodeBtnVisible = false;
        IsVerificationCodeVisible = false;
    }

    [RelayCommand]
    async Task GetUsersAsync()
    {
        try
        {
            var users = await registrationService.GetUsers();

            if (UsersCollection.Count != 0)
                UsersCollection.Clear();

            foreach (var user in users)
                UsersCollection.Add(user);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get users: {ex.Message}", "OK");
        }
        finally
        {
        }
    }

    [RelayCommand]
    async Task Registration()
    {
        ErrorText = "";
        try
        {
            var user = new User();
            await GetUsersAsync();
            user = SetUserVals(user);
            
            ValidateAllProperties();
            if (!HasErrors)
            {
                user = HashUserPassword(user);
                await emailService.SendVerificationEmail(user.Email);
                SetVisibilityOfVerificationButtons(true);
                await App.Current.MainPage.DisplayAlert("Code sent", "Verification Code was sent to: " + user.Email + "\n\nPlease allow up to 3 minutes for code to arrive", "OK");


                while (true)
                {
                    // Wait for the verification code to be entered
                    while (!VerificationEntered)
                    {
                        await Task.Delay(100); // Wait for .1 second before checking again
                    }

                    if (Verificationcode == emailService.verificationCode.ToString())
                    {

                        await registrationService.RegisterUser(user);
                        await EmailService.SendSuccessEmail(user.Email);
                        await App.Current.MainPage.DisplayAlert("Account registered!", "Your account has been successfully registered", "OK");
                        ResetValues();
                        SetVisibilityOfVerificationButtons(false);
                        // Change pages to graph test page
                        await Shell.Current.GoToAsync(nameof(GraphTestPage));
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Invalid verification code. Please try again.", "OK");
                        Verificationcode = null;
                        VerificationEntered = false; 
                    }
                }
            }
            else
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    [RelayCommand]
    private void Verification()
    {
        VerificationEntered = true;
    }

    public User SetUserVals(User user)
    {
        user.Email = Email;
        user.Salt = SaltHash.GenerateSalt();
        user.Password = Password;
        user.PhoneNumber = Phonenumber;
        user.FirstName = Firstname;
        user.LastName = Lastname;
        return user;
    }

    public User HashUserPassword(User user)
    {
        user.Password = SaltHash.HashPassword(Password, user.Salt);
        return user;
    }

    public void SetVisibilityOfVerificationButtons(bool visibileStatus)
    {
        IsVerificationCodeVisible = visibileStatus;
        IsVerificationCodeBtnVisible = visibileStatus;
    }

    public void ResetValues()
    {
        Email = null;
        Password = null;
        Phonenumber = null;
        Firstname = null;
        Lastname = null;
    }

}
