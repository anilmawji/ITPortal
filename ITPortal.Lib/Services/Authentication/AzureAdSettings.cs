using Microsoft.Extensions.Configuration;

namespace ITPortal.Lib.Services.Authentication;

public class AzureAdSettings
{
    public string? ClientId { get; set; }
    public string? TenantId { get; set; }
    public string[]? Scopes { get; set; }

    public AzureAdSettings(IConfiguration configuration)
    {
        ClientId = configuration["AzureAd:ClientId"];
        TenantId = configuration["AzureAd:TenantId"];
        Scopes = configuration.GetValue<string[]>("AzureAd:Scopes");
    }
}
