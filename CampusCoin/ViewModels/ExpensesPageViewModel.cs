using CampusCoin.Services;
using CampusCoin.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusCoin.ViewModels
{
    public partial class ExpensesPageViewModel : ObservableValidator
    {
        private readonly IMessageOutputHandlingService _messageOutputHandlingService;
        public ExpensesPageViewModel(IMessageOutputHandlingService messageOutputHandlingService)
        {
            _messageOutputHandlingService = messageOutputHandlingService;
        }

        [RelayCommand]
        async Task RouteExpenseReportPage()
        {
            await Shell.Current.GoToAsync(nameof(ExpenseReportPage));
        }
    }
}
