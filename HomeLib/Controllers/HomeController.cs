using System;
using HomeLibServices.Managers;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.IO;
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
            try
            {
                if (string.IsNullOrWhiteSpace(literals))
                {
                    return View(BookManager.GetAuthorsFirstLiteral(literals));
                }

                return countAuthors > 50 ? View(BookManager.GetAuthorsFirstLiteral(literals)) : View("ShowAuthors", BookManager.SearchAuthorsByFirstLiteral(literals));
            }
            catch (SqlException)
            {
                return ErrorHandler("Упс... Сервер базы данных не доступен.");
            }
        }

        [Route("Books")]
        public IActionResult GetBooks(long id)
        {
            try
            {
                return View("ShowBooks", BookManager.GetAuthor(id).Authorships.Select(a => a.Book));
            }
            catch (SqlException)
            {
                return ErrorHandler("Упс... Сервер базы данных не доступен.");
            }
        }

        [Route("Book")]
        public IActionResult GetBook(long id)
        {
            try
            {
                return View("Book", BookManager.GetBook(id));
            }
            catch (SqlException)
            {
                return ErrorHandler("Упс... Сервер базы данных не доступен.");
            }
        }

        [Route("GetBook/Download")]
        public IActionResult GetFile(long id)
        {
            try
            {
                Book book = BookManager.GetBook(id);
                return File(BookManager.GetBookBytes(book), "application/x-fictionbook", $"{book.Title}.fb2");
            }
            catch (FileNotFoundException)
            {
                return ErrorHandler("Упс... Такой файл не найден");
            }
            catch (SqlException)
            {
                return ErrorHandler("Упс... Сервер базы данных не доступен.");
            }
        }

        [Route("Search")]
        public IActionResult Search(string name, string term)
        {
            try
            {
                if (name == "FullName")
                {
                    return View("ShowAuthors", BookManager.SearchAuthorsByName(term));
                }
                return View("ShowBooks", BookManager.SearchBooksByTitle(term));
            }
            catch (SqlException)
            {
                return ErrorHandler("Упс... Сервер базы данных не доступен.");
            }
        }

        private IActionResult ErrorHandler(string errorMessage)
        {
            TempData["error"] = errorMessage;
            return RedirectToAction("Index", "Error");
        }
    }
}
