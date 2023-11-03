using System.Diagnostics;
using CampusCoin.Services;
using CampusCoin.ViewModels;
using CampusCoin.Views;
using Microsoft.EntityFrameworkCore;
using CampusCoin.Models;
using SkiaSharp.Views.Maui.Controls.Hosting;


namespace CampusCoin;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        try
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa_solid.ttf", "FontAwesome");
                })
                .RegisterDatabase()
                .RegisterServices()
                .RegisterViewModels()
                .RegisterViews();

            return builder.Build();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            throw;
        }
     
    }
    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<MainPageViewModel>();

        mauiAppBuilder.Services.AddTransient<LoginPageViewModel>();

        mauiAppBuilder.Services.AddTransient<RegistrationPageViewModel>();

        mauiAppBuilder.Services.AddSingleton<GraphTestPageViewModel>();


        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder
            .Services
            .AddSingleton<MainPage>();

        mauiAppBuilder.Services.AddTransient<LoginPage>();
        mauiAppBuilder.Services.AddTransient<RegistrationPage>();

        mauiAppBuilder
            .Services
            .AddSingleton<GraphTestPage>();


        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder
            .Services
            .AddSingleton<IMessageOutputHandlingService, MessageOutputHandlingService>();

        mauiAppBuilder.Services.AddSingleton<LoginService>();
        mauiAppBuilder.Services.AddSingleton<RegistrationService>();
        mauiAppBuilder.Services.AddSingleton<EmailService>();



        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterDatabase(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder
            .Services
            .AddDbContextFactory<CampusCoinContext>(options =>
            {
                //todo: replace with environment variable or config file rather than hard coded connection string
                options.UseSqlServer("Server=tcp:136.55.236.20,1438;Initial Catalog=SeniorProjectDatabase;Persist Security Info=False;User ID=Remote1;Password=SeniorProject;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;TrustServerCertificate=True;");
            });

        return mauiAppBuilder;
    }   
}
