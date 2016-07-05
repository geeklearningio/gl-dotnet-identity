namespace GeekLearning.Authentication.OAuth.Server
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

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
            appBuilder.UseMiddleware<OAuthServerMiddleware>();
            return appBuilder;
        }
    }
}
