using CampusCoin.Services;
using CampusCoin.Views;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusCoin.ViewModels;

public partial class ExpenseReportViewModel : ObservableValidator
{
    private readonly IMessageOutputHandlingService _messageOutputHandlingService;
    private readonly ExpensesService expensesService;
    private readonly PersistedLoginService persistedLoginService;

    public ExpenseReportViewModel(ExpensesService expensesService, PersistedLoginService persistedLoginService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.expensesService = expensesService;
        this.persistedLoginService = persistedLoginService;
        _messageOutputHandlingService = messageOutputHandlingService;
    }
}
