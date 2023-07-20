using System.Security.Claims;

namespace ITPortal.Lib.Services.Authentication;

public class AuthenticatedUser
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

    public Claim? GetClaimWithType(string claimType) =>
        Principal.FindFirst(c => c.Type == claimType);

    public bool UpdateClaimType(string oldClaimType, string newClaimType)
    {
        Claim? name = GetClaimWithType(oldClaimType);

        if (name != null)
        {
            // Must recreate principal when adding a new claim
            List<Claim> claims = Principal.Claims.ToList();

            claims.Add(new Claim(newClaimType, name.Value));
            claims.Remove(name);

            Initialize(claims);

            return true;
        }
        return false;
    }
}