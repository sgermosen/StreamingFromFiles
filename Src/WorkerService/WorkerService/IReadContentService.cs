using WorkerService.Models;

namespace WorkerService
{
    public interface IReadContentService
    {
        void Run(string inputFile, string fileName);
    }

    public class ReadContentService : IReadContentService
    {
        private readonly ILogger<ReadContentService> _logger;
        private readonly IApiConsumerService _apiConsumerService;
        private readonly IFileConstructionService _fileConstructionService;

        public ReadContentService(ILogger<ReadContentService> logger, IApiConsumerService apiConsumerService, IFileConstructionService fileConstructionService)
        {
            _logger = logger;
            _apiConsumerService = apiConsumerService;
            _fileConstructionService = fileConstructionService;
        }

        public void Run(string inputFile, string fileName)
        {
            _logger.LogInformation($"Start to Read file content: {fileName}");
            StreamReader sStreamReader = new StreamReader($"{inputFile}");
            string AllData = sStreamReader.ReadToEnd();
            string[] rows = AllData.Split(",".ToCharArray());
            _logger.LogInformation("End of Read file content");
            var existingBooks = BookContext.Instance.Books;
            var books = new List<Book>();
            foreach (string row in rows)
            {
                Book book ;
                var exist = existingBooks.Any(p => p.Isbn == row);
                if (exist)
                {
                    book = existingBooks.FirstOrDefault(p => p.Isbn == row);
                    book.DataRetrievalType = DataRetrievalType.Cache;
                }
                else
                {
                    book = _apiConsumerService.GetFromApi(row, fileName).Result;
                    if (book != null)
                    {
                        BookContext.Instance.Books.Add(book);
                    }
                }
                if (book != null)
                {
                    books.Add(book);
                }
            }
            // _apiConsumerService.Run(rows, fileName);
            _fileConstructionService.Run(books, fileName);

        }
    }
}
