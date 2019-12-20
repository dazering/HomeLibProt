using System.Collections.Generic;

namespace HomeLibServices.Models
{
    public class Author
    {
        public long AuthorId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<Book> Books { get; set; }
    }
}
