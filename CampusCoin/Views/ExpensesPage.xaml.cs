using CampusCoin.ViewModels;

namespace CampusCoin.Views;

public partial class ExpensesPage : ContentPage
{
	public ExpensesPage(ExpensesPageViewModel vm)
	{
		InitializeComponent();
		BindingContext= vm;
    }
}