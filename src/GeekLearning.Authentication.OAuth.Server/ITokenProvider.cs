

namespace GeekLearning.Authentication.OAuth.Server
{
    using System.Security.Claims;
    using Microsoft.Extensions.Primitives;
    using System.Collections.Generic;
    public interface ITokenProvider
    {
        IToken GenerateAuthorizationToken(AuthorizationRequest request, ClaimsPrincipal identity);
        ITokenValidationResult ValidateAuthorizationCode(string code);
        IToken GenerateAccessToken(ClaimsIdentity identity, IEnumerable<string> audiences);
        ITokenValidationResult ValidateRefreshToken(string refreshToken);
        IToken GenerateRefreshToken(ClaimsIdentity identity, IEnumerable<string> audiences);
    }
}
