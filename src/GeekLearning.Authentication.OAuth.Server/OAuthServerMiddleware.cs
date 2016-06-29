

namespace GeekLearning.Authentication.OAuth.Server
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;    // This project can output the Class library as a NuGet Package.
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

        private string SafeFormRead(HttpContext context, string key)
        {
            Microsoft.Extensions.Primitives.StringValues values;
            if (context.Request.Form.TryGetValue(key, out values))
            {
                return values;
            }
            return null;
        }

        private async Task ProcessTokenRequest(HttpContext context)
        {
            var clientProvider = context.RequestServices.GetRequiredService<IClientProvider>();
            var tokenProvider = context.RequestServices.GetRequiredService<ITokenProvider>();

            var request = new TokenRequest
            {
                Grant_Type = SafeFormRead(context, "grant_type"),
                Code = SafeFormRead(context, "code"),
                Refresh_Token = SafeFormRead(context, "refresh_token"),
                Redirect_Uri = SafeFormRead(context, "redirect_uri"),
                Client_Id = SafeFormRead(context, "client_id"),
                Client_Secret = SafeFormRead(context, "client_secret"),
            };

            ITokenValidationResult grantValidation = null;
            IClientValidationResult clientValidation = null;
            bool includeRefreshToken = false;

            if (request.Grant_Type == "authorization_code")
            {
                grantValidation = tokenProvider.ValidateAuthorizationCode(request.Code);
                if (grantValidation.Success)
                {
                    clientValidation = await clientProvider.ValidateCredentialsAsync(grantValidation.NameIdentifier, request.Client_Id, request.Client_Secret);
                    includeRefreshToken = true;
                }
            }
            else if (request.Grant_Type == "refresh_token")
            {
                grantValidation = tokenProvider.ValidateRefreshToken(request.Refresh_Token);
                if (grantValidation.Success)
                {
                    clientValidation = await clientProvider.ValidateTokenAsync(grantValidation.NameIdentifier, request.Client_Id);
                }
            }

            var result = tokenProvider.ValidateAuthorizationCode(request.Code);

            if (grantValidation != null && grantValidation.Success)
            {
                if (clientValidation.Success)
                {
                    IToken token = tokenProvider.GenerateAccessToken(clientValidation.Identity, (await clientProvider.GetClientAudiences(request.Client_Id)).Concat(new[] { options.Value.Issuer }));
                    IToken refreshToken = null;
                    if (includeRefreshToken)
                    {
                        refreshToken = tokenProvider.GenerateRefreshToken(clientValidation.Identity, new[] { options.Value.Issuer });
                        await clientProvider.OnTokenEmitted(request.Client_Id, result.NameIdentifier, new IToken[] { token, refreshToken });
                    }
                    else
                    {
                        await clientProvider.OnTokenEmitted(request.Client_Id, result.NameIdentifier, new IToken[] { token });
                    }

                    var responseContent = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        refresh_token = refreshToken?.Token,
                        access_token = token?.Token,
                        token_type = "bearer",
                        expires_in = this.options.Value.AccesssTokenLifetime.TotalSeconds,
                    });

                    context.Response.Headers.Add("Cache-Control", "no-store");
                    context.Response.Headers.Add("Pragma", "no-cache");
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
