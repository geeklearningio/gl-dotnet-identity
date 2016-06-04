

namespace GeekLearning.Authentication.OAuth.Server
{
    using System.Security.Claims;
    public interface ITokenProvider
    {
        string GenerateAuthorizationToken(AuthorizationRequest request, ClaimsPrincipal identity);
    }
}
