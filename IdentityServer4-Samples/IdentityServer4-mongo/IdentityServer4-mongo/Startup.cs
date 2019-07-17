namespace QuickstartIdentityServer
{
    using Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.InitializeContainer(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIdentityServer()
                .UseAuthentication()
                .UseStaticFiles()
                .UseMvcWithDefaultRoute()
                .ConfigureMongoDriver2IgnoreExtraElements()
                .InitializeDatabase();
        }

        public IConfiguration Configuration { get; }
    }
}