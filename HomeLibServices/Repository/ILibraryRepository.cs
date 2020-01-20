using HomeLibServices.Models;
using System.Collections.Generic;

namespace HomeLibServices.Repository
{
    public interface ILibraryRepository
    {
        bool AddBook(Book book);
        Book GetBook(long id);
        Author GetAuthor(long id);
        IEnumerable<Book> GetAllBooks();
        IEnumerable<Author> GetAllAuthors();
        int CountBooks();
        IEnumerable<(string, int)> GetAuthorsByFirstLiteral(string firstLiterals);
        IEnumerable<Book> SearchBookByTitle(string searchTerm);
        IEnumerable<Author> SearchAuthorByName(string searchTerm);
    }
}