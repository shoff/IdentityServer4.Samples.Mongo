namespace MongoDbIdentityServer.Configuration
{
    using Extension;
    using IdentityServer4;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    internal static class Ioc
    {
        internal static IServiceCollection InitializeContainer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging()
                .AddOptions()
                .AddHttpClient()
                .Configure<MongoOptions>(configuration.GetSection("MongoOptions"))
                .Configure<GoogleOptions>(configuration.GetSection("GoogleOptions"))
                .AddCors()
                .InitializeDependencies()
                .InitializeIdentityServer()
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            return services;
        }


        private static IServiceCollection InitializeDependencies(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection InitializeIdentityServer(this IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddMongoRepository()
                .AddClients()
                .AddIdentityApiResources()
                .AddPersistedGrants()
                .AddTestUsers(Config.GetUsers());

            var sp = services.BuildServiceProvider();
            var google = sp.GetRequiredService<IOptions<GoogleOptions>>();

            services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = google.Value.ClientId;
                    options.ClientSecret = google.Value.ClientSecret;
                })
                .AddOpenIdConnect(Constants.OIDC, Constants.OPEN_ID_CONNECT, options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                    options.Authority = Constants.AUTHORITY;
                    options.ClientId = Constants.IMPLICIT;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = Constants.NAME,
                        RoleClaimType = Constants.ROLE
                    };
                });

            return services;
        }
    }
}