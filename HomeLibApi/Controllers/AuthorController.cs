using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HomeLibServices.Managers;
using HomeLibServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibApi.Controllers
{
    [Route("authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        BookManager BookManager;

        public AuthorController(BookManager bookManager)
        {
            BookManager = bookManager;
        }

        [Route("author/{id}")]
        public IActionResult GetBooksByAuthorId(long id)
        {
            try
            {
                return Ok(new ApiResponse<Author>() { Data = BookManager.GetAuthor(id) });
            }
            catch (SqlException)
            {
                return StatusCode(500, new ApiResponse<string> { Data = "Сервер базы данных не доступен" });
            }
        }

        [Route("{literals?}")]
        public IActionResult SearchFirstLiteralsOfAuthorName(string literals = "")
        {
            try
            {
                var authors = BookManager.GetAuthorsFirstLiteral(literals);
                if (authors != null)
                {
                    return Ok(new ApiResponse<IEnumerable<AlphabetSearchResult>> { Data = authors });
                }

                return NotFound(new ApiResponse<string> { Data = "Автор не найден" });
            }
            catch (SqlException)
            {
                return StatusCode(500, new ApiResponse<string> { Data = "Сервер базы данных не доступен" });
            }
        }

        [Route("getauthors/{name}")]
        public IActionResult GetAuthorsByFirstLiterals(string name)
        {
            try
            {
                return Ok(new ApiResponse<IEnumerable<Author>>() { Data = BookManager.SearchAuthorsByFirstLiteral(name) });
            }
            catch (SqlException)
            {
                return StatusCode(500, new ApiResponse<string>() { Data = "Сервер базы данных не доступен" });
            }
        }

        [Route("search/{name}")]
        public IActionResult SearchAuthor(string name)
        {
            try
            {
                return Ok(new ApiResponse<IEnumerable<Author>>() { Data = BookManager.SearchAuthorsByName(name) });
            }
            catch (SqlException)
            {
                return StatusCode(500, new ApiResponse<string>() { Data = "Сервер базы данных не доступен" });
            }
        }
    }
}
