using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;

namespace BgServicex
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string loggerTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var logfile = Path.Combine(baseDir, "App_Data", "logs", "log.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.With(new ThreadIdEnricher())
                .Enrich.FromLogContext()
                .WriteTo.Console(LogEventLevel.Information, loggerTemplate, theme: AnsiConsoleTheme.Literate)
                .WriteTo.File(logfile, LogEventLevel.Information, loggerTemplate,
                    rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90)
                .CreateLogger();

            try
            {
                Log.Information("====================================================================");
                Log.Information($"Application Starts. Version: {System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version}");
                Log.Information($"Application Directory: {baseDir}");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Application terminated unexpectedly");
            }
            finally
            {
                Log.Information("====================================================================\r\n");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Configure the app here.
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));
                    services.AddScoped<IServiceA, ServiceA>();
                    services.AddScoped<IServiceB, ServiceB>();
                    services.AddDbContext<ApplicationDataContext>(options =>
                                            options.UseSqlServer(
                                            hostContext.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value));
                    //       services.AddIdentity<IdentityUser, IdentityRole>(cfg =>
                    //       {
                    //           cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                    //           cfg.SignIn.RequireConfirmedEmail = false;
                    //           cfg.User.RequireUniqueEmail = true;
                    //           cfg.Password.RequireDigit = false;
                    //           cfg.Password.RequiredUniqueChars = 0;
                    //           cfg.Password.RequireLowercase = false;
                    //           cfg.Password.RequireNonAlphanumeric = false;
                    //           cfg.Password.RequireUppercase = false;
                    //           cfg.Password.RequiredLength = 6;
                    //       }).AddDefaultTokenProviders()
                    //.AddEntityFrameworkStores<ApplicationDataContext>();
                })
                .UseSerilog();
    }
}
