using CampusCoin.ViewModels;

namespace CampusCoin.Views;

public partial class ResetPasswordPage : ContentPage
{
	public ResetPasswordPage(ResetPasswordPageViewModel vm)
	{
        InitializeComponent();
        BindingContext = vm;
    }
}