namespace GeekLearning.Authentication.OAuth.Server.Mvc
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AuthorizationCodeResult : RedirectResult
    {
        private AuthorizationRequest authorizationRequest;

        public AuthorizationCodeResult(AuthorizationRequest request) : base(request.Redirect_Uri)
        {
            this.authorizationRequest = request;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var tokenProvider = context.HttpContext.RequestServices.GetRequiredService<ITokenProvider>();

            var redirectUri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(authorizationRequest.Redirect_Uri,
                new Dictionary<string, string>()
                {
                    ["code"] = tokenProvider.GenerateAuthorizationToken(authorizationRequest, context.HttpContext.User).Token,
                    ["state"] = authorizationRequest.State,
                });

            this.Url = redirectUri;

            return base.ExecuteResultAsync(context);
        }
    }
}
