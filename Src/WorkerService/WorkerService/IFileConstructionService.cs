using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using WorkerService.Models;
using System.ComponentModel;
using WorkerService.Extensions;
using System.IO;

namespace WorkerService
{
    public interface IFileConstructionService
    {
        void Run(List<Book> items, string fileName);
    }

    public class FileConstructionService : IFileConstructionService
    {
        private readonly ILogger<ApiConsumerService> _logger;
        private readonly string _outputFolder;

        public FileConstructionService(ILogger<ApiConsumerService> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _outputFolder = settings.Value.OutputFolder;
        }

        public async void Run(List<Book> items, string fileName)
        { 
            _logger.LogInformation("Start Create the File");
            using (StreamWriter writer = new StreamWriter($"{_outputFolder}\\{fileName}.csv"))
            {
                writer.WriteLine($"Row Number,Data Retrieval Type,ISBN,Title,Subtitle,Author Name(s),Number of Pages,Publish Date");
                int rowNum = 0;
                foreach (var item in items)
                {
                    writer.WriteLine($"{++rowNum},{item.DataRetrievalType.GetEnumDescription()},{item.Isbn},{item.title},{item.subtitle},{string.Join("; ", item.authors.Select(u => u.key))},{item.number_of_pages},{item.publish_date}");
                } 
            }
            _logger.LogInformation("End File creation");
        }
    }
}
