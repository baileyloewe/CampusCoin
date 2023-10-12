using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CampusCoin.ViewModels;

public partial class RegistrationPageViewModel : ObservableObject
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

    RegistrationService registrationService;

    public ObservableCollection<Users> UsersCollection { get; } = new();

    public RegistrationPageViewModel(RegistrationService registrationService)
    {
        this.registrationService = registrationService;
    }

    [RelayCommand]
    async Task GetUsersAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
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
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task Registration()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy= true;

            Console.WriteLine($"Bug");
            var potentialUser = new Users();
            potentialUser.Email = Email;
            potentialUser.Password = Password;

        }
        catch (Exception ex)
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
