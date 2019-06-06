using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScannerService.Models
{
    public class Book
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Annotation { get; set; }
        public string Year { get; set; }
        public string Isbn { get; set; }
        public string Cover { get; set; }
        /// local path to archive
        public string PathArchive { get; set; }
        /// local path to book file in archive
        public string PathBook { get; set; }

        public Authtor Authtor { get; set; }
        public Book()
        {
            Authtor = new Authtor();
        }
    }
}
