using CampusCoin.ViewModels;

namespace CampusCoin.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}

    private void LoginBtn_Pressed(object sender, EventArgs e)
    {

    }
}

