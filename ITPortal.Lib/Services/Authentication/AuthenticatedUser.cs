using System.Security.Claims;

namespace ITPortal.Lib.Services.Authentication;

public sealed class AuthenticatedUser
{
    public ClaimsPrincipal Principal { get; set; }

    public AuthenticatedUser(ClaimsPrincipal principal)
    {
        Principal = principal;
    }

    // Non-empty claims identity indicates logged in user
    public AuthenticatedUser(IEnumerable<Claim> claims) : this()
    {
        Initialize(claims);
    }

    // Empty claims identity indicates logged out user
    public AuthenticatedUser()
    {
        Principal = new ClaimsPrincipal(new ClaimsIdentity());
    }

    private void Initialize(IEnumerable<Claim> claims)
    {
        Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));
    }

    public Claim? GetClaimWithType(string claimType) => Principal.FindFirst(c => c.Type == claimType);

    public bool ReplaceClaimType(List<Claim> claims, string oldClaimType, string newClaimType)
    {
        Claim? old = GetClaimWithType(oldClaimType);

        if (old == null)
        {
            return false;
        }
        claims.Add(new Claim(newClaimType, old.Value));
        claims.Remove(old);

        return true;
    }

    public bool ReplaceClaimTypes(Dictionary<string, string> claimTypeReplacements)
    {
        bool success = true;

        // If adding a new claim to the principal, the principal must be completely rebuilt
        List<Claim> claims = Principal.Claims.ToList();

        foreach ((string oldClaimType, string newClaimType) in claimTypeReplacements)
        {
            if (!ReplaceClaimType(claims, oldClaimType, newClaimType))
            {
                success = false;
            }
        }
        Initialize(claims);

        return success;
    }
}
