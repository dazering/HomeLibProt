using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
            try
            {
                return Ok(new ApiResponse<IEnumerable<Book>> { Data = BookManager.SearchBooksByTitle(title) });
            }
            catch (SqlException)
            {
                return StatusCode(500, new ApiResponse<string> { Data = "Сервер базы данных не доступен" });
            }

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
                return StatusCode(500, new ApiResponse<string> { Data = "Сервер базы данных не доступен" });
            }
            if (book != null)
            {
                return Ok(new ApiResponse<Book> { Data = book });
            }

            return NotFound(new ApiResponse<string> { Data = "Книга не найдена" });
        }

        [Route("Download/{id}")]
        public IActionResult GetFile(long id)
        {
            try
            {
                var book = BookManager.GetBook(id);
                if (book != null)
                {
                    return File(BookManager.GetBookBytes(book), "application/x-fictionbook", $"{book.Title}.fb2");
                }
                return NotFound(new ApiResponse<string> { Data = "Книга не найдена" });
            }
            catch (FileNotFoundException)
            {
                return NotFound(new ApiResponse<string> { Data = "Файл книги не найден" });
            }
            catch (SqlException)
            {
                return StatusCode(500, new ApiResponse<string> { Data = "Сервер базы данных не доступен" });
            }
        }
    }
}
