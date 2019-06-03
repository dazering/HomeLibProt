using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeLib.Models
{
    /// <summary>
    /// Buffer for info about first chars (or full name) author (or title book) and amount book/authors 
    /// </summary>
    public class MetaInfo
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public long Id { get; set; }
    }
}
