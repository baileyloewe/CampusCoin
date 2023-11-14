using CampusCoin.Services;
using CampusCoin.Views;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusCoin.ViewModels;

public partial class ExpensesPageViewModel : ObservableValidator
{
    private readonly IMessageOutputHandlingService _messageOutputHandlingService;

    [ObservableProperty]
    string category;

    [ObservableProperty]
    string amount;

    [ObservableProperty]
    string dateEntered;

    [ObservableProperty]
    string description;

    ExpensesService expensesService;
    PersistedLoginService persistedLoginService;

    public ExpensesPageViewModel(ExpensesService expensesService, PersistedLoginService persistedLoginService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.expensesService = expensesService;
        this.persistedLoginService = persistedLoginService;
        _messageOutputHandlingService = messageOutputHandlingService;
    }

    [RelayCommand]
    async Task SubmitExpenseToDatabase()
    {
        try
        {
            var userData = new UserData();
            userData =  setUserDataValues(userData);
            await expensesService.SubmitExpense(userData);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Something went wrong: {ex.InnerException.Message}", "OK");
        }
    }

    public UserData setUserDataValues(UserData userData)
    {
        User user = persistedLoginService.GetUser();
        DateTime date = DateTime.Now;

        userData.Category = "Bills";
        userData.Amount = "50";
        userData.DateEntered = date.ToString("MM,dd,yyyy HH,mm,ss");
        userData.Description = "Test";
        userData.UserId = user.UserId;
        
        return userData;
    }

    [RelayCommand]
    async Task RouteExpenseReportPage()
    {
        await Shell.Current.GoToAsync(nameof(ExpenseReportPage));
    }
}
