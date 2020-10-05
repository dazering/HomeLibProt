using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using HomeLibServices.Managers;
using HomeLibServices.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibApi.Controllers
{
    [Route("books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        BookManager BookManager;

        public BookController(BookManager bookManager)
        {
            BookManager = bookManager;
        }

        [Route("search/{title}")]
        public IActionResult SearchBook(string title)
        {
            return Ok(BookManager.SearchBooksByTitle(title));
        }

        [Route("book/{bookId}")]
        public IActionResult GetBook(long bookId)
        {
            Book book;
            try
            {
                book = BookManager.GetBook(bookId);
            }
            catch (SqlException)
            {
                return StatusCode(500, "Сервер базы данных не доступен");
            }
            if (book != null)
            {
                return Ok(book);
            }

            return NotFound("Книга не найдена");
        }

        [Route("Download/{id}")]
        public IActionResult GetFile(long id)
        {
            Book book;
            try
            {
                book = BookManager.GetBook(id);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Файл не найден");
            }
            catch (SqlException)
            {
                return StatusCode(500,"Сервер базы данных не доступен.");
            }
            if (book != null)
            {
                return File(BookManager.GetBookBytes(book), "application/x-fictionbook", $"{book.Title}.fb2");
            }
            return NotFound("Книга не найдена");
        }
    }
}
