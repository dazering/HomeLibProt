namespace HomeLibServices.Models
{
    public class Authorship
    {
        public long Id { get; set; }
        public long BookId { get; set; }
        public Book Book { get; set; }

        public long AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
