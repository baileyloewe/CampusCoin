using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CampusCoin.Views;

namespace CampusCoin.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;

    [ObservableProperty]
    string email;

    [ObservableProperty]
    string password;

    public bool IsNotBusy => !IsBusy;

    LoginService loginService;

    public ObservableCollection<Users> UsersCollection { get; } = new();

    Users user;

    public LoginPageViewModel(LoginService loginService)
    {
        this.loginService = loginService;
    }

    [RelayCommand]
    async Task Login()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy= true;

            Console.WriteLine($"Bug");
            user = await loginService.GetUserByEmail(Email);

            if (user.Password == SaltHash.HashPassword(Password, user.Salt))
            {
                // Change pages to main page
                // Pass the user to the page (likely not user but instead the user from DB including userID # for pulling data from DB)
                await Shell.Current.GoToAsync($"{nameof(MainPage)}?User={user}",
                    new Dictionary<string, object>
                    {
                    {nameof(MainPage), new object() }
                    });
            }
        }
        catch(Exception ex ) 
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Incorrect Email and Password Combination: {ex.Message}", "OK");
        }
        finally 
        { 
            IsBusy = false; 
        }
    }
}
