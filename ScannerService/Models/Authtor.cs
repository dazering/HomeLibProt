using System.Collections.Generic;

namespace ScannerService.Models
{
    public class Authtor
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = "Unknown";
        public string MiddleName { get; set; } = "Unknown";
        public string LastName { get; set; } = "Unknown";
        //public string FullName { get; set; }
        public IEnumerable<Book> Books { get; set; }
    }
}
