namespace MongoDbIdentityServer
{
    using System;
    using System.Linq;
    using Interface;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Events;

    public class Program
    {
        internal static string HostingEnvironment =>
            Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Process) ??
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Process);

        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer";
            var serilogConfiguration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{HostingEnvironment}.json", true, true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .ReadFrom.Configuration(serilogConfiguration)
                .CreateLogger();

            var host = CreateWebHostBuilder(args);

            var seed = Array.IndexOf(args, "/seed") > -1; 
            if (seed)
            {
                args = args.Except(new[] { "/seed" }).ToArray();
                IRepository repository = (IRepository) host.Services.GetService(typeof(IRepository));
                var seeder = new SeedData(repository);
                seeder.Seed();
            }

            host.Run();
        }

        public static IWebHost CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", false, true);
                    config.AddJsonFile($"appsettings.{HostingEnvironment}.json", true, true);
                    config.AddJsonFile("secrets.json", true, true);
                    config.AddJsonFile($"secrets.{HostingEnvironment}.json", true, true);
                })
                .UseStartup<Startup>()
                .UseSerilog()
                .UseKestrel()
                .Build();
        }
    }
}