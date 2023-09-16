using ShellMixedSample.ViewModels;

namespace ShellMixedSample;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage(MainPageViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}

