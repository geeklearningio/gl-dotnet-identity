

namespace GeekLearning.Authentication.OAuth.Server
{
    using System.Security.Claims;
    using Microsoft.Extensions.Primitives;

    public interface ITokenProvider
    {
        string GenerateAuthorizationToken(AuthorizationRequest request, ClaimsPrincipal identity);
        ITokenValidationResult ValidateAuthorizationCode(string code);
        string GenerateAccessToken(ClaimsIdentity identity);
    }
}
