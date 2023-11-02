using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CampusCoin.Views;
using CampusCoin.Validation;

namespace CampusCoin.ViewModels;

public partial class LoginPageViewModel : ObservableValidator
{
    private readonly IMessageOutputHandlingService _messageOutputHandlingService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;

    [EmailValidation]
    [ObservableProperty]
    string email;

    //[PasswordValidation]
    [ObservableProperty]
    string password;

    public bool IsNotBusy => !IsBusy;

    LoginService loginService;

    public ObservableCollection<Users> UsersCollection { get; } = new();

    Users user;

    public LoginPageViewModel(LoginService loginService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.loginService = loginService;
        _messageOutputHandlingService = messageOutputHandlingService;
    }

    [RelayCommand]
    async Task Login()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy= true;

            ValidateAllProperties();
            if (!HasErrors)
            {
                Users matchedUser = await loginService.GetUserByEmail(Email);

                if (matchedUser == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Email not registered", "OK");
                    return;
                }
                if (SaltHash.HashPassword(Password, matchedUser.Salt) != matchedUser.Password)
                    await Shell.Current.DisplayAlert("Error", "Incorrect Password", "OK");
                else
                Email = null;
                Password = null;
                // Temporary route to potential post-login view
                await Shell.Current.GoToAsync(nameof(ExpensesPage));
            }
            else
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
        }
        catch(Exception ex ) 
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error",
                $"Invalid email", "OK");
        }
        finally 
        { 
            IsBusy = false; 
        }
    }
}
