using System.Collections.Generic;

namespace HomeLibServices.Models
{
    public class Author
    {
        public long AuthorId { get; set; }
        public string FirstName { get; set; } = "Unknown";
        public string MiddleName { get; set; } = "Unknown";
        public string LastName { get; set; } = "Unknown";
        public string FullName { get; set; }
        public IEnumerable<Book> Books { get; set; }
    }
}
