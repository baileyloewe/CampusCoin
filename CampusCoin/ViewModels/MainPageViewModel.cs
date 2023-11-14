using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;
using CampusCoin.Models;
using CampusCoin.Services;
using Microsoft.EntityFrameworkCore;
using CampusCoin.Views;

namespace CampusCoin.ViewModels
{
    public partial class MainPageViewModel : ObservableValidator
    {
        private readonly IMessageOutputHandlingService _messageOutputHandlingService;
        private readonly IDbContextFactory<CampusCoinContext> _testContextFactory;

        // note that the IMessageOutputHandlingService is injected into the constructor- https://learn.microsoft.com/en-us/dotnet/architecture/maui/dependency-injection
        // this is necessary in our case for unit tests to be able to mock the service
        public MainPageViewModel(IMessageOutputHandlingService messageOutputHandlingService, IDbContextFactory<CampusCoinContext> testContextFactory)
        {
            // assign injected services to private variables
            _testContextFactory = testContextFactory;
            _messageOutputHandlingService = messageOutputHandlingService;
        }

        /// <summary>
        /// Testing database connection
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task CheckDatabaseConnection()
        {
            try
            {
                using (var context = _testContextFactory.CreateDbContext())
                {
                    var test = context.Users.ToList();
                    await _messageOutputHandlingService.OutputSuccessToUser($"Database connection successful. Found {test.Count} users.");
                }
            }
            catch (Exception ex)
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(new List<ValidationResult> { new ValidationResult($"Database connection failed. {ex.Message}") });
            }
        }

        /// <summary>
        /// Routes to login page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task RouteToLoginPage()
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        /// <summary>
        /// Routes to registration page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task RouteToRegistrationPage()
        {
            await Shell.Current.GoToAsync(nameof(RegistrationPage));
        }
    }
}
