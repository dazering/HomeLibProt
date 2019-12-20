using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLibServices.Models
{
    public class Book
    {
        public long BookId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        /// <summary>
        /// Books cover in Base64 as string
        /// </summary>
        public string Cover { get; set; }

        public string Annotation { get; set; }

        public LocalPath Path { get; set; }
        public Author Author { get; set; }

        public Book()
        {
            Author = new Author();
            Path = new LocalPath();
        }
    }
}
