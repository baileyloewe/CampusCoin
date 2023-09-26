using CampusCoin.ViewModels;

namespace CampusCoin.Views;

public partial class GraphTestPage : ContentPage
{
	public GraphTestPage(GraphTestPageViewModel vm)
	{
		BindingContext = vm; 
		InitializeComponent();
	}
}