using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Authentication.OAuth.Server
{
    public class OAuthServerOptions
    {
        public string TokenEndpointPath { get; set; } = "/.well-known/oauth2/token";

        public string AuthorizationEndpointPath { get; set; } = "/.well-known/oauth2/authorization";

        public string LoginPath { get; set; } = "/Account/Login";

        public Dictionary<string, SigningCredentials> Keys { get; set; } = new Dictionary<string, SigningCredentials>();

        public string Issuer { get; set; }

        public TimeSpan AccesssTokenLifetime { get; set; } = TimeSpan.FromHours(24);
        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(365);
        public TimeSpan AuthorizationCodeLifetime { get; set; } = TimeSpan.FromMinutes(5);
    }
}
