using ITPortal.Lib.Services;
using ITPortal.Lib.Services.Authentication;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

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

        ConfigureServices(builder.Services);

        return builder.Build();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddMudServices();
        services.AddMsalAuthenticationServices();
        services.AddScriptJobServices(FileSystem.Current.AppDataDirectory);
        services.AddScoped<IGraphClientService, GraphClientService>();
        services.AddGitHubServices();
    }
}