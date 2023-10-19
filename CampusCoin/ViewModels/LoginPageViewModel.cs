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

    public LoginPageViewModel(LoginService loginService)
    {
        this.loginService = loginService;
    }

    [RelayCommand]
    async Task GetUserByEmailAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var user = await loginService.GetUserByEmail(Email);
    
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

            var potentialUser = new Users();
            potentialUser.Email = Email;
            potentialUser.Password = Password;

            Users matchedUser = await loginService.GetUserByEmail(Email);

            if (Password!= matchedUser.Password)
                await Shell.Current.DisplayAlert("Error", "Invalid Password", "OK");

            else
                // Temporary route to potential post-login view
                await Shell.Current.GoToAsync(nameof(GraphTestPage));
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
