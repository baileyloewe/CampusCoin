using CampusCoin.Views;

namespace CampusCoin;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
        Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        Routing.RegisterRoute(nameof(ExpensesPage), typeof(ExpensesPage));
        Routing.RegisterRoute(nameof(ExpensesTestPage), typeof(ExpensesTestPage));
        Routing.RegisterRoute(nameof(EditUserAccountInfoPage), typeof(EditUserAccountInfoPage));
        Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));
        Routing.RegisterRoute(nameof(ExpenseReportPage), typeof(ExpenseReportPage));
    }
}
