namespace ITPortal.Lib.Services;

public class AzureAdSettings
{
    public string? ClientId { get; set; }
    public string? TenantId { get; set; }
    public string[]? Scopes { get; set; }
}
