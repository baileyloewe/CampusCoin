using CampusCoin.Services;
using CampusCoin.Views;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusCoin.ViewModels;

public partial class ExpensesPageViewModel : ObservableValidator
{
    [ObservableProperty]
    string category;

    [ObservableProperty]
    string amount;

    [ObservableProperty]
    DateTime dateEntered;

    [ObservableProperty]
    string description;

    [ObservableProperty]
    string selectedCategory;

    private readonly IMessageOutputHandlingService _messageOutputHandlingService;
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
            await Shell.Current.DisplayAlert("Success",
                $"Entry Submitted Successfully", "OK");
            await Shell.Current.GoToAsync(nameof(ExpensesPage));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Something went wrong: {ex.InnerException.Message}", "OK");
        }
    }

    public UserData setUserDataValues(UserData userData)
    {
        User user = persistedLoginService.getLoggedInUser();
        DateTime date = DateTime.Now;

        userData.Category = SelectedCategory;
        userData.Amount = int.Parse(Amount);
        userData.DateEntered = date;
        userData.Description = Description;
        userData.UserId = user.UserId;
        
        return userData;
    }

    public string IsBillsCategory
    {
        get { return "bills"; }
        set { SelectedCategory = "Bills"; }
    }

    public string IsFoodCategory
    {
        get { return "food"; }
        set => SelectedCategory = "Food";
    }

    public string IsAutoCategory
    {
        get { return "auto"; }
        set { SelectedCategory = "Auto"; }
    }

    public string IsEntertainmentCategory
    {
        get { return "entertainment"; }
        set { SelectedCategory = "Entertainment"; }
    }

    public string IsInvestmentsCategory
    {
        get { return "investments"; }
        set { SelectedCategory = "Investments"; }
    }

    public string IsMiscCategory
    {
        get { return "misc"; }
        set { SelectedCategory = "Misc"; }
    }

    [RelayCommand]
    async Task RouteDashboardPage()
    {
        await Shell.Current.GoToAsync(nameof(DashboardPage));
    }
}
