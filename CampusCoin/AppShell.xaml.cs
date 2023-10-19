using CampusCoin.Views;

namespace CampusCoin;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
        Routing.RegisterRoute(nameof(GraphTestPage), typeof(GraphTestPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }
}
