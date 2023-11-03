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
    string errorText;

    [ObservableProperty]
    List<String> errorList;

    public bool IsNotBusy => !IsBusy;

    RegistrationService registrationService;
    EmailService emailService;

    public ObservableCollection<Users> UsersCollection { get; } = new();

    public RegistrationPageViewModel(RegistrationService registrationService, EmailService emailService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.registrationService = registrationService;
        this.emailService = emailService;
        _messageOutputHandlingService=messageOutputHandlingService;
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
            potentialUser.Password = SaltHash.HashPassword(Password, potentialUser.Salt);
            potentialUser.PhoneNumber = Phonenumber;
            potentialUser.FirstName = Firstname;
            potentialUser.LastName = Lastname;

            ValidateAllProperties();
            if (!HasErrors)
            {
                await emailService.SendVerificationEmail(potentialUser.Email);
                await App.Current.MainPage.DisplayAlert("Code sent", "Verification Code was sent to: " + potentialUser.Email + "\nPlease allow up to 2 minutes for email to arrive", "OK");
                IsVerificationCodeVisible = true;

                while (true)
                {
                    // Wait for the verification code to be entered
                    while (string.IsNullOrEmpty(Verificationcode))
                    {
                        await Task.Delay(100); // Wait for .1 second before checking again
                    }

                    if (Verificationcode == emailService.verificationCode.ToString())
                    {

                        await registrationService.RegisterUser(potentialUser);

                        Email = null;
                        Password = null;
                        Phonenumber = null;
                        Firstname = null;
                        Lastname = null;

                        IsVerificationCodeVisible = false;
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
}
