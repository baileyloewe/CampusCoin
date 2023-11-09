using CampusCoin.Services;
using CampusCoin.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusCoin.ViewModels
{
    public partial class ExpensesTestPageViewModel : ObservableValidator
    {
        private readonly IMessageOutputHandlingService _messageOutputHandlingService;
        public ExpensesTestPageViewModel(IMessageOutputHandlingService messageOutputHandlingService) 
        {
            _messageOutputHandlingService = messageOutputHandlingService;
        }

        [RelayCommand]
        async Task RouteExpenses()
        {
            await Shell.Current.GoToAsync(nameof(ExpensesPage));
        }
    }
}
