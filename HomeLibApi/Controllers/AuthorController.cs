using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HomeLibServices.Managers;
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
                return Ok(BookManager.GetAuthor(id));
            }
            catch (SqlException)
            {
                return StatusCode(500, "Сервер базы данных не доступен");
            }
        }

        [Route("{literals}")]
        public IActionResult SearchFirstLiteralsOfAuthorName(string literals = "")
        {
            try
            {
                var authors = BookManager.GetAuthorsFirstLiteral(literals);
                if (authors != null)
                {
                    return Ok(authors);
                }

                return NotFound("Автор не найден");
            }
            catch (SqlException)
            {
                return StatusCode(500, "Сервер базы данных не доступен");
            }
        }

        [Route("getauthors/{name}")]
        public IActionResult GetAuthorsByFirstLiterals(string name)
        {
            try
            {
                return Ok(BookManager.SearchAuthorsByFirstLiteral(name));
            }
            catch (SqlException)
            {
                return StatusCode(500, "Сервер базы данных не доступен");
            }
        }

        [Route("search/{name}")]
        public IActionResult SearchAuthor(string name)
        {
            try
            {
                return Ok(BookManager.SearchAuthorsByName(name));
            }
            catch (SqlException)
            {
                return StatusCode(500, "Сервер базы данных не доступен");
            }
        }
    }
}
