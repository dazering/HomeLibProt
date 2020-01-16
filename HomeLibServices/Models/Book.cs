using System.Collections.Generic;

namespace HomeLibServices.Models
{
    public class Book
    {
        public long BookId { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Isbn { get; set; }
        /// <summary>
        /// Books cover in Base64 as string
        /// </summary>
        public string Cover { get; set; }

        public string Annotation { get; set; }

        public LocalPath Path { get; set; }

        public List<Authorship> Authorships { get; set; }

        public Book()
        {
            Authorships = new List<Authorship>();
            Path = new LocalPath();
        }
    }
}
