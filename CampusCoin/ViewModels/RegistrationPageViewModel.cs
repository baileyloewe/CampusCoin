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

    public ObservableCollection<Users> UsersCollection { get; } = new();

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
        if (IsBusy)
            return;
        ErrorText = "";
        try
        {
            IsBusy = true;
            var potentialUser = new Users();
            await GetUsersAsync();

            potentialUser.Email = Email;
            potentialUser.Salt = SaltHash.GenerateSalt();
            potentialUser.Password = Password;
            potentialUser.PhoneNumber = Phonenumber;
            potentialUser.FirstName = Firstname;
            potentialUser.LastName = Lastname;

            ValidateAllProperties();
            if (!HasErrors)
            {
                potentialUser.Password = SaltHash.HashPassword(Password, potentialUser.Salt);
                await emailService.SendVerificationEmail(potentialUser.Email);
                IsVerificationCodeVisible = true;
                IsVerificationCodeBtnVisible = true;
                await App.Current.MainPage.DisplayAlert("Code sent", "Verification Code was sent to: " + potentialUser.Email + "\n\nPlease allow up to 3 minutes for code to arrive", "OK");


                while (true)
                {
                    // Wait for the verification code to be entered
                    while (!VerificationEntered)
                    {
                        await Task.Delay(100); // Wait for .1 second before checking again
                    }

                    if (Verificationcode == emailService.verificationCode.ToString())
                    {

                        await registrationService.RegisterUser(potentialUser);
                        await emailService.SendSuccessEmail(potentialUser.Email);
                        await App.Current.MainPage.DisplayAlert("Account registered!", "Your account has been successfully registered", "OK");
                        Email = null;
                        Password = null;
                        Phonenumber = null;
                        Firstname = null;
                        Lastname = null;

                        IsVerificationCodeVisible = false;
                        IsVerificationCodeBtnVisible= false;
                        // Change pages to main page
                        // Pass the user to the GraphTestPage (likely not potential user? later but instead the user from DB including userID # for pulling data from DB)

                        await Shell.Current.GoToAsync($"{nameof(GraphTestPage)}?User={potentialUser}",
                            new Dictionary<string, object>
                            {
                            {nameof(GraphTestPage), new object() }
                            });
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
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Verification()
    {
        VerificationEntered = true;
    }
}
