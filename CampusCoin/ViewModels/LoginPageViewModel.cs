using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CampusCoin.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;

    [ObservableProperty]
    string username;

    [ObservableProperty]
    string password;

    public bool IsNotBusy => !IsBusy;

    LoginService loginService;

    public ObservableCollection<Users> UsersCollection { get; } = new();

    public LoginPageViewModel(LoginService loginService)
    {
        this.loginService = loginService;
    }

    [RelayCommand]
    async Task GetUsersAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var users = await loginService.GetUsers();

            if (UsersCollection.Count != 0)
                UsersCollection.Clear();

            foreach (var user in users)
                UsersCollection.Add(user);
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get users: {ex.Message}" , "OK");
        }
        finally
        {
            IsBusy = false;
        }
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
            var potentialUser = new Users();
            potentialUser.Username = Username;
            potentialUser.Password = Password;

        }
        catch(Exception ex ) 
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Incorrect Username and Password Combination: {ex.Message}", "OK");
        }
        finally 
        { 
            IsBusy = false; 
        }
    }
}
