using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;
using CampusCoin.Models;
using CampusCoin.Services;
using Microsoft.EntityFrameworkCore;
using CampusCoin.Views;
using System.Windows.Input;
using System.Diagnostics;

namespace CampusCoin.ViewModels
{
    public partial class MainPageViewModel : ObservableValidator
    {
        private readonly IMessageOutputHandlingService _messageOutputHandlingService;
        private readonly IDbContextFactory<CampusCoinContext> _context;
        private readonly PersistedLoginService _persistedLoginService;

        // note that the IMessageOutputHandlingService is injected into the constructor- https://learn.microsoft.com/en-us/dotnet/architecture/maui/dependency-injection
        // this is necessary in our case for unit tests to be able to mock the service
        public MainPageViewModel(IMessageOutputHandlingService messageOutputHandlingService, IDbContextFactory<CampusCoinContext> context, PersistedLoginService persistedLoginService)
        {
            // Assign injected services to private variables
            _context = context;
            _messageOutputHandlingService = messageOutputHandlingService;
            _persistedLoginService = persistedLoginService;
            if (persistedLoginService.loggedIn())
            {
                persistedLoginService.logout();
            }
            else if (isRememberMeEnabled())
            {
                Task task = ExecuteRememberMe();
            }

        }
        /// <summary> Logs in the user who AutoToken is stored and routes to ExpensesPage </summary>
        /// <returns> Returns if an AuthToken exists, which indicates RememberMe preference is enabled</returns>
        private bool isRememberMeEnabled()
        {
            return Preferences.Default.ContainsKey("AuthToken");
        }

        /// <summary> Logs in the user who AutoToken is stored and routes to ExpensesPage </summary>
        [RelayCommand]
        private async Task ExecuteRememberMe()
        {
            var dbContext = await _context.CreateDbContextAsync();
            string storedToken = Preferences.Default.Get("AuthToken", "Unknown");
            User user = null;
            try
            {
                using (dbContext)
                {
                    user = dbContext.Users.FirstOrDefault(u => u.AuthToken == storedToken);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            _persistedLoginService.login(user);
            await Shell.Current.GoToAsync(nameof(DashboardPage));
        }

        /// <summary> Routes to login page </summary>
        [RelayCommand]
        private async Task RouteToLoginPage()
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        /// <summary> Routes to registration page </summary>
        [RelayCommand]
        private async Task RouteToRegistrationPage()
        {
            await Shell.Current.GoToAsync(nameof(RegistrationPage));
        }
    }
}
