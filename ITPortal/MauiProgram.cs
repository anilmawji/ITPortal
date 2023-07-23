using ITPortal.Data;
using ITPortal.Lib.Services;
using ITPortal.Lib.Services.Authentication;
using ITPortal.Lib.Services.Automation;
using ITPortal.Lib.Services.Core;
using Microsoft.Extensions.Configuration;

namespace ITPortal;

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
			});

		builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        builder.Configuration.AddConfiguration(configuration);

        ConfigureServices(builder.Services);

        return builder.Build();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddMsalAuthentication();

        services.AddSingleton<IOutputStreamService<PSMessage, PSStream>, PowerShellOutputStreamService>();

        services.AddSingleton<IGraphClientService, GraphClientService>();

        services.AddHttpClient<HttpClientService>("GitHubApi");
        services.AddSingleton<IGitHubService, GitHubService>();

        services.AddSingleton<WeatherForecastService>();
    }
}
