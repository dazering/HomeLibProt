using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLibServices.Models
{
    public class LocalPath
    {
        /// <summary>
        /// The file
        /// </summary>
        public Book Book { get; set; }
        /// <summary>
        /// Name of archive containing the file
        /// </summary>
        public string ArchiveName { get; set; }
        /// <summary>
        /// Name of the file
        /// </summary>
        public string FbName { get; set; }
    }
}
