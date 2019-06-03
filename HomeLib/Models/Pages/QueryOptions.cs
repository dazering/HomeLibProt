using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace HomeLib.Models.Pages
{
    public class QueryOptions
    {
        [BindProperty(Name ="name")]
        public string AuthorName { get; set; }
        [BindProperty(Name ="literal")]
        public string SearchFirstLiterals { get; set; } = "";
    }
}
