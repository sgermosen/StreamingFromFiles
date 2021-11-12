using BgServicex.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BgServicex
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private FileSystemWatcher _folderWatcher;
        private readonly string _inputFolder;
        private readonly IServiceProvider _services;
        // private readonly ApplicationDataContext _context;

        public Worker(ILogger<Worker> logger, IOptions<AppSettings> settings, IServiceProvider services)//, ApplicationDataContext context)
        {
            _logger = logger;
            _services = services;
            //_context = context;
            _inputFolder = settings.Value.InputFolder;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service Starting");
            if (!Directory.Exists(_inputFolder))
            {
                _logger.LogWarning($"Please make sure the InputFolder [{_inputFolder}] exists, then restart the service.");
                return Task.CompletedTask;
            }

            _logger.LogInformation($"Binding Events from Input Folder: {_inputFolder}");
            _folderWatcher = new FileSystemWatcher(_inputFolder, "*.TXT")
            {
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                  NotifyFilters.DirectoryName | NotifyFilters.Size
            };
            _folderWatcher.Created += Input_OnChanged;
            _folderWatcher.EnableRaisingEvents = true;

            return base.StartAsync(cancellationToken);
        }

        protected async void Input_OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                _logger.LogInformation($"InBound Change Event Triggered by [{e.FullPath}]");

                // do some work
                var eventFile = new EventFile
                {
                    DirectoryName = e.FullPath,
                    FileName = e.Name,
                    //Attributes = ,
                    //CreationTime = ,
                    //Size =
                };

                using (var scope = _services.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDataContext>();
                    _context.EventFiles.Add(eventFile);
                    await _context.SaveChangesAsync();
                }

                using (var scope = _services.CreateScope())
                {
                    var serviceA = scope.ServiceProvider.GetRequiredService<IServiceA>();
                    serviceA.Run();
                }

                _logger.LogInformation("Done with Inbound Change Event");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Service");
            _folderWatcher.EnableRaisingEvents = false;
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing Service");
            _folderWatcher.Dispose();
            base.Dispose();
        }
    }
}