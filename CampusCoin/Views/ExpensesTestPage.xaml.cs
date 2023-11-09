using CampusCoin.ViewModels;

namespace CampusCoin.Views;

public partial class ExpensesTestPage : ContentPage
{
	public ExpensesTestPage(ExpensesTestPageViewModel vm)
	{
		InitializeComponent();
		BindingContext= vm;
	}
}