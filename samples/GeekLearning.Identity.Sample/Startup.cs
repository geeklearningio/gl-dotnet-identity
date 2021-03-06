﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GeekLearning.Identity.Sample.Data;
using GeekLearning.Identity.Sample.Models;
using GeekLearning.Identity.Sample.Services;
using GeekLearning.Authentication.OAuth.Server;
using Microsoft.IdentityModel.Tokens;
using GeekLearning.Security.Cryptography;

namespace GeekLearning.Identity.Sample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddOAuthServer<Services.SampleClientProvider>();

            var keyId = Configuration["OAuthServer:DefaultKey:Id"];
            var alg = Configuration["OAuthServer:DefaultKey:Alg"];
            SecurityKey securityKey = null;
            SigningCredentials signingCredentials = null;
            if (alg == "rsa")
            {
                var rsaParams = RsaKeyHelper.ReadParameters(Configuration["OAuthServer:DefaultKey:PrivateKey"], Configuration["OAuthServer:DefaultKey:PrivateKeyPassword"]);
                securityKey = new RsaSecurityKey(rsaParams);
                signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature);
            }
            else if (alg == "ecdsa")
            {
                var cngKey = ECDSAKeyHelper.ReadParameters(Configuration["OAuthServer:DefaultKey:PrivateKey"], Configuration["OAuthServer:DefaultKey:PrivateKeyPassword"]);
                securityKey = new ECDsaSecurityKey(new System.Security.Cryptography.ECDsaCng(cngKey));
                signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256Signature);
            }

            services.Configure<OAuthServerOptions>(options =>
            {
                options.Issuer = Configuration["OAuthServer:Issuer"];
                options.Keys.Add(keyId, signingCredentials);
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseOAuthServer();

            app.UseStaticFiles();

            app.UseIdentity();
            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
