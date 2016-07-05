namespace GeekLearning.Authentication.OAuth.Server
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;

    public class OAuthServerOptions
    {
        public PathString TokenEndpointPath { get; set; } = "/.well-known/oauth2/token";

        public PathString AuthorizationEndpointPath { get; set; } = "/.well-known/oauth2/authorization";

        public PathString LoginPath { get; set; } = "/Account/Login";

        public Dictionary<string, SigningCredentials> Keys { get; set; } = new Dictionary<string, SigningCredentials>();

        public string Issuer { get; set; }

        public TimeSpan AccesssTokenLifetime { get; set; } = TimeSpan.FromHours(24);

        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(365);

        public TimeSpan AuthorizationCodeLifetime { get; set; } = TimeSpan.FromMinutes(5);
    }
}
