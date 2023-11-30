using CampusCoin.ViewModels;

namespace CampusCoin.Views;

public partial class DashboardPage : ContentPage
{
	public DashboardPage(DashboardPageViewModel vm)
	{
		BindingContext = vm; 
		InitializeComponent();
	}
}