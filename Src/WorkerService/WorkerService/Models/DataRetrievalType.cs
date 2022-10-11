using System.ComponentModel;

namespace WorkerService.Models
{
    public enum DataRetrievalType
    { 
        [Description("Server")]
        Server = 1,
        [Description("Cache")]
        Cache = 2
    }
}
