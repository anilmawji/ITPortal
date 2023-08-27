using ITPortal.Data;
using ITPortal.Lib.Services;
using ITPortal.Lib.Services.Authentication;
using ITPortal.Lib.Services.Core;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using ITPortal.Lib.Services.Automation.Job;

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
        services.AddMudServices();
        services.AddMsalAuthentication();
        services.AddSingleton<IScriptJobService, ScriptJobService>();
        services.AddGitHubServices();
        services.AddSingleton<IGraphClientService, GraphClientService>();

        services.AddSingleton<WeatherForecastService>();
    }
}
