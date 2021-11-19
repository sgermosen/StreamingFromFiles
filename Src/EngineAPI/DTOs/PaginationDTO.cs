namespace EngineAPI.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; }
        private int recordsPerPage = 10;
        private readonly int maxRecordsPerPage = 100;

        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                recordsPerPage = (value > maxRecordsPerPage) ? maxRecordsPerPage : value;
            }
        }

    }
}
