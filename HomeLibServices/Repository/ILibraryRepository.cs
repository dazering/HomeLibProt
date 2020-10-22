using HomeLibServices.Models;
using System.Collections.Generic;

namespace HomeLibServices.Repository
{
    public interface ILibraryRepository
    {
        bool AddBook(Book book);
        Book GetBook(long id);
        Author GetAuthor(long id);
        int CountBooks();
        IEnumerable<(string, int)> GetAuthorsFirstLiteral(string firstLiterals);
        IEnumerable<Book> SearchBookByTitle(string searchTerm);
        IEnumerable<Author> SearchAuthorByName(string searchTerm);
        IEnumerable<Author> SearchAuthorsByFirstLiteral(string firstLiterals);
    }
}