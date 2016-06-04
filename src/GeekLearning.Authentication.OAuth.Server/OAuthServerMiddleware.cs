

namespace GeekLearning.Authentication.OAuth.Server
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class OAuthServerMiddleware
    {
        public OAuthServerMiddleware(RequestDelegate next, IOptions<OAuthServerOptions> options, ILogger<OAuthServerMiddleware> logger)
        {
            this.next = next;
            this.options = options;
            this.logger = logger;
        }

        private readonly RequestDelegate next;
        private readonly IOptions<OAuthServerOptions> options;
        private ILogger<OAuthServerMiddleware> logger;

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == options.Value.TokenEndpointPath)
            {
                await ProcessTokenRequest(context);
            }
            else if (context.Request.Path == options.Value.AuthorizationEndpointPath)
            {
                await ProcessAuthorizationRequest(context);
            }
            else
            {
                await next.Invoke(context);
            }
        }

        private async Task ProcessTokenRequest(HttpContext context)
        {

            await next.Invoke(context);
        }

        private async Task ProcessAuthorizationRequest(HttpContext context)
        {
            var request = new AuthorizationRequest
            {
                ClientId = context.Request.Query["client_id"],
                ResponseType = context.Request.Query["response_type"],
                RedirectUri = context.Request.Query["redirect_uri"],
                Scope = context.Request.Query["scope"],
                State = context.Request.Query["state"],
            };

            this.logger.LogInformation($"OAuth authentication request {request.ClientId}: {request.State}");

            //var redirectUri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(request.RedirectUri, new Dictionary<string, string>
            //{
            //    ["code"] = "somecode",
            //    ["state"] = request.State
            //});


            var redirectUri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(options.Value.LoginPath, new Dictionary<string, string>()
            {
                ["protocol"] = "oauth2",
                ["state"] = request.State,
            });

            context.Response.Redirect(redirectUri);

            //await next.Invoke(context);
        }
    }
}
