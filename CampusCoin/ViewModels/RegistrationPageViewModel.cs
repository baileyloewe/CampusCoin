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
    string errorText;

    [ObservableProperty]
    List<String> errorList;

    public bool IsNotBusy => !IsBusy;

    RegistrationService registrationService;

    public ObservableCollection<Users> UsersCollection { get; } = new();

    public RegistrationPageViewModel(RegistrationService registrationService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.registrationService = registrationService;
        _messageOutputHandlingService=messageOutputHandlingService;
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
                await registrationService.RegisterUser(potentialUser);

                Email = null;
                Password = null;
                Phonenumber = null;
                Firstname = null;
                Lastname = null;

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
