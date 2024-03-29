using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
   .UseWindowsService()
   .ConfigureAppConfiguration((context, config) =>
   {
       // configure the app here.
   })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));
        services.AddScoped<IReadContentService, ReadContentService>();
        services.AddScoped<IApiConsumerService, ApiConsumerService>();
        services.AddScoped<IFileConstructionService, FileConstructionService>();
    })
   .Build();

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
    Log.Information($"Application Directory: {AppDomain.CurrentDomain.BaseDirectory}");
    await host.RunAsync();
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





