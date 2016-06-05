using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace GeekLearning.Authentication.OAuth.Server
{
    public static class GeekLearningAuthenticationOAuthServerExtensions
    {
        public static IServiceCollection AddOAuthServer<TClientProvider>(this IServiceCollection services)
            where TClientProvider : class, IClientProvider
        {
            services.AddScoped<IClientProvider, TClientProvider>();
            services.AddTransient<ITokenProvider, DefaultTokenProvider>();

            return services;
        }

        public static IApplicationBuilder UseOAuthServer(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<GeekLearning.Authentication.OAuth.Server.OAuthServerMiddleware>();
            return appBuilder;
        }
    }
}
