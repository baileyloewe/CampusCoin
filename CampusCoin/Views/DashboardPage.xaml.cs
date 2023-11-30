using CampusCoin.ViewModels;

namespace CampusCoin.Views;

public partial class DashboardPage : ContentPage
{
	public DashboardPage(DashboardTestPageViewModel vm)
	{
		BindingContext = vm; 
		InitializeComponent();
	}
}