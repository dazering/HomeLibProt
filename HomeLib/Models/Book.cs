using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeLib.Models
{
    public class Book
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Annotation { get; set; }
        public string Year { get; set; }
        public string Isbn { get; set; }
        public string Cover { get; set; }
        /// <summary>
        /// local path to archive
        /// </summary>
        public string PathArchive { get; set; }
        /// <summary>
        /// local path to book file in archive
        /// </summary>
        public string PathBook { get; set; }

        public Authtor Authtor { get; set; }
    }
}
