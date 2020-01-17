﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace HomeLib.Models
{
    public class Authtor
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = "Unknown";
        public string MiddleName { get; set; } = "Unknown";
        public string LastName { get; set; } = "Unknown";
        public string FullName { get; set; }
        public IEnumerable<Book> Books { get; set; }
    }
}
