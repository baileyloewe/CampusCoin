using CampusCoin.ViewModels;

namespace CampusCoin.Views;

public partial class RegistrationPage : ContentPage
{
    public RegistrationPage(RegistrationPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}