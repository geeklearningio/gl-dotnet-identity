

namespace GeekLearning.Authentication.OAuth.Server
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using Microsoft.Extensions.DependencyInjection;
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
            var clientProvider = context.RequestServices.GetRequiredService<IClientProvider>();
            var tokenProvider = context.RequestServices.GetRequiredService<ITokenProvider>();

            var request = new TokenRequest
            {
                Grant_Type = context.Request.Form["grant_type"],
                Code = context.Request.Form["code"],
                Redirect_Uri = context.Request.Form["redirect_uri"],
                Client_Id = context.Request.Form["client_id"],
                Client_Secret = context.Request.Form["client_secret"],
            };

            var result = tokenProvider.ValidateAuthorizationCode(request.Code);

            if (result.Success)
            {
                var clientValidation = await clientProvider.ValidateCredentialsAsync(result.NameIdentifier, request.Client_Id, request.Client_Secret);
                if (clientValidation.Success)
                {
                    var token = tokenProvider.GenerateAccessToken(clientValidation.Identity);
                    var responseContent = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        access_token = token,
                        token_type = "access_token",
                        expires_in = this.options.Value.AccesssTokenLifetime.TotalSeconds,
                    });

                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(responseContent);
                    return;
                }
                else
                {
                    this.Forbid(context);
                    return;
                }
            }
            else
            {
                this.Forbid(context);
                return;
            }
        }

        private async Task ProcessAuthorizationRequest(HttpContext context)
        {
            var request = new AuthorizationRequest
            {
                Client_Id = context.Request.Query["client_id"],
                Response_Type = context.Request.Query["response_type"],
                Redirect_Uri = context.Request.Query["redirect_uri"],
                Scope = context.Request.Query["scope"],
                State = context.Request.Query["state"],
            };

            if (request.Response_Type != "code")
            {
                BadRequest(context);
                return;
            }

            this.logger.LogInformation($"OAuth authentication request {request.Client_Id}: {request.State}");

            var redirectUri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(options.Value.LoginPath, new Dictionary<string, string>()
            {
                ["protocol"] = "oauth2",
                ["state"] = request.State,
                ["scope"] = request.Scope,
                ["redirect_uri"] = request.Redirect_Uri,
                ["response_type"] = request.Response_Type,
                ["client_id"] = request.Client_Id
            });

            context.Response.Redirect(redirectUri);
        }

        private void BadRequest(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private void Forbid(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }
    }
}
