using HomeLibServices.Managers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Book = HomeLibServices.Models.Book;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HomeLib.Controllers
{
    public class HomeController : Controller
    {
        private BookManager BookManager;
        public HomeController(BookManager manager)
        {
            BookManager = manager;
        }
        // GET: /<controller>/
        [Route("")]
        public IActionResult Index(string literals = "", int countAuthors = 0)
        {
            if (string.IsNullOrWhiteSpace(literals))
            {
                return View(BookManager.GetAuthorsFirstLiteral(literals));
            }

            return countAuthors > 50 ? View(BookManager.GetAuthorsFirstLiteral(literals)) : View("ShowAuthors", BookManager.SearchAuthorsByFirstLiteral(literals));
        }

        [Route("Books")]
        public IActionResult GetBooks(long id)
        {
            return View("ShowBooks", BookManager.GetAuthor(id).Authorships.Select(a => a.Book));
        }

        [Route("Book")]
        public IActionResult GetBook(long id)
        {
            return View("Book", BookManager.GetBook(id));
        }

        [Route("GetBook/Download")]
        public FileContentResult GetFile(long id)
        {
            Book book = BookManager.GetBook(id);
            return File(BookManager.GetBookBytes(book), "application/x-fictionbook", $"{book.Title}.fb2");
        }

        [Route("Search")]
        public IActionResult Search(string name, string term)
        {
            if (name == "FullName")
            {
                return View("ShowAuthors", BookManager.SearchAuthorsByName(term));
            }
            return View("ShowBooks", BookManager.SearchBooksByTitle(term));
        }
    }
}
