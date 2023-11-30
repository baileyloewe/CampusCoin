using CampusCoin.ViewModels;

namespace CampusCoin.Views;

public partial class EditUserAccountInfoPage : ContentPage
{
	public EditUserAccountInfoPage(EditUserAccountInfoPageViewModel vm)
	{
        InitializeComponent();
        BindingContext = vm;
    }
}