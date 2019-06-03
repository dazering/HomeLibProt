using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HomeLib.Models.Pages
{
    public class SearchOptions
    {
        [BindProperty(Name = "term")]
        public string SearchTerm { get; set; }
        [BindProperty(Name ="name")]
        public string PropertyName { get; set; } = "Title";
    }
}
