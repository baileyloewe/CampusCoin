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
    string phoneNumber;

    [FirstnameValidation]
    [ObservableProperty]
    string firstName;

    [LastnameValidation]
    [ObservableProperty]
    string lastName;

    [ObservableProperty]
    string verificationCode;

    [ObservableProperty]
    private bool isVerificationVisible;

    [ObservableProperty]
    string errorText;

    [ObservableProperty]
    List<String> errorList;

    public bool IsNotBusy => !IsBusy;
    public bool VerificationEntered = false;

    RegistrationService registrationService;
    EmailService emailService;
    PersistedLoginService persistedLoginService;

    public ObservableCollection<User> UsersCollection { get; } = new();

    public RegistrationPageViewModel(RegistrationService registrationService, EmailService emailService, PersistedLoginService persistedLoginService ,IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.registrationService = registrationService;
        this.emailService = emailService;
        this.persistedLoginService = persistedLoginService;
        _messageOutputHandlingService=messageOutputHandlingService;
        this.isVerificationVisible = false;
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
                SetVisibilityOfVerification(true);
                await App.Current.MainPage.DisplayAlert("Code sent", "Verification Code was sent to: " + user.Email + "\n\nPlease allow up to 3 minutes for code to arrive", "OK");


                while (true)
                {
                    // Wait for the verification code to be entered
                    while (!VerificationEntered)
                    {
                        await Task.Delay(100); // Wait for .1 second before checking again
                    }

                    if (VerificationCode == emailService.verificationCode.ToString())
                    {
                        try
                        {
                            await registrationService.RegisterUser(user);
                            await EmailService.SendRegistrationSuccessEmail(user.Email);
                        }
                        catch (Exception ex)
                        {
                            // if it is a duplicate email error
                            Debug.WriteLine(ex);
                            if (ex.ToString().Contains("UniqueEmail"))
                            {
                                await App.Current.MainPage.DisplayAlert("Error", "Email already registered", "OK");
                                return;
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Error", "Something went wrong. Please try again.", "OK");
                                return;
                            }
                        }
                        finally
                        {
                            IsBusy = false;
                        }

                        await App.Current.MainPage.DisplayAlert("Account registered!", "Your account has been successfully registered", "OK");
                        ResetValues();
                        SetVisibilityOfVerification(false);
                        await Shell.Current.GoToAsync(nameof(MainPage));
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Invalid verification code. Please try again.", "OK");
                        VerificationCode = null;
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
        user.Salt = SaltHashService.GenerateSalt();
        user.Password = Password;
        user.PhoneNumber = PhoneNumber;
        user.FirstName = FirstName;
        user.LastName = LastName;
        return user;
    }

    public User HashUserPassword(User user)
    {
        user.Password = SaltHashService.HashPassword(Password, user.Salt);
        return user;
    }

    public void SetVisibilityOfVerification(bool visibileStatus)
    {
        IsVerificationVisible = visibileStatus;
    }

    public void ResetValues()
    {
        Email = null;
        Password = null;
        PhoneNumber = null;
        FirstName = null;
        LastName = null;
        VerificationCode = null;
        VerificationEntered = false;
    }


}
