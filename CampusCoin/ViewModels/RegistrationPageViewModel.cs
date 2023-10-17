using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CampusCoin.Views;

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
            System.Diagnostics.Debug.WriteLine("users[0]:", users[0]);

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

    private Boolean UserExistsWithEmail(Users potentialUser)
    {
        try
        {
            foreach (var user in UsersCollection)
            {
                System.Diagnostics.Debug.WriteLine("Here");
                System.Diagnostics.Debug.WriteLine(user.Email);
                if (user.Email == potentialUser.Email)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        return true;
    }

    [RelayCommand]
    async Task Registration()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            var potentialUser = new Users();
            await GetUsersAsync();


            if (!UserExistsWithEmail(potentialUser))
            {
                potentialUser.Email = Email;
                potentialUser.Password = Password;
                potentialUser.PhoneNumber = Phonenumber;
                potentialUser.FirstName = Firstname;
                potentialUser.LastName = Lastname;


                await registrationService.RegisterUser(potentialUser);


                // Change pages (likely not to main page but to the page post login and authetication)
                // Pass the user to the page (likely not potential user but instead the user from DB including userID # for pulling data from DB)
                await Shell.Current.GoToAsync($"{nameof(MainPage)}?User={potentialUser}",
                    new Dictionary<string, object>
                    {
                        {nameof(MainPage), new object() }
                    });
            }
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
