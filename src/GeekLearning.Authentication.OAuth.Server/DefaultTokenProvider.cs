namespace GeekLearning.Authentication.OAuth.Server
{
    using Microsoft.Extensions.Options;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.Extensions.Primitives;
    using Microsoft.IdentityModel.Tokens;
    public class DefaultTokenProvider : ITokenProvider
    {
        private IOptions<OAuthServerOptions> options;

        public DefaultTokenProvider(IOptions<OAuthServerOptions> options)
        {
            this.options = options;
        }

        public string GenerateAccessToken(ClaimsIdentity identity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var signingKey = this.options.Value.Keys.First();
            var token = tokenHandler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Audience = this.options.Value.Issuer,
                Expires = (DateTime.Now + this.options.Value.AccesssTokenLifetime),
                NotBefore = DateTime.Now.AddMinutes(-1),
                IssuedAt = DateTime.Now,
                //Expires = (DateTimeOffset.UtcNow + this.options.Value.AccesssTokenLifetime).DateTime,
                //NotBefore = DateTimeOffset.UtcNow.AddMinutes(-1).DateTime,
                //IssuedAt = DateTimeOffset.UtcNow.DateTime,
                Issuer = this.options.Value.Issuer,
                SigningCredentials = signingKey.Value,
                Subject = identity
            });

            return tokenHandler.WriteToken(token);
        }

        public string GenerateAuthorizationToken(AuthorizationRequest request, ClaimsPrincipal identity)
        {
            if (request.Response_Type != "code")
            {
                throw new System.Security.SecurityException("Oauth request ResponseType MUST be \"code\"");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var signingKey = this.options.Value.Keys.First();
            var token = tokenHandler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Audience = this.options.Value.Issuer,
                Expires = (DateTime.Now + this.options.Value.AuthorizationCodeLifetime),
                NotBefore = DateTime.Now.AddMinutes(-1),
                IssuedAt = DateTime.Now,
                //Expires = (DateTimeOffset.UtcNow + this.options.Value.AuthorizationCodeLifetime).DateTime,
                //NotBefore = DateTimeOffset.UtcNow.AddMinutes(-1).DateTime,
                //IssuedAt = DateTimeOffset.UtcNow.DateTime,
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

        private static bool ValidateLifetime(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (!notBefore.HasValue || !expires.HasValue)
            {
                return false;
            }
            var now = DateTimeOffset.UtcNow;
            return now - validationParameters.ClockSkew < new DateTimeOffset(expires.Value, TimeSpan.Zero)
                && now + validationParameters.ClockSkew > new DateTimeOffset(notBefore.Value, TimeSpan.Zero);
        }


        public ITokenValidationResult ValidateAuthorizationCode(string code)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            Microsoft.IdentityModel.Tokens.SecurityToken token;
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(code, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    //LifetimeValidator = ValidateLifetime,
                    ValidIssuer = this.options.Value.Issuer,
                    ValidAudience = this.options.Value.Issuer,
                    IssuerSigningKeys = this.options.Value.Keys.Values.Select(x => x.Key)
                }, out token);
                return new TokenValidationResult
                {
                    Success = true,
                    NameIdentifier = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value
                };
            }
            catch
            {
                return new TokenValidationResult
                {
                    Success = false
                };
            }

        }

        public class TokenValidationResult : ITokenValidationResult
        {
            public string NameIdentifier { get; set; }

            public bool Success { get; set; }
        }
    }
}
