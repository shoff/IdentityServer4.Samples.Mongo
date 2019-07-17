namespace QuickstartIdentityServer
{
    using System;
    using System.Linq;
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

            var seed = Array.IndexOf(args, "/seed") > -1; 
            if (seed)
            {
                args = args.Except(new[] { "/seed" }).ToArray();
                // TODO seed mongodb
            }

            CreateWebHostBuilder(args).Run();
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
                .UseIISIntegration()
                .Build();
        }
    }
}