using BgServicex.Utils;
using Domain;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            catch (Exception exception)
            {
                Log.Fatal(exception, "Application terminated unexpectedly");
                if (exception.Source.Contains("ActiveDirectory"))
                {
                    Log.Fatal(exception, "ActiveDirectory: Make sure that you have filled out the appsettings.json");

                    Console.Error.WriteLine("TIP: Make sure that you have filled out the appsettings.json file before running this sample.");
                }

                Console.Error.WriteLine($"{exception.Message}");

                if (exception.GetBaseException() is ErrorResponseException apiException)
                {
                    Console.Error.WriteLine(
                        $"ERROR: API call failed with error code '{apiException.Body.Error.Code}' and message '{apiException.Body.Error.Message}'.");
                }
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
                    //services.AddScoped<IServiceA, ServiceA>();
                    //services.AddScoped<IServiceB, ServiceB>();
                   // services.AddScoped<IUploadFileService, UploadFileService>();
                    //  services.AddScoped<IBlobHelper, BlobHelper>();
                    // services.AddScoped<IUserHelper, UserHelper>();
                    services.AddDbContext<ApplicationDataContext>(options =>
                                            options.UseSqlServer(
                                            hostContext.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value));
                    //       services.AddIdentity<ApplicationUser, IdentityRole>(cfg =>
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
