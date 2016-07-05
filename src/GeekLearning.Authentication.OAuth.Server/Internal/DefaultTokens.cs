namespace GeekLearning.Authentication.OAuth.Server.Internal
{
    using Microsoft.IdentityModel.Tokens;
    using System;

    public static class DefaultTokensHelper
    {
        public static IToken GetAccessToken(this SecurityTokenHandler handler, string id, SecurityToken token)
        {
            return new DefaultToken(id, handler.WriteToken(token), new DateTimeOffset(token.ValidFrom), new DateTimeOffset(token.ValidFrom), "access_token");
        }

        public static IToken GetAuthorizationCode(this SecurityTokenHandler handler, string id, SecurityToken token)
        {
            return new DefaultToken(id, handler.WriteToken(token), new DateTimeOffset(token.ValidFrom), new DateTimeOffset(token.ValidFrom), "authorization_code");
        }

        public static IToken GetRefreshToken(this SecurityTokenHandler handler, string id, SecurityToken token)
        {
            return new DefaultToken(id, handler.WriteToken(token), new DateTimeOffset(token.ValidFrom), new DateTimeOffset(token.ValidFrom), "refresh_token");
        }
    }

    public class DefaultToken : IToken
    {
        public DefaultToken(string id,
            string token,
            DateTimeOffset fromDate,
            DateTimeOffset toDate,
            string type)
        {
            this.Id = id;
            this.Token = token;
            this.FromDate = fromDate;
            this.ToDate = toDate;
            this.Type = type;
        }

        public DateTimeOffset FromDate { get; }

        public DateTimeOffset ToDate { get; }

        public string Id { get; }

        public string Token { get; }

        public string Type { get; }
    }
}
