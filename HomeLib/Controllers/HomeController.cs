using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeLib.Infrostructure.Scanner;
using HomeLib.Models.Repository;
using HomeLib.Models.Pages;
using HomeLib.Models;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HomeLib.Controllers
{
    public class HomeController : Controller
    {
        private Scanner scanner;
        private ILibraryRepository libraryRepository;
        private Downloader downloader;
        public HomeController(ILibraryRepository repo, Scanner scan, Downloader downl)
        {
            scanner = scan;
            libraryRepository = repo;
            downloader = downl;
        }
        // GET: /<controller>/
        [Route("")]
        public IActionResult Index(QueryOptions options)
        {
            if (options.AuthorName != null)
            {
                return View("ShowBooks", libraryRepository.SearchBooksByAuthor(options));
            }
            if (options.SearchFirstLiterals.Length<3)
            {
                return View(libraryRepository.SearchAuthorByFirstLiterals(options));
            }
            if(options.SearchFirstLiterals.Length == 3)
            {
                return View("ShowAuthors", libraryRepository.SearchAuthors(options));
            }
            return View();
        }
        [Route("GetBook")]
        public IActionResult GetBook(long id)
        {
            return View("Book", libraryRepository.GetBook(id));
        }
        
        [Route("GetBook/Download")]
        public FileContentResult GetFile(long id)
        {
            Book book = libraryRepository.GetBook(id);
            return File(downloader.GetBytes(book), "application/x-fictionbook", string.Format("{0}.fb2", book.Title));
        }
        
        [Route("Search")]
        public IActionResult Search(SearchOptions options)
        {
            if (options.PropertyName == "FullName")
            {
                return View("ShowAuthors", libraryRepository.GetAuthtors(options));
            }
            return View("ShowBooks",libraryRepository.GetBooks(options));
        }
    }
}
