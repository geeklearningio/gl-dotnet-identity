namespace GeekLearning.Authentication.OAuth.Server
{
    using Microsoft.Extensions.Options;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    public class DefaultTokenProvider : ITokenProvider
    {
        private IOptions<OAuthServerOptions> options;

        public DefaultTokenProvider(IOptions<OAuthServerOptions> options)
        {
            this.options = options;
        }

        public string GenerateAuthorizationToken(AuthorizationRequest request, ClaimsPrincipal identity)
        {
            if (request.ResponseType != "code")
            {
                throw new System.Security.SecurityException("Oauth request ResponseType MUST be \"code\"");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var signingKey = this.options.Value.Keys.First();
            var token = tokenHandler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Audience = this.options.Value.Issuer,
                Expires = DateTimeOffset.UtcNow.AddMinutes(5).DateTime,
                NotBefore = DateTimeOffset.UtcNow.AddMinutes(-1).DateTime,
                IssuedAt = DateTimeOffset.UtcNow.DateTime,
                Issuer = this.options.Value.Issuer,
                SigningCredentials = signingKey.Value,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("https://claims.schemas.geeklearning.io/oauth2/keyid", signingKey.Key),
                    identity.FindFirst(ClaimTypes.NameIdentifier)
                })
            });

            return tokenHandler.WriteToken(token);
        }
    }
}
