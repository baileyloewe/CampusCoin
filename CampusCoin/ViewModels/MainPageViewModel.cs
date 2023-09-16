using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampusCoin.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ShellMixedSample.Models;

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
            // initialize count and counter text
            Count = 0;
            CounterText = GetCounterText();
        }

        // Observable property for count with validation attributes - https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/observableproperty
        [ObservableProperty]
        [Required(ErrorMessage = "Count is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Integer must be a positive number and less than max integer value.")]
        private int _count;

        [ObservableProperty]
        private string _counterText;

        /// <summary>
        /// Increases the count by 1 and updates the counter text
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task IncrementCount()
        {
            Count++;
            ValidateAllProperties(); // validating all properties

            // Display validation errors if they exist, otherwise update counter text
            if (HasErrors)
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
            else
            {
                CounterText = GetCounterText();
            }

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
        private string GetCounterText()
        {
            return Count switch
            {
                0 => "Click me!",
                1 => "Clicked 1 time",
                _ => $"Clicked {Count} times"
            };
        }
    }
}
