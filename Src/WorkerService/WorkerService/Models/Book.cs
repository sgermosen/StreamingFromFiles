namespace WorkerService.Models
{
    public class Author
    {
        public string key { get; set; }
    }

    public class Created
    {
        public string type { get; set; }
        public DateTime value { get; set; }
    }

    public class LastModified
    {
        public string type { get; set; }
        public DateTime value { get; set; }
    }

    public class Book
    { 
        public Book()
        { 
        }
        public string Isbn { get; set; }
        public string number_of_pages;
        public DataRetrievalType? DataRetrievalType { get; set; }
        public Type type { get; set; }
        public string title { get; set; }
        public List<Author> authors { get; set; }
        public string publish_date { get; set; }
        public List<string> source_records { get; set; }
        public List<string> publishers { get; set; }
        public List<string> isbn_10 { get; set; }
        public List<string> isbn_13 { get; set; }
        public string physical_format { get; set; }
        public string full_title { get; set; }
        public string subtitle { get; set; }
        public List<int> covers { get; set; }
        public List<Work> works { get; set; }
        public string key { get; set; }
        public int latest_revision { get; set; }
        public int revision { get; set; }
        public Created created { get; set; }
        public LastModified last_modified { get; set; }
    }

    public class Type
    {
        public string key { get; set; }
    }

    public class Work
    {
        public string key { get; set; }
    }


}
