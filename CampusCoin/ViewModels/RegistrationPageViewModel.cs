using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CampusCoin.Views;
using System.Collections.Generic;

namespace CampusCoin.ViewModels;

public partial class RegistrationPageViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    string email;

    [ObservableProperty]
    string password;

    [ObservableProperty]
    string phonenumber;

    [ObservableProperty]
    string firstname;

    [ObservableProperty]
    string lastname;

    [ObservableProperty]
    string errorText;

    [ObservableProperty]
    List<String> errorList;

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
            potentialUser.Password = Password;
            potentialUser.PhoneNumber = Phonenumber;
            potentialUser.FirstName = Firstname;
            potentialUser.LastName = Lastname;

            await registrationService.RegisterUser(potentialUser);

            // Change pages to main page
            // Pass the user to the page (likely not potential user but instead the user from DB including userID # for pulling data from DB)
            await Shell.Current.GoToAsync($"{nameof(MainPage)}?User={potentialUser}",
                new Dictionary<string, object>
                {
                    {nameof(MainPage), new object() }
                });
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
