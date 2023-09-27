using ITPortal.Lib.Services;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using CommunityToolkit.Maui;

namespace ITPortal;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
             })
            .UseMauiCommunityToolkit();

        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        builder.Configuration.AddConfiguration(configuration);

        ConfigureServices(builder.Services, configuration);

        return builder.Build();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMudServices();
        services.AddMsalAuthenticationServices(configuration);
        services.AddScriptJobServices(configuration, FileSystem.Current.AppDataDirectory);
        services.AddScoped<IGraphClientService, GraphClientService>();
        services.AddGitHubServices();
    }
}