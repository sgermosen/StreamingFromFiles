namespace WorkerService.Models
{
    public class BookContext
    {
        public static readonly BookContext Instance = new BookContext();

        public BookContext()
        {
            Books = new List<Book>();
        }

        public List<Book> Books { get; set; }

    }
}
