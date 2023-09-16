using ShellMixedSample.ViewModels;

namespace ShellMixedSample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("fa_solid.ttf", "FontAwesome");
			})
			.RegisterServices()
			.RegisterViewModels()
			.RegisterViews();

		return builder.Build();
	}

	/// <summary>
	/// All view models are registered here. Transient means that a new instance of the view model is created every time it is requested. Singleton means that only one instance of the view model is created and it is reused every time it is requested.
	/// Scoped means that a new instance of the view model is created for each scope. For example, if you have a view model that is scoped to a page, then a new instance of the view model is created for each page.
	/// </summary>
	/// <param name="mauiAppBuilder"></param>
	/// <returns></returns>
	public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
	{
        mauiAppBuilder.Services.AddScoped<MainPageViewModel>();

        return mauiAppBuilder;
    }

	public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
	{
        mauiAppBuilder
            .Services
            .AddScoped<MainPage>();

        return mauiAppBuilder;
    }

	public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
	{
        // register any services here
        return mauiAppBuilder;
    }
}
